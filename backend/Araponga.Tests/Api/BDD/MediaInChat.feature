#language: pt-BR
Funcionalidade: Mídias em Chat
  Como um usuário
  Eu quero enviar mídias no chat
  Para compartilhar imagens e áudios em conversas

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente
    E que existe um usuário "Maria" como residente
    E que o usuário "João" está autenticado

  Cenário: Enviar imagem no chat
    Dado que existe uma imagem "imagem1.jpg" disponível
    Quando o usuário envia uma mensagem com a imagem "imagem1.jpg"
    Então a mensagem deve ser enviada com sucesso
    E a mensagem deve conter a imagem

  Cenário: Enviar áudio no chat
    Dado que existe um áudio "audio1.mp3" disponível
    Quando o usuário envia uma mensagem com o áudio "audio1.mp3"
    Então a mensagem deve ser enviada com sucesso
    E a mensagem deve conter o áudio

  Cenário: Tentar enviar vídeo no chat (bloqueado)
    Dado que existe um vídeo "video1.mp4" disponível
    Quando o usuário tenta enviar uma mensagem com o vídeo "video1.mp4"
    Então deve retornar erro "video"

  Cenário: Enviar imagem maior que 5MB (limite excedido)
    Dado que existe uma imagem de 6MB disponível
    Quando o usuário tenta enviar uma mensagem com a imagem
    Então deve retornar erro "size exceeds"

  Cenário: Enviar áudio maior que 2MB (limite excedido)
    Dado que existe um áudio de 3MB disponível
    Quando o usuário tenta enviar uma mensagem com o áudio
    Então deve retornar erro "size exceeds"
