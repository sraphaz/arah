namespace Araponga.Domain.Users;

public enum ContactVisibility
{
    Public = 1,           // Email, telefone, endereço visíveis para todos
    ResidentsOnly = 2,    // Apenas moradores validados
    Private = 3           // Nunca visível publicamente
}
