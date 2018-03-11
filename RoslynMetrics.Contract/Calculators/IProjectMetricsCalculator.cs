using Microsoft.CodeAnalysis;
using System;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.Contracts.Calculators
{
	public interface IProjectMetricsCalculator
	{
		bool IgnoreGeneratedCode
		{
			get;
			set;
		}

		NamespaceMetrics Calculate(Project project);
	}
}