namespace Araponga.Domain.Chat;

/// <summary>
/// Tipo de conteúdo da mensagem.
/// MVP: Text.
/// Fase 2: Media / Reference / System.
/// </summary>
public enum MessageContentType
{
    Text = 1,
    Media = 2,
    Reference = 3,
    System = 4
}
