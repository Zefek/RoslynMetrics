using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.Contracts.Calculators
{
    public abstract class MetricCalculator
    {
        public abstract string Name
        {
            get;
        }
        public abstract MetricResult Calculate<T, K>(T metrics) where T : Metrics.BaseMetrics<K> where K : Metrics.BaseMetric;
    }
    public class MaintainabilityIndexCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.MaintainabilityIndex;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            IEnumerable<double> list = metrics.Results.SelectMany<K, MetricResult>((K x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == Name).Select<MetricResult, double>((MetricResult x) => (double)x.Value).ToList<double>();
            if (!list.Any<double>())
            {
                result.Value = 100;
                return result;
            }
            result.Value = Math.Round(list.Sum() / (double)list.Count<double>());
            return result;
        }
        
    }
    public class CyclomaticComplexityCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.CyclomaticComplexity;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            result.Value = metrics.Results.SelectMany<K, MetricResult>((K x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == Name).Select<MetricResult, int>((MetricResult x) => (int)x.Value).Sum();
            return result;
        }
    }
    public class DepthOfInheritanceCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.DepthOfInheritance;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            IEnumerable<int> nums = metrics.Results.SelectMany<K, MetricResult>((K x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == Name).Select<MetricResult, int>((MetricResult x) => (int)x.Value);
            if (!nums.Any<int>())
            {
                result.Value = 0;
                return result;
            }
            result.Value = nums.Max();
            return result;
        }
    }
    public class LinesOfCodeCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.LinesOfCode;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            result.Value = metrics.Results.SelectMany<K, MetricResult>((K x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == Name).Select<MetricResult, int>((MetricResult x) => (int)x.Value).Sum();
            return result;
        }
    }
    public class NumberOfMethodsCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.NumberOfMethods;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            if (metrics is NamespaceMetrics)
            {
                result.Value = metrics.Results.Sum((K namespaceMetric) => (namespaceMetric as NamespaceMetric).TypeMetrics.Results.Sum<TypeMetric>((TypeMetric typeMetric) => typeMetric.MemberMetrics.Results.Count<MemberMetric>()));
            }
            else
            {
                result.Value = 0;
            }
            return result;
        }
    }
    public class NumberOfTypesCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.NumberOfTypes;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            if (metrics is NamespaceMetrics)
            {
                result.Value = metrics.Results.Sum<K>((K metric) => (metric as NamespaceMetric).TypeMetrics.Results.Count<TypeMetric>());
            }
            else if (metrics is TypeMetrics)
            {
                result.Value = (metrics as TypeMetrics).Results.Count<TypeMetric>();
            }
            else
            {
                result.Value = 0;
            }
            return result;
        }
    }
    public class ClassCouplingCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.ClassCoupling;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            ClassCouplingMetricResult result = new ClassCouplingMetricResult { Name = Name };
            List<string> types = (
                from x in metrics.Results.SelectMany<K, MetricResult>((K x) => x.Metrics).Where<MetricResult>((MetricResult x) => x.Name == Name).OfType<ClassCouplingMetricResult>().SelectMany<ClassCouplingMetricResult, string>((ClassCouplingMetricResult x) => x.Types).Distinct<string>()
                orderby x
                select x).ToList<string>();
            result.Value = types.Count;
            result.Types = types;
            return result;
        }
    }
    public class NumberOfNamespacesCalculator : MetricCalculator
    {
        public override string Name
        {
            get
            {
                return MetricNames.NumberOfNamespaces;
            }
        }
        public override MetricResult Calculate<T, K>(T metrics)
        {
            MetricResult result = new MetricResult { Name = Name };
            result.Value = metrics.Results.Count();
            return result;
        }
    }
}
