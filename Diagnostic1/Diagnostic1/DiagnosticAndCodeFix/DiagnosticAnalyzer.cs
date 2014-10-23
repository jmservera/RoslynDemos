using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiagnosticAndCodeFix
{
    // TODO: Consider implementing other interfaces that implement IDiagnosticAnalyzer instead of or in addition to ISymbolAnalyzer

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DiagnosticAnalyzer : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "ForBracesDiagnostic";
        internal const string Description = "Should use braces";
        internal const string MessageFormat = "'{0}' does not contain braces";
        internal const string Category = "Style";

        internal static DiagnosticDescriptor Rule = 
            new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Error, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest
        {
            get
            {
                return ImmutableArray.Create(SyntaxKind.ForStatement, SyntaxKind.ForEachStatement);
            }
        }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var forStatement = node as ForStatementSyntax;
            if (forStatement != null && forStatement.Statement != null && !forStatement.Statement.IsKind(SyntaxKind.Block))
            {
                var diagnostic = Diagnostic.Create(Rule, forStatement.ForKeyword.GetLocation(), forStatement.ForKeyword.Text);
                addDiagnostic(diagnostic);
            }
            else
            {
                var foreachStatement = node as ForEachStatementSyntax;
                if (foreachStatement != null && foreachStatement.Statement != null && !foreachStatement.Statement.IsKind(SyntaxKind.Block))
                {
                    var diagnostic = Diagnostic.Create(Rule, foreachStatement.ForEachKeyword.GetLocation(), foreachStatement.ForEachKeyword.Text);
                    addDiagnostic(diagnostic);
                }
            }
        }
    }
}
