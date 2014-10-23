using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace DiagnosticAndCodeFix
{
    [ExportCodeFixProvider(DiagnosticAnalyzer.DiagnosticId, LanguageNames.CSharp)]
    public class CodeFixProvider : ICodeFixProvider
    {
        public IEnumerable<string> GetFixableDiagnosticIds()
        {
            return new[] { DiagnosticAnalyzer.DiagnosticId };
        }

        public async Task<IEnumerable<CodeAction>> GetFixesAsync(Document document, TextSpan span, IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = null;
            var forNode = root.FindToken(span.Start).Parent;
            var forStatement = forNode as ForStatementSyntax;
            if (forStatement != null)
            {
                var newFor = forStatement.WithStatement(SyntaxFactory.Block(forStatement.Statement))
                    .WithAdditionalAnnotations(Formatter.Annotation);
                newRoot = root.ReplaceNode(forStatement, newFor);
            }
            else
            {
                var foreachStatment = forNode as ForEachStatementSyntax;
                var newFor = foreachStatment.WithStatement(SyntaxFactory.Block(foreachStatment.Statement))
                    .WithAdditionalAnnotations(Formatter.Annotation);
                newRoot = root.ReplaceNode(foreachStatment, newFor);
            }

            return new[] { CodeAction.Create("Add Braces", document.WithSyntaxRoot(newRoot)) };
        }

    }
}