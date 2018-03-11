using Microsoft.CodeAnalysis;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using RoslynMetrics.Contracts.Metrics;
using RoslynMetrics.Contracts.Calculators;

namespace RoslynMetrics.CSharp
{
	internal sealed class NamespaceMetricsCalculator : SemanticModelMetricsCalculator
	{
        List<MetricCalculator> calculators = new List<MetricCalculator>();
		public NamespaceMetricsCalculator(SemanticModel semanticModel) : base(semanticModel)
		{
            Initialize();
		}

		public NamespaceMetricsCalculator(ICalculatorContext context) : base(context)
		{
            Initialize();
		}
        private void Initialize()
        {
            calculators.Add(new CyclomaticComplexityCalculator());
            calculators.Add(new MaintainabilityIndexCalculator());
            calculators.Add(new ClassCouplingCalculator());
            calculators.Add(new DepthOfInheritanceCalculator());
            calculators.Add(new LinesOfCodeCalculator());
            calculators.Add(new NumberOfMethodsCalculator());
            calculators.Add(new NumberOfTypesCalculator());
        }

		public NamespaceMetric CalculateFrom(NamespaceDeclarationSyntaxInfo namespaceNode, TypeMetrics typeMetrics)
		{
			NamespaceMetric namespaceMetric = new NamespaceMetric()
			{
				Name = namespaceNode.Name,
				TypeMetrics = typeMetrics
			};
            foreach (MetricCalculator calculator in calculators)
            {
                namespaceMetric.AddMetricResult(calculator.Calculate<TypeMetrics, TypeMetric>(typeMetrics));
            }			
			return namespaceMetric;
		}
	}
}