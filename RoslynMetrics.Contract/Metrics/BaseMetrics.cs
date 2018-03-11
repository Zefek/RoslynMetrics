using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynMetrics.Contracts.Metrics
{
    public class BaseMetrics<T> where T : BaseMetric
    {
        public IEnumerable<T> Results
        {
            get;
            set;
        }
    }
}
