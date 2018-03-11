using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using RoslynMetrics.CSharp.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RoslynMetrics.Contracts.Metrics;
using RoslynMetrics.Contracts.Calculators;

namespace RoslynMetrics.CSharp
{
	internal sealed class TypeMetricsCalculator : SemanticModelMetricsCalculator
	{
        List<MetricCalculator> calculators = new List<MetricCalculator>();
		public TypeMetricsCalculator(SemanticModel semanticModel) : base(semanticModel)
		{
            Initialize();
		}

		public TypeMetricsCalculator(ICalculatorContext context) : base(context)
		{
            Initialize();
		}

        private void Initialize()
        {
            calculators.Add(new MaintainabilityIndexCalculator());
            calculators.Add(new ClassCouplingCalculator());
            calculators.Add(new CyclomaticComplexityCalculator());
            calculators.Add(new DepthOfInheritanceCalculator());
            calculators.Add(new LinesOfCodeCalculator());
        }

		public TypeMetric CalculateFrom(TypeDeclarationSyntaxInfo typeNode, MemberMetrics memberMetrics)
		{
			TypeDeclarationSyntax syntax = (TypeDeclarationSyntax)typeNode.Syntax;
			TypeMetricKind metricKind = TypeMetricsCalculator.GetMetricKind(syntax);
			LcomMetrics lcomMetric = this.CalculateLackOfCohesion(syntax);
			TypeMetric typeMetric = new TypeMetric()
			{
				Name = TypeMetricsCalculator.GetDisplayName(syntax),
				Kind = metricKind,
				MemberMetrics = memberMetrics
			};
			List<MetricResult> metricResults = new List<MetricResult>();
            foreach (MetricCalculator calculator in calculators)
            {
                typeMetric.AddMetricResult(calculator.Calculate<MemberMetrics, MemberMetric>(memberMetrics));
            }
            typeMetric.AddMetricResult(new MetricResult()
            {
                Name = MetricNames.LackOfCohesionOfMethods,
                Value = lcomMetric.Lcom
            });
            typeMetric.AddMetricResult(new MetricResult()
            {
                Name = MetricNames.LackOfCohesionOfMethodsHendersonSellers,
                Value = lcomMetric.LcomHendersonSellers
            });
            typeMetric.AddMetricResult(new MetricResult()
            {
                Name = MetricNames.NumberOfFields,
                Value = lcomMetric.NumFields
            });
            typeMetric.AddMetricResult(new MetricResult()
            {
                Name = MetricNames.NumberOfMethods,
                Value = lcomMetric.NumMethods
            });
			return typeMetric;
		}

		private LcomMetrics CalculateLackOfCohesion(TypeDeclarationSyntax type)
		{
			return (new LackOfCohesionAnalyzer(base.Context.Model)).Calculate(type);
		}

		private static string GetDisplayName(TypeDeclarationSyntax type)
		{
			return TypeNameResolver.GetName(type);
		}

		private static TypeMetricKind GetMetricKind(TypeDeclarationSyntax type)
		{
			bool flag = type is InterfaceDeclarationSyntax;
			bool flag1 = type is ClassDeclarationSyntax;
			bool flag2 = type is StructDeclarationSyntax;
			if (flag1)
			{
				return TypeMetricKind.Class;
			}
			if (flag)
			{
				return TypeMetricKind.Interface;
			}
			if (flag2)
			{
				return TypeMetricKind.Struct;
			}
			return TypeMetricKind.Unknown;
		}
	}
}