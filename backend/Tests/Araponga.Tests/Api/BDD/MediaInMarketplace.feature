#language: pt-BR
Funcionalidade: Mídias em Marketplace
  Como um vendedor
  Eu quero adicionar mídias aos meus itens
  Para mostrar os produtos de forma atrativa

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente
    E que o usuário "João" está autenticado
    E que existe uma loja "Loja do João"

  Cenário: Criar item com imagem principal
    Dado que existe uma imagem "produto1.jpg" disponível
    Quando o usuário cria um item com a imagem "produto1.jpg"
    Então o item deve ser criado com sucesso
    E o item deve ter uma imagem principal

  Cenário: Criar item com múltiplas imagens
    Dado que existem 5 imagens disponíveis
    Quando o usuário cria um item com as 5 imagens
    Então o item deve ser criado com sucesso
    E o item deve ter 5 mídias
    E a primeira imagem deve ser a imagem principal

  Cenário: Criar item com mais de 10 mídias (limite excedido)
    Dado que existem 11 imagens disponíveis
    Quando o usuário tenta criar um item com as 11 imagens
    Então deve retornar erro "media items allowed"

  Cenário: Criar item com vídeo e imagens
    Dado que existe uma imagem "produto1.jpg" disponível
    E que existe um vídeo "video1.mp4" disponível
    Quando o usuário cria um item com a imagem e o vídeo
    Então o item deve ser criado com sucesso
    E o item deve ter 2 mídias
    E a primeira mídia deve ser a imagem principal

  Cenário: Arquivar item remove mídias associadas
    Dado que existe um item com 3 mídias
    Quando o usuário arquiva o item
    Então o item deve ser arquivado
    E as 3 mídias devem ser removidas
