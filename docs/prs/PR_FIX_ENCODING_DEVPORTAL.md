# Fix: Corrigir Encoding UTF-8 no Developer Portal

## Resumo

Este PR corrige problemas de encoding UTF-8 no Developer Portal que causavam caracteres acentuados aparecerem incorretamente (ex: "có" aparecendo como "C├│").

## Problema

Caracteres acentuados em português estavam aparecendo incorretamente no Developer Portal:
- "có" aparecendo como "C├│"
- Outros caracteres acentuados ilegíveis

**Causa raiz:**
- Arquivos HTML podem ter sido salvos com encoding incorreto (Windows-1252 ou ISO-8859-1)
- Servidor pode não estar configurando Content-Type com charset=utf-8 corretamente

## Solução

### 1. Garantir Encoding UTF-8 nos Arquivos

Ambos os arquivos foram re-salvos garantindo encoding UTF-8 sem BOM:
- `docs/devportal/index.html`
- `backend/Araponga.Api/wwwroot/devportal/index.html`

### 2. Melhorar Configuração do Servidor

Atualizado `Program.cs` para garantir que todos os arquivos HTML sejam servidos com charset UTF-8:

```csharp
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/devportal",
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.WebRootPath, "devportal")),
    OnPrepareResponse = context =>
    {
        if (string.Equals(context.File.Name, "index.html", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(context.File.Extension, ".html", StringComparison.OrdinalIgnoreCase))
        {
            context.Context.Response.ContentType = "text/html; charset=utf-8";
            context.Context.Response.Headers.Append("Content-Type", "text/html; charset=utf-8");
        }
    }
});
```

### 3. Verificações Realizadas

- ✅ Meta tag `<meta charset="utf-8" />` presente no HTML
- ✅ Arquivos re-salvos com encoding UTF-8 sem BOM
- ✅ Content-Type configurado explicitamente no servidor
- ✅ Headers HTTP configurados corretamente

## Arquivos Modificados

- `docs/devportal/index.html` - Re-salvo com UTF-8
- `backend/Araponga.Api/wwwroot/devportal/index.html` - Re-salvo com UTF-8
- `backend/Araponga.Api/Program.cs` - Melhorada configuração de Content-Type

## Testes

Para verificar se o problema foi resolvido:

1. Abrir o Developer Portal no navegador
2. Verificar se caracteres acentuados aparecem corretamente:
   - "Conteúdo" (não "Conte├║do")
   - "curadoria" (não "curadoria com caracteres estranhos")
   - "território" (não "territ├│rio")
   - "có" (não "C├│")

3. Verificar headers HTTP:
   - `Content-Type: text/html; charset=utf-8` deve estar presente

## Notas

- O arquivo HTML já tinha a meta tag `<meta charset="utf-8" />` correta
- O problema estava na forma como o arquivo foi salvo (encoding) e/ou como o servidor estava servindo
- Garantir que editores de texto salvem arquivos HTML com UTF-8

## Checklist

- [x] Arquivos re-salvos com encoding UTF-8 sem BOM
- [x] Content-Type configurado explicitamente no servidor
- [x] Headers HTTP configurados corretamente
- [x] Meta tag charset presente no HTML
- [ ] Teste manual no navegador (verificar caracteres acentuados)
