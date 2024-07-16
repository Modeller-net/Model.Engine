using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Generators;

public class EntityGenerator
{
    private readonly EntityBuilder _builder;

    public EntityGenerator(EntityBuilder builder)
    {
        _builder = builder;
    }

    public string Generate()
    {
        var ns = SyntaxFactory
            .NamespaceDeclaration(SyntaxFactory.ParseName("Domain"));

        var e = SyntaxFactory.ClassDeclaration(_builder.Name.Name.Value)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        var p = SyntaxFactory.List<PropertyDeclarationSyntax>();
        foreach (var f in _builder.Fields)
        {
            var t = SyntaxFactory.ParseTypeName(f.DataType.DataType);
            var pds = SyntaxFactory.PropertyDeclaration(t, f.Name.Value)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
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