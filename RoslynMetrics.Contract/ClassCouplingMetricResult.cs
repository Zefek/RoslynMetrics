using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
	public class ClassCouplingMetricResult : MetricResult
	{
		public IEnumerable<string> Types
		{
			get;
			set;
		}

		public ClassCouplingMetricResult()
		{
		}
	}
}