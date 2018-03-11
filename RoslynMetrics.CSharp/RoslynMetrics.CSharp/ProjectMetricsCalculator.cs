using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RoslynMetrics.Contracts.Calculators;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.CSharp
{
	public sealed class ProjectMetricsCalculator : IProjectMetricsCalculator
	{
		public bool IgnoreGeneratedCode
		{
			get;
			set;
		}

		public ProjectMetricsCalculator()
		{
		}

		public NamespaceMetrics Calculate(Project project)
		{
			Compilation compilation = project.GetCompilation();
			IEnumerable<KeyValuePair<string, NamespaceDeclaration>> namespaceDeclarations = GetNamespaceDeclarations(project, this.IgnoreGeneratedCode);
			List<NamespaceMetric> namespaceMetrics = new List<NamespaceMetric>();
			foreach (KeyValuePair<string, NamespaceDeclaration> namespaceDeclaration in namespaceDeclarations)
			{
				TypeMetrics typeMetric = CalculateTypeMetrics(compilation, namespaceDeclaration.Value);
				NamespaceMetric namespaceMetric = CalculateNamespaceMetrics(compilation, namespaceDeclaration.Value, typeMetric);
				namespaceMetrics.Add(namespaceMetric);
			}
			return new NamespaceMetrics()
			{
				Results = namespaceMetrics.AsReadOnly()
			};
		}

		private MemberMetrics CalculateMemberMetrics(Compilation compilation, TypeDeclaration typeNodes)
		{
			List<MemberMetric> memberMetrics = new List<MemberMetric>();
			Parallel.ForEach<TypeDeclarationSyntaxInfo>(typeNodes.SyntaxNodes, (TypeDeclarationSyntaxInfo typeNode) => {
				MemberMetrics memberMetric = (new MemberMetricsCalculator(compilation.GetSemanticModel(typeNode.Syntax.SyntaxTree))).Calculate(typeNode);
				memberMetrics.AddRange(memberMetric.Results);
			});
			return new MemberMetrics()
			{
				Results = memberMetrics.AsReadOnly()
			};
		}

		private NamespaceMetric CalculateNamespaceMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes, TypeMetrics typeMetrics)
		{
			if (!namespaceNodes.SyntaxNodes.Any<NamespaceDeclarationSyntaxInfo>())
			{
				return null;
			}
			NamespaceDeclarationSyntaxInfo namespaceDeclarationSyntaxInfo = namespaceNodes.SyntaxNodes.FirstOrDefault<NamespaceDeclarationSyntaxInfo>();
			SemanticModel semanticModel = compilation.GetSemanticModel(namespaceDeclarationSyntaxInfo.Syntax.SyntaxTree);
			return (new NamespaceMetricsCalculator(semanticModel)).CalculateFrom(namespaceDeclarationSyntaxInfo, typeMetrics);
		}

		private TypeMetrics CalculateTypeMetrics(Compilation compilation, NamespaceDeclaration namespaceNodes)
		{
			List<TypeMetric> typeMetrics = new List<TypeMetric>();
			foreach (KeyValuePair<string, TypeDeclaration> typeDeclaration in GetTypeDeclarations(namespaceNodes))
			{
				TypeDeclaration value = typeDeclaration.Value;
				MemberMetrics memberMetric = CalculateMemberMetrics(compilation, value);
				typeMetrics.Add(CalculateTypeMetrics(compilation, value, memberMetric));
			}
			TypeMetrics typeMetric = new TypeMetrics()
			{
				Results = typeMetrics.ToList<TypeMetric>().AsReadOnly()
			};
			return typeMetric;
		}

		private TypeMetric CalculateTypeMetrics(Compilation compilation, TypeDeclaration typeNodes, MemberMetrics memberMetrics)
		{
			if (!typeNodes.SyntaxNodes.Any<TypeDeclarationSyntaxInfo>())
			{
				return null;
			}
			TypeDeclarationSyntaxInfo typeDeclarationSyntaxInfo = typeNodes.SyntaxNodes.FirstOrDefault<TypeDeclarationSyntaxInfo>();
			SemanticModel semanticModel = compilation.GetSemanticModel(typeDeclarationSyntaxInfo.Syntax.SyntaxTree);
			return (new TypeMetricsCalculator(semanticModel)).CalculateFrom(typeDeclarationSyntaxInfo, memberMetrics);
		}

		private IEnumerable<KeyValuePair<string, NamespaceDeclaration>> GetNamespaceDeclarations(Project project, bool ignoreGeneratedCode = false)
		{
			List<NamespaceDeclarationSyntaxInfo> namespaceDeclarationSyntaxInfos = new List<NamespaceDeclarationSyntaxInfo>();
			List<string> strs = new List<string>()
			{
				".*\\.g\\.cs$",
				".*\\.xaml\\.cs$",
				".*\\.designer\\.cs$"
			};
			foreach (Document document in project.Documents)
			{
				string filePath = document.FilePath;
				if (ignoreGeneratedCode && document.IsGeneratedCodeFile(strs))
				{
					continue;
				}
				INamespaceCollector namespaceCollectorSyntaxWalker = new NamespaceCollectorSyntaxWalker();
				SyntaxNode syntaxRoot = document.GetSyntaxRoot();
				List<NamespaceDeclarationSyntaxInfo> list = (
					from x in namespaceCollectorSyntaxWalker.GetNamespaces<NamespaceDeclarationSyntax>(syntaxRoot)
					select new NamespaceDeclarationSyntaxInfo()
					{
						Name = x.GetName(x.SyntaxTree.GetRoot(new CancellationToken())),
						CodeFile = filePath,
						Syntax = x
					}).ToList<NamespaceDeclarationSyntaxInfo>();
				namespaceDeclarationSyntaxInfos.AddRange(list);
			}
			IEnumerable<IGrouping<string, NamespaceDeclarationSyntaxInfo>> name = 
				from x in namespaceDeclarationSyntaxInfos
				group x by x.Name;
			return name.ToDictionary<IGrouping<string, NamespaceDeclarationSyntaxInfo>, string, NamespaceDeclaration>((IGrouping<string, NamespaceDeclarationSyntaxInfo> x) => x.Key, (IGrouping<string, NamespaceDeclarationSyntaxInfo> y) => new NamespaceDeclaration()
			{
				Name = y.Key,
				SyntaxNodes = y.ToList<NamespaceDeclarationSyntaxInfo>().AsReadOnly().AsEnumerable<NamespaceDeclarationSyntaxInfo>()
			});
		}

		private IEnumerable<KeyValuePair<string, TypeDeclaration>> GetTypeDeclarations(NamespaceDeclaration namespaceDeclaration)
		{
			List<TypeDeclarationSyntaxInfo> typeDeclarationSyntaxInfos = new List<TypeDeclarationSyntaxInfo>();
			foreach (NamespaceDeclarationSyntaxInfo syntaxNode in namespaceDeclaration.SyntaxNodes)
			{
				IEnumerable<TypeDeclarationSyntaxInfo> list = (
					from x in (new TypeCollectorSyntaxWalker()).GetTypes<TypeDeclarationSyntax>(syntaxNode.Syntax)
					select new TypeDeclarationSyntaxInfo()
					{
						Name = x.GetName(x.SyntaxTree.GetRoot(new CancellationToken())),
						CodeFile = syntaxNode.CodeFile,
						Syntax = x
					}).ToList<TypeDeclarationSyntaxInfo>();
				typeDeclarationSyntaxInfos.AddRange(list);
			}
			IEnumerable<IGrouping<string, TypeDeclarationSyntaxInfo>> name = 
				from x in typeDeclarationSyntaxInfos
				group x by x.Name;
			return name.ToDictionary<IGrouping<string, TypeDeclarationSyntaxInfo>, string, TypeDeclaration>((IGrouping<string, TypeDeclarationSyntaxInfo> x) => x.Key, (IGrouping<string, TypeDeclarationSyntaxInfo> y) => new TypeDeclaration()
			{
				Name = y.Key,
				SyntaxNodes = y.ToList<TypeDeclarationSyntaxInfo>().AsReadOnly().AsEnumerable<TypeDeclarationSyntaxInfo>()
			});
		}
	}
}