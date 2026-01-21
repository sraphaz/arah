#language: pt-BR
@tag1
Funcionalidade: Upload de Mídia
  Como um usuário
  Eu quero fazer upload de mídias
  Para compartilhar conteúdo visual com a comunidade

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente

  Cenário: Upload de imagem com sucesso
    Dado que o usuário "João" está autenticado
    Quando ele faz upload de uma imagem de 2MB
    Então o upload deve ser concluído com sucesso
    E a mídia deve estar disponível para uso

  Cenário: Upload de vídeo excedendo limite
    Dado que o usuário "João" está autenticado
    Quando ele tenta fazer upload de um vídeo de 60MB
    Então deve retornar erro "size exceeds"

  Cenário: Upload de áudio com sucesso
    Dado que o usuário "João" está autenticado
    Quando ele faz upload de um áudio de 1MB
    Então o upload deve ser concluído com sucesso
    E a mídia deve estar disponível para uso

  Cenário: Upload de tipo de arquivo não permitido
    Dado que o usuário "João" está autenticado
    Quando ele tenta fazer upload de um arquivo PDF
    Então deve retornar erro "not allowed"
