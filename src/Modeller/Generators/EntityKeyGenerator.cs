using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Generators;

public class EntityKeyGenerator
{
    private readonly Enterprise _enterprise;
    private readonly EntityType _entity;

    public EntityKeyGenerator(Enterprise enterprise, EntityType entity)
    {
        _enterprise = enterprise;
        _entity = entity;
    }

    public string Generate()
    {
        var project = SyntaxFactory.IdentifierName(_enterprise.Project.Value);
        var domain = SyntaxFactory.IdentifierName("Domain");
        var entities = SyntaxFactory.IdentifierName("Entities");
        var qualifiedName = SyntaxFactory.QualifiedName(SyntaxFactory.QualifiedName(project, domain),entities);
        var ns = SyntaxFactory.FileScopedNamespaceDeclaration(qualifiedName);

        var ps = _entity.Key?.PrimaryKeyFieldList
            .Select(f =>
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(f.Name.Value))
                    .WithType(SyntaxFactory.ParseTypeName(f.DataType.Name)))
            .ToList();

        var e = SyntaxFactory.RecordDeclaration(
                kind: SyntaxKind.RecordDeclaration,
                keyword: SyntaxFactory.Token(SyntaxKind.RecordKeyword),
                identifier: _entity.Name.Value)
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(ps)))
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword));

        return ns.AddMembers(e)
            .NormalizeWhitespace()
            .ToFullString();
    }
}