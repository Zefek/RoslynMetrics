using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using RoslynMetrics.Contracts;
using RoslynMetrics.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using RoslynMetrics.Contracts.Metrics;
using RoslynMetrics.Contracts.Calculators;

namespace RoslynMetrics
{
	internal sealed class ModuleMetricsCalculator
	{
		public bool IgnoreGeneratedCode
		{
			get;
			set;
		}

		public ModuleMetricsCalculator()
		{
		}

        public ModuleMetric Calculate(Project project)
        {
            Compilation compilation = ProjectExtensions.GetCompilation(project);
            IModuleSymbol moduleSymbol = compilation.Assembly.Modules.FirstOrDefault<IModuleSymbol>();
            NamespaceMetrics namespaceMetric = ModuleMetricsCalculator.CalculateNamespaceMetrics(project, this.IgnoreGeneratedCode);
            NamespaceMetrics namespaceMetric1 = namespaceMetric;
            if (namespaceMetric == null)
            {
                return null;
            }
            IEnumerable<string> strs = ModuleMetricsCalculator.CalculateClassCoupling(namespaceMetric1);
            double num = ModuleMetricsCalculator.CalculateMaintainabilityIndex(namespaceMetric1);
            int num1 = ModuleMetricsCalculator.CalculateCyclomaticComplexity(namespaceMetric1);
            int num2 = this.CalculateDepthOfInheritance(namespaceMetric1);
            int num3 = ModuleMetricsCalculator.CalculateLinesOfCode(namespaceMetric1);
            int num4 = namespaceMetric1.Results.Count<NamespaceMetric>();
            int num5 = ModuleMetricsCalculator.CalculateNumberOfTypes(namespaceMetric1);
            int num6 = ModuleMetricsCalculator.CalculateNumberOfMethods(namespaceMetric1);
            ModuleMetric moduleMetric = new ModuleMetric();
            moduleMetric.ModuleName = moduleSymbol.Name;
            moduleMetric.ProjectFile = project.FilePath ?? string.Empty;
            moduleMetric.AssemblyVersion = string.Empty;
            moduleMetric.FileVersion = string.Empty;
            moduleMetric.NamespaceMetrics = namespaceMetric1;
            List<MetricResult> metricResults = new List<MetricResult>();
            moduleMetric.AddMetricResult(new MetricResult()
            {
                Name = "MaintainabilityIndex",
                Value = num
            });

            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "CyclomaticComplexity",
                Value = num1
            });
            moduleMetric.AddMetricResult(new ClassCouplingMetricResult
            {
                Name = "ClassCoupling",
                Value = strs.Count<string>(),
                Types = strs
            });
            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "DepthOfInheritance",
                Value = num2
            });
            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "LinesOfCode",
                Value = num3
            });
            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "NumberOfNamespaces",
                Value = num4
            });
            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "NumberOfTypes",
                Value = num5
            });
            moduleMetric.AddMetricResult(new MetricResult
            {
                Name = "NumberOfMethods",
                Value = num6
            });
			return moduleMetric;
		}

		private static IEnumerable<string> CalculateClassCoupling(NamespaceMetrics metrics)
		{
			return (
				from x in metrics.Results.SelectMany<NamespaceMetric, MetricResult>((NamespaceMetric x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == "ClassCoupling").OfType<ClassCouplingMetricResult>().SelectMany<ClassCouplingMetricResult, string>((ClassCouplingMetricResult x) => x.Types).Distinct<string>()
				orderby x
				select x).ToList<string>();
		}

		private static int CalculateCyclomaticComplexity(NamespaceMetrics metrics)
		{
			return metrics.Results.SelectMany<NamespaceMetric, MetricResult>((NamespaceMetric x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == "CyclomaticComplexity").Select<MetricResult, int>((MetricResult x) => (int)x.Value).Sum();
		}

		private int CalculateDepthOfInheritance(NamespaceMetrics metrics)
		{
			IEnumerable<int> nums = metrics.Results.SelectMany<NamespaceMetric, MetricResult>((NamespaceMetric x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == "DepthOfInheritance").Select<MetricResult, int>((MetricResult x) => (int)x.Value);
			if (!nums.Any<int>())
			{
				return 0;
			}
			return nums.Max();
		}

		private static int CalculateLinesOfCode(NamespaceMetrics metrics)
		{
			return metrics.Results.SelectMany<NamespaceMetric, MetricResult>((NamespaceMetric x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == "LinesOfCode").Select<MetricResult, int>((MetricResult x) => (int)x.Value).Sum();
		}

		private static double CalculateMaintainabilityIndex(NamespaceMetrics metrics)
		{
			IEnumerable<double> list = metrics.Results.SelectMany<NamespaceMetric, MetricResult>((NamespaceMetric x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == "MaintainabilityIndex").Select<MetricResult, double>((MetricResult x) => (double)x.Value).ToList<double>();
			if (!list.Any<double>())
			{
				return 100;
			}
			return Math.Round(list.Sum() / (double)list.Count<double>());
		}

		private static NamespaceMetrics CalculateNamespaceMetrics(Project project, bool ignoreGeneratedCode = false)
		{
			IProjectMetricsCalculator projectMetricsCalculator = ModuleMetricsCalculator.GetProjectMetricsCalculator(project);
			IProjectMetricsCalculator projectMetricsCalculator1 = projectMetricsCalculator;
			if (projectMetricsCalculator == null)
			{
				return null;
			}
            projectMetricsCalculator1.IgnoreGeneratedCode = ignoreGeneratedCode;
			return projectMetricsCalculator1.Calculate(project);
		}

		private static int CalculateNumberOfMethods(NamespaceMetrics namespaceMetrics)
		{
			return namespaceMetrics.Results.Sum<NamespaceMetric>((NamespaceMetric namespaceMetric) => namespaceMetric.TypeMetrics.Results.Sum<TypeMetric>((TypeMetric typeMetric) => typeMetric.MemberMetrics.Results.Count<MemberMetric>()));
		}

		private static int CalculateNumberOfTypes(NamespaceMetrics namespaceMetrics)
		{
			return namespaceMetrics.Results.Sum<NamespaceMetric>((NamespaceMetric metric) => metric.TypeMetrics.Results.Count<TypeMetric>());
		}
        /*
		private static Assembly GetAssemblyFrom(Compilation compilation)
		{
			Assembly assembly;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				CancellationToken cancellationToken = new CancellationToken();
				if (!compilation.Emit(memoryStream, null, null, null, null, cancellationToken, null, null).get_Success())
				{
					return null;
				}
				else
				{
					assembly = Assembly.ReflectionOnlyLoad(memoryStream.ToArray());
				}
			}
			return assembly;
		}
        */
		private static IProjectMetricsCalculator GetProjectMetricsCalculator(Project project)
		{
			if (project.Language == "C#")
			{
				return new ProjectMetricsCalculator();
			}
			return null;
		}
	}
}