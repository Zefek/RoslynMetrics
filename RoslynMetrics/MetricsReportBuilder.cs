using RoslynMetrics.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using VsCodeMetrics.Entities;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics
{
	internal class MetricsReportBuilder
	{
		public MetricsReportBuilder()
		{
		}

		public CodeMetricsReport Build(IEnumerable<ModuleMetric> metrics)
		{
			CodeMetricsReport codeMetricsReport = new CodeMetricsReport();
			codeMetricsReport.Version=11;
			CodeMetricsReport codeMetricsReport1 = codeMetricsReport;
			codeMetricsReport1.Add(MetricsReportBuilder.GetTargetsResult(metrics));
			return codeMetricsReport1;
		}

		private static IEnumerable<Member> GetMemberResults(MemberMetrics metrics)
		{
			foreach (MemberMetric memberMetric in metrics.Results)
			{
				Member member = new Member();
				member.Name=memberMetric.Name;
				Member member1 = member;
				string codeFile = memberMetric.CodeFile;
				if (codeFile == null)
				{
					codeFile = string.Empty;
				}
                member1.File = codeFile;
                member.Line = memberMetric.LineNumber;
                Member member2 = member;
				if (memberMetric.Kind == MemberMetricKind.Method)
				{
                    member2.Kind = "Method";
				}
				else if (memberMetric.Kind == MemberMetricKind.PropertyAccessor)
				{
                    member2.Kind = "Property";
				}
				else if (memberMetric.Kind ==  MemberMetricKind.EventAccessor)
				{
                    member2.Kind = "Event";
				}
				if (memberMetric.Metrics != null)
				{
					member2.Add(MetricsReportBuilder.GetMetricsResult(memberMetric.Metrics));
				}
				yield return member2;
			}
		}

		private static Members GetMembersResult(MemberMetrics metricsResults)
		{
			Members member = new Members();
			member.AddRange(MetricsReportBuilder.GetMemberResults(metricsResults));
			return member;
		}

        private static IEnumerable<Metric> GetMetricResults(IEnumerable<MetricResult> metrics)
        {
            foreach (MetricResult metricResult in metrics)
            {
                Metric metric = new Metric();
                metric.Name = metricResult.Name;
                metric.Value = metricResult.Value.ToString();
                yield return metric;
            }
        }

		private static Metrics GetMetricsResult(IEnumerable<MetricResult> metrics)
		{
			Metrics metric = new Metrics();
			metric.AddRange(MetricsReportBuilder.GetMetricResults(metrics));
			return metric;
		}

		private static VsCodeMetrics.Entities.Module GetModuleResult(ModuleMetric metric)
		{
			VsCodeMetrics.Entities.Module module = new VsCodeMetrics.Entities.Module();
			module.Name=metric.ModuleName;
			module.AssemblyVersion=metric.AssemblyVersion;
			module.FileVersion=metric.FileVersion;
			VsCodeMetrics.Entities.Module module1 = module;
			if (metric.Metrics != null)
			{
                module1.Add(MetricsReportBuilder.GetMetricsResult(metric.Metrics));
			}
			module1.Add(MetricsReportBuilder.GetNamespaces(metric.NamespaceMetrics));
			return module1;
		}

		private static Modules GetModulesResult(ModuleMetric metric)
		{
			Modules module = new Modules();
			module.Add(MetricsReportBuilder.GetModuleResult(metric));
			return module;
		}

		private static IEnumerable<Namespace> GetNamespaceResults(NamespaceMetrics metrics)
		{
			foreach (NamespaceMetric namespaceMetric in metrics.Results)
			{
				Namespace @namespace = new Namespace();
				@namespace.Name=namespaceMetric.Name;
				Namespace namespace1 = @namespace;
				if (namespaceMetric.Metrics != null)
				{
					namespace1.Add(MetricsReportBuilder.GetMetricsResult(namespaceMetric.Metrics));
				}
				namespace1.Add(MetricsReportBuilder.GetTypesResult(namespaceMetric.TypeMetrics));
				yield return namespace1;
			}
		}

		private static Namespaces GetNamespaces(NamespaceMetrics metrics)
		{
			Namespaces @namespace = new Namespaces();
			@namespace.AddRange(MetricsReportBuilder.GetNamespaceResults(metrics));
			return @namespace;
		}

		private static IEnumerable<Target> GetTargetResults(IEnumerable<ModuleMetric> metrics)
		{
			foreach (ModuleMetric metric in metrics)
			{
				Target target = new Target();
				target.Name=metric.ProjectFile;
				Target target1 = target;
				ModuleProjectKind projectKind = metric.ProjectKind;
				if (projectKind == ModuleProjectKind.CSharp)
				{
					target1.ProjectKind="cs";
				}
				else if (projectKind == ModuleProjectKind.VisualBasic)
				{
					target1.ProjectKind="vb";
				}
				target1.Add(MetricsReportBuilder.GetModulesResult(metric));
				yield return target1;
			}
		}

		private static Targets GetTargetsResult(IEnumerable<ModuleMetric> metrics)
		{
			Targets target = new Targets();
			target.AddRange(MetricsReportBuilder.GetTargetResults(metrics));
			return target;
		}

		private static IEnumerable<VsCodeMetrics.Entities.Type> GetTypeResults(TypeMetrics metrics)
		{
			foreach (TypeMetric typeMetric in metrics.Results)
			{
				VsCodeMetrics.Entities.Type type = new VsCodeMetrics.Entities.Type();
				type.Name=typeMetric.Name;
				VsCodeMetrics.Entities.Type type1 = type;
				if (typeMetric.Kind == TypeMetricKind.Class)
				{
					type1.Kind="Class";
				}
				else if (typeMetric.Kind == TypeMetricKind.Interface)
				{
					type1.Kind="Interface";
				}
				else if (typeMetric.Kind == TypeMetricKind.Struct)
				{
					type1.Kind="Struct";
				}
				else if (typeMetric.Kind == TypeMetricKind.ValueType)
				{
					type1.Kind="ValueType";
				}
				else if (typeMetric.Kind == TypeMetricKind.Delegate)
				{
					type1.Kind="Delegate";
				}
				if (typeMetric.Metrics != null)
				{
					type1.Add(MetricsReportBuilder.GetMetricsResult(typeMetric.Metrics));
				}
				type1.Add(MetricsReportBuilder.GetMembersResult(typeMetric.MemberMetrics));
				yield return type1;
			}
		}

		private static Types GetTypesResult(TypeMetrics metrics)
		{
			Types type = new Types();
			type.AddRange(MetricsReportBuilder.GetTypeResults(metrics));
			return type;
		}
	}
}