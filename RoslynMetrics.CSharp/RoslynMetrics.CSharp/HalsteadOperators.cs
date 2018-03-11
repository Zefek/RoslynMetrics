using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;

namespace RoslynMetrics.CSharp
{
	internal sealed class HalsteadOperators
	{
		public readonly static IEnumerable<SyntaxKind> All;

		static HalsteadOperators()
		{
			SyntaxKind[] syntaxKindArray = new SyntaxKind[] { SyntaxKind.DotToken, SyntaxKind.EqualsToken, SyntaxKind.SemicolonToken, SyntaxKind.PlusPlusToken, SyntaxKind.PlusToken, SyntaxKind.PlusEqualsToken, SyntaxKind.MinusMinusToken, SyntaxKind.MinusToken, SyntaxKind.MinusEqualsToken, SyntaxKind.AsteriskToken, SyntaxKind.AsteriskEqualsToken, SyntaxKind.SlashToken, SyntaxKind.SlashEqualsToken, SyntaxKind.PercentToken, SyntaxKind.PercentEqualsToken, SyntaxKind.AmpersandToken, SyntaxKind.BarToken, SyntaxKind.CaretToken, SyntaxKind.TildeToken, SyntaxKind.ExclamationToken, SyntaxKind.ExclamationEqualsToken, SyntaxKind.GreaterThanToken, SyntaxKind.GreaterThanEqualsToken, SyntaxKind.LessThanToken, SyntaxKind.LessThanEqualsToken };
			HalsteadOperators.All = syntaxKindArray;
		}

		public HalsteadOperators()
		{
		}
	}
}