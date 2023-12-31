﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Formatting;

namespace MakeConst
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MakeConstCodeFixProvider)), Shared]
    public class MakeConstCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MakeConstAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => MakeConstAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private static async Task<Document> MakeConstAsync(Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            // Remove the leading trivia from the local declaration.
            SyntaxToken firstToken = localDeclaration.GetFirstToken();
            SyntaxTriviaList leadingTrivia = firstToken.LeadingTrivia;
            LocalDeclarationStatementSyntax trimmedLocal = localDeclaration.ReplaceToken(
                firstToken, firstToken.WithLeadingTrivia(SyntaxTriviaList.Empty));

            // Create a const token with the leading trivia.
            SyntaxToken constToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.ConstKeyword, SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));

            // Insert the const token into the modifiers list, creating a new modifiers list.
            SyntaxTokenList newModifiers = trimmedLocal.Modifiers.Insert(0, constToken);
            // Produce the new local declaration.
            LocalDeclarationStatementSyntax newLocal = trimmedLocal
                .WithModifiers(newModifiers)
                .WithDeclaration(localDeclaration.Declaration);

            // Add an annotation to format the new local declaration.
            LocalDeclarationStatementSyntax formattedLocal = newLocal.WithAdditionalAnnotations(Formatter.Annotation);

            // Replace the old local declaration with the new local declaration.
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode newRoot = oldRoot.ReplaceNode(localDeclaration, formattedLocal);

            // Return document with transformed tree.
            return document.WithSyntaxRoot(newRoot);
        }

        //private async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        //{
        //    // Compute new uppercase name.
        //    var identifierToken = typeDecl.Identifier;
        //    var newName = identifierToken.Text.ToUpperInvariant();

        //    // Get the symbol representing the type to be renamed.
        //    var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
        //    var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

        //    // Produce a new solution that has all references to that type renamed, including the declaration.
        //    var originalSolution = document.Project.Solution;
        //    var optionSet = originalSolution.Workspace.Options;
        //    var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

        //    // Return the new solution with the now-uppercase type name.
        //    return newSolution;
        //}
    }
}
