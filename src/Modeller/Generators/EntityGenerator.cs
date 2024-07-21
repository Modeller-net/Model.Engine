using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Generators;

public class EntityGenerator
{
    private readonly Enterprise _enterprise;
    private readonly EntityType _entity;
    public EntityGenerator(Enterprise enterprise, EntityType entity)
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
 
        var e = SyntaxFactory.ClassDeclaration(_entity.Name.Value)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        var p = SyntaxFactory.List<PropertyDeclarationSyntax>();
        foreach (var f in _entity.Fields)
        {
            var t = SyntaxFactory.ParseTypeName(f.DataType.Name);
            var pds = SyntaxFactory.PropertyDeclaration(t, f.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                );
            p = p.Add(pds);
        }

        return ns
            .AddMembers(e.AddMembers([.. p]))
            .NormalizeWhitespace()
            .ToFullString();
    }
}
