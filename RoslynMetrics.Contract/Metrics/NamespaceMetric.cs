using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts.Metrics
{
	public class NamespaceMetric: BaseMetric
	{

		public TypeMetrics TypeMetrics
		{
			get;
			set;
		}

		public NamespaceMetric()
		{
		}
	}
}