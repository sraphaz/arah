(function () {
  // Banner sempre visível - sem lógica de redirect

  var explorerButton = document.querySelector('[data-explorer-button]');
  var explorerContainer = document.getElementById('openapi-browser');
  var explorerStatus = document.querySelector('[data-explorer-status]');
  var explorerLoaded = false;

  function setStatus(message, isError) {
    if (!explorerStatus) {
      return;
    }

    explorerStatus.textContent = message;
    explorerStatus.style.color = isError ? '#f26d6d' : 'inherit';
  }

  async function resolveSpecUrl() {
    var preferred = '/swagger/v1/swagger.json';
    try {
      var response = await fetch(preferred, { cache: 'no-store' });
      if (response.ok) {
        return preferred;
      }
    } catch (error) {
      // ignore
    }

    return './openapi.json';
  }

  async function loadExplorer() {
    if (explorerLoaded) {
      explorerContainer?.classList.toggle('swagger-hidden');
      return;
    }

    if (!explorerContainer) {
      setStatus('Explorer indisponível: container não encontrado.', true);
      return;
    }

    setStatus('Carregando especificação…');
    var specUrl = await resolveSpecUrl();

    try {
      var response = await fetch(specUrl, { cache: 'no-store' });
      if (!response.ok) {
        setStatus('Não foi possível carregar a especificação.', true);
        return;
      }

      var spec = await response.json();
      renderSpec(spec, explorerContainer);
      explorerLoaded = true;
      explorerContainer.classList.remove('swagger-hidden');
      setStatus('Explorer pronto.');
    } catch (error) {
      setStatus('Falha ao processar a especificação.', true);
    }
  }

  if (explorerButton) {
    explorerButton.addEventListener('click', function () {
      if (explorerContainer && explorerLoaded) {
        explorerContainer.classList.toggle('swagger-hidden');
        explorerButton.setAttribute(
          'aria-expanded',
          explorerContainer.classList.contains('swagger-hidden') ? 'false' : 'true'
        );
        return;
      }

      loadExplorer();
      explorerButton.setAttribute('aria-expanded', 'true');
    });
  }

  function renderSpec(spec, container) {
    container.innerHTML = '';
    if (!spec || !spec.paths) {
      container.textContent = 'Spec inválida.';
      return;
    }

    var tagMap = {};
    Object.keys(spec.paths).forEach(function (path) {
      var methods = spec.paths[path];
      Object.keys(methods).forEach(function (method) {
        var operation = methods[method];
        var tags = operation.tags && operation.tags.length ? operation.tags : ['General'];
        tags.forEach(function (tag) {
          tagMap[tag] = tagMap[tag] || [];
          tagMap[tag].push({ method: method.toUpperCase(), path: path, operation: operation });
        });
      });
    });

    Object.keys(tagMap).sort().forEach(function (tag) {
      var section = document.createElement('section');
      section.className = 'openapi-tag';
      var heading = document.createElement('h3');
      heading.textContent = tag + ' (' + tagMap[tag].length + ')';
      section.appendChild(heading);

      tagMap[tag].forEach(function (entry) {
        var details = document.createElement('details');
        details.className = 'endpoint';
        var summary = document.createElement('summary');
        var method = document.createElement('span');
        method.className = 'method ' + entry.method.toLowerCase();
        method.textContent = entry.method;
        summary.appendChild(method);
        var title = document.createElement('span');
        title.textContent = entry.path + (entry.operation.summary ? ' — ' + entry.operation.summary : '');
        summary.appendChild(title);
        details.appendChild(summary);

        var body = document.createElement('div');
        body.style.marginTop = '10px';

        if (entry.operation.parameters && entry.operation.parameters.length) {
          var params = document.createElement('p');
          params.innerHTML = '<strong>Parâmetros:</strong> ' + entry.operation.parameters.map(function (param) {\n            return param.name + ' (' + param.in + ')';\n          }).join(', ');
          body.appendChild(params);
        }

        if (entry.operation.requestBody) {
          var requestSchema = extractSchemaName(entry.operation.requestBody);
          var req = document.createElement('p');
          req.innerHTML = '<strong>Body:</strong> ' + requestSchema;
          body.appendChild(req);
        }

        if (entry.operation.responses) {
          var responseList = document.createElement('ul');
          responseList.style.margin = '6px 0 0';
          Object.keys(entry.operation.responses).forEach(function (status) {
            var responseSchema = extractSchemaName(entry.operation.responses[status]);
            var item = document.createElement('li');
            item.textContent = status + (responseSchema ? ' → ' + responseSchema : '');
            responseList.appendChild(item);
          });
          body.appendChild(responseList);
        }

        details.appendChild(body);
        section.appendChild(details);
      });

      container.appendChild(section);
    });

    if (spec.components && spec.components.schemas) {
      var schemasSection = document.createElement('section');
      schemasSection.className = 'openapi-tag';
      var schemasTitle = document.createElement('h3');
      schemasTitle.textContent = 'Schemas';
      schemasSection.appendChild(schemasTitle);

      var grid = document.createElement('div');
      grid.className = 'schema-grid';
      Object.keys(spec.components.schemas).sort().forEach(function (name) {
        var schema = spec.components.schemas[name];
        var card = document.createElement('div');
        card.className = 'schema-card';
        var title = document.createElement('h4');
        title.textContent = name;
        card.appendChild(title);

        if (schema.properties) {
          var list = document.createElement('ul');
          Object.keys(schema.properties).forEach(function (prop) {
            list.appendChild(document.createElement('li')).textContent = prop;
          });
          card.appendChild(list);
        } else {
          var empty = document.createElement('p');
          empty.textContent = 'Sem propriedades detalhadas.';
          card.appendChild(empty);
        }

        grid.appendChild(card);
      });

      schemasSection.appendChild(grid);
      container.appendChild(schemasSection);
    }
  }

  function extractSchemaName(target) {
    if (!target) {
      return '';
    }

    if (target.content && target.content['application/json'] && target.content['application/json'].schema) {
      return extractSchemaName(target.content['application/json'].schema);
    }

    if (target.schema) {
      return extractSchemaName(target.schema);
    }

    if (target.$ref) {
      return target.$ref.split('/').pop();
    }

    if (target.type === 'array' && target.items) {
      var itemsName = extractSchemaName(target.items);
      return itemsName ? 'Array<' + itemsName + '>' : 'Array';
    }

    return target.title || target.type || '';
  }
})();
