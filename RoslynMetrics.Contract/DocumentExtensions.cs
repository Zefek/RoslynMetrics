using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RoslynMetrics.Contracts
{
	public static class DocumentExtensions
	{
		public static SyntaxNode GetSyntaxRoot(this Document document)
		{
			Task<SyntaxNode> syntaxRootAsync = document.GetSyntaxRootAsync(new CancellationToken());
			syntaxRootAsync.Wait();
			return syntaxRootAsync.Result;
		}

		public static bool IsGeneratedCodeFile(this Document doc, IEnumerable<string> pattern)
		{
			string filePath = doc.FilePath;
			return pattern.Any<string>((string x) => Regex.IsMatch(filePath, x, RegexOptions.IgnoreCase | RegexOptions.Singleline));
		}
	}
}