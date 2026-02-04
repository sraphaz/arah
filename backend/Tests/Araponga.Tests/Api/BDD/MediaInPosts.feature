#language: pt-BR
Funcionalidade: Mídias em Posts
  Como um usuário
  Eu quero adicionar mídias aos meus posts
  Para enriquecer o conteúdo compartilhado

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente
    E que o usuário "João" está autenticado

  Cenário: Criar post com uma imagem
    Dado que existe uma imagem "imagem1.jpg" disponível
    Quando o usuário cria um post com a imagem "imagem1.jpg"
    Então o post deve ser criado com sucesso
    E o post deve conter 1 mídia

  Cenário: Criar post com múltiplas imagens
    Dado que existem 5 imagens disponíveis
    Quando o usuário cria um post com as 5 imagens
    Então o post deve ser criado com sucesso
    E o post deve conter 5 mídias

  Cenário: Criar post com mais de 10 mídias (limite excedido)
    Dado que existem 11 imagens disponíveis
    Quando o usuário tenta criar um post com as 11 imagens
    Então deve retornar erro "media items allowed"

  Cenário: Criar post com vídeo e imagem
    Dado que existe uma imagem "imagem1.jpg" disponível
    E que existe um vídeo "video1.mp4" disponível
    Quando o usuário cria um post com a imagem e o vídeo
    Então o post deve ser criado com sucesso
    E o post deve conter 2 mídias

  Cenário: Criar post com mais de um vídeo (limite excedido)
    Dado que existem 2 vídeos disponíveis
    Quando o usuário tenta criar um post com os 2 vídeos
    Então deve retornar erro "video"

  Cenário: Deletar post remove mídias associadas
    Dado que existe um post com 3 mídias
    Quando o usuário deleta o post
    Então o post deve ser deletado
    E as 3 mídias devem ser removidas
