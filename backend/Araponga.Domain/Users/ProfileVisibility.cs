namespace Araponga.Domain.Users;

public enum ProfileVisibility
{
    Public = 1,           // Visível para todos
    ResidentsOnly = 2,   // Apenas moradores dos territórios onde o usuário é membro
    Private = 3          // Apenas o próprio usuário
}
