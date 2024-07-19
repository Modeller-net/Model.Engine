using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Generators;

public class EntityKeyGenerator
{
    private readonly EntityKeyBuilder _builder;

    public EntityKeyGenerator(EntityKeyBuilder builder)
    {
        _builder = builder;
    }

    public string Generate()
    {
        var ns = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName("Domain"));

        var ps = _builder.Fields
            .Select(f =>
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(f.Name.Value))
                    .WithType(SyntaxFactory.ParseTypeName(f.DataType.DataType)))
            .ToList();

        var e = SyntaxFactory.RecordDeclaration(
                kind: SyntaxKind.RecordDeclaration,
                keyword: SyntaxFactory.Token(SyntaxKind.RecordKeyword),
                identifier: _builder.Name.Value.Value)
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(ps)))
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword));

        return ns.AddMembers(e)
            .NormalizeWhitespace()
            .ToFullString();
    }
}