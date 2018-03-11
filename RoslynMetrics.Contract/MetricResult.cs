using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
    public class MetricResult
    {
        public string Name
        {
            get; set;
        }

        public double? Value
        {
            get;
            set;
        }

        public string ValueText
        {
            get
            {
                if (this.Value == null)
                {
                    return string.Empty;
                }
                return this.Value.ToString();
            }
        }

        public MetricResult()
        {
        }
    }
}