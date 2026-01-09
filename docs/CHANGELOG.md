# Changelog

## Unreleased
- Refatorado o território para ser puramente geográfico e movida a lógica social para entidades e serviços de vínculo.
- Adicionada documentação revisada de histórias de usuário em `docs/user-stories.md`.
- Atualizados os endpoints de API para busca/territórios próximos/sugestões e gestão de vínculos.
- Ajustados os filtros de feed/mapa/saúde para usar papéis sociais de vínculo.
- Adicionada persistência opcional em Postgres com mapeamentos EF Core ao lado do provedor InMemory.
- Adicionada página inicial estática mínima da API com UI auxiliar de configuração.
- Adicionado tratamento estruturado de erros com `ProblemDetails` e ganchos de testes para cenários de exceção.
- Publicado o portal de autosserviço como site estático em `docs/` para GitHub Pages, com links para documentação e changelog.
