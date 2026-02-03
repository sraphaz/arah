using System.Reflection;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Garante padronização entre entidades (Records) duplicadas: mesma tabela mapeada em
/// Araponga.Infrastructure/Postgres/Entities e Araponga.Infrastructure.Shared/Postgres/Entities.
/// Se alguém alterar uma entidade e não a outra, o teste falha (detecção de drift).
/// Ver docs/VALIDACAO_INFRAESTRUTURA_INTEGRIDADE.md §2.3 e §6.
/// </summary>
public sealed class DuplicateEntityStandardizationTests
{
    private static readonly Assembly InfraAssembly = typeof(Araponga.Infrastructure.Postgres.Entities.TerritoryRecord).Assembly;
    private const string InfraEntitiesNamespace = "Araponga.Infrastructure.Postgres.Entities";

    private static readonly Assembly SharedAssembly = typeof(Araponga.Infrastructure.Shared.Postgres.Entities.TerritoryRecord).Assembly;
    private const string SharedEntitiesNamespace = "Araponga.Infrastructure.Shared.Postgres.Entities";

    [Fact]
    public void DuplicateEntities_Infrastructure_And_Shared_Have_Same_Properties()
    {
        var infraRecords = GetRecordTypes(InfraAssembly, InfraEntitiesNamespace);
        var sharedRecords = GetRecordTypes(SharedAssembly, SharedEntitiesNamespace);

        var byName = infraRecords
            .Select(t => (Name: t.Name, Type: t, Source: "Infrastructure"))
            .Concat(sharedRecords.Select(t => (Name: t.Name, Type: t, Source: "Shared")))
            .GroupBy(x => x.Name)
            .Where(g => g.Count() > 1)
            .ToList();

        var errors = new List<string>();

        foreach (var group in byName)
        {
            var list = group.ToList();
            var first = list[0];
            var firstProps = GetPropertySignatures(first.Type);

            for (var i = 1; i < list.Count; i++)
            {
                var other = list[i];
                var otherProps = GetPropertySignatures(other.Type);

                var onlyInFirst = firstProps.Keys.Except(otherProps.Keys).ToList();
                var onlyInOther = otherProps.Keys.Except(firstProps.Keys).ToList();
                var typeMismatch = firstProps
                    .Where(kv => otherProps.TryGetValue(kv.Key, out var t) && t != kv.Value)
                    .Select(kv => $"{kv.Key}: {kv.Value} vs {otherProps[kv.Key]}")
                    .ToList();

                if (onlyInFirst.Count > 0 || onlyInOther.Count > 0 || typeMismatch.Count > 0)
                {
                    errors.Add($"Entidade duplicada '{group.Key}':");
                    if (onlyInFirst.Count > 0)
                        errors.Add($"  - Só em {first.Source}: {string.Join(", ", onlyInFirst)}");
                    if (onlyInOther.Count > 0)
                        errors.Add($"  - Só em {other.Source}: {string.Join(", ", onlyInOther)}");
                    if (typeMismatch.Count > 0)
                        errors.Add($"  - Tipo diferente: {string.Join("; ", typeMismatch)}");
                    errors.Add($"  Atualize os dois lados (Infrastructure e Shared) para manter padronização. Ver docs/VALIDACAO_INFRAESTRUTURA_INTEGRIDADE.md §6.");
                }
            }
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    private static List<Type> GetRecordTypes(Assembly assembly, string ns)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == ns && t.Name.EndsWith("Record"))
            .ToList();
    }

    private static Dictionary<string, string> GetPropertySignatures(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p.PropertyType.FullName ?? p.PropertyType.Name);
    }
}
