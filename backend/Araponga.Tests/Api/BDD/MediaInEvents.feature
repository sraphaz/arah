#language: pt-BR
Funcionalidade: Mídias em Eventos
  Como um organizador de eventos
  Eu quero adicionar mídias aos eventos
  Para promover e documentar eventos

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente
    E que o usuário "João" está autenticado

  Cenário: Criar evento com imagem de capa
    Dado que existe uma imagem "capa.jpg" disponível
    Quando o usuário cria um evento com a imagem de capa "capa.jpg"
    Então o evento deve ser criado com sucesso
    E o evento deve ter uma imagem de capa

  Cenário: Criar evento com capa e mídias adicionais
    Dado que existe uma imagem "capa.jpg" disponível
    E que existem 3 imagens adicionais disponíveis
    Quando o usuário cria um evento com a capa e as 3 imagens adicionais
    Então o evento deve ser criado com sucesso
    E o evento deve ter 1 imagem de capa
    E o evento deve ter 3 mídias adicionais

  Cenário: Criar evento com mais de 5 mídias adicionais (limite excedido)
    Dado que existe uma imagem "capa.jpg" disponível
    E que existem 6 imagens adicionais disponíveis
    Quando o usuário tenta criar um evento com a capa e as 6 imagens adicionais
    Então deve retornar erro "additional media items allowed"

  Cenário: Criar evento com capa e vídeo adicional
    Dado que existe uma imagem "capa.jpg" disponível
    E que existe um vídeo "video1.mp4" disponível
    Quando o usuário cria um evento com a capa e o vídeo adicional
    Então o evento deve ser criado com sucesso
    E o evento deve ter 1 imagem de capa
    E o evento deve ter 1 vídeo adicional

  Cenário: Cancelar evento remove mídias associadas
    Dado que existe um evento com 1 capa e 3 mídias adicionais
    Quando o usuário cancela o evento
    Então o evento deve ser cancelado
    E as 4 mídias devem ser removidas
