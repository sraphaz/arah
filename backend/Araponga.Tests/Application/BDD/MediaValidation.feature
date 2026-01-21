#language: pt-BR
Funcionalidade: Validação de Mídia
  Como um sistema
  Eu quero validar mídias antes de processá-las
  Para garantir segurança e qualidade

  Contexto:
    Dado que existe um território "Vale do Itamambuca"
    E que existe um usuário "João" como residente

  Cenário: Validar tamanho de imagem dentro do limite
    Dado que existe uma imagem de 2MB
    Quando o sistema valida a imagem
    Então a validação deve passar
    E a imagem deve ser aceita

  Cenário: Validar tamanho de imagem excedendo limite
    Dado que existe uma imagem de 15MB
    Quando o sistema valida a imagem
    Então a validação deve falhar
    E deve retornar erro de validação "size exceeds"

  Cenário: Validar tipo MIME permitido
    Dado que existe um arquivo com tipo MIME "image/jpeg"
    Quando o sistema valida o tipo MIME
    Então a validação deve passar
    E o arquivo deve ser aceito

  Cenário: Validar tipo MIME não permitido
    Dado que existe um arquivo com tipo MIME "application/pdf"
    Quando o sistema valida o tipo MIME
    Então a validação deve falhar
    E deve retornar erro de validação "not allowed"

  Cenário: Validar quantidade de mídias dentro do limite
    Dado que existem 5 mídias
    Quando o sistema valida a quantidade
    Então a validação deve passar
    E as mídias devem ser aceitas

  Cenário: Validar quantidade de mídias excedendo limite
    Dado que existem 11 mídias
    Quando o sistema valida a quantidade
    Então a validação deve falhar
    E deve retornar erro de validação "Maximum media count exceeded"
