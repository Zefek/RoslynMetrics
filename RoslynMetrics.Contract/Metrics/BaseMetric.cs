using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynMetrics.Contracts.Metrics
{
    public abstract class BaseMetric
    {
        private List<MetricResult> _metricResults = new List<MetricResult>();
        public string Name { get; set; }
        public IEnumerable<MetricResult> Metrics { get { return _metricResults; } }
        public void AddMetricResult(MetricResult metricResult)
        {
            _metricResults.Add(metricResult);
        }
        public MetricResult GetMetricsByName(string name)
        {
            return Metrics.SingleOrDefault(k => k.Name.ToLower() == name.ToLower());
        }
    }
}
