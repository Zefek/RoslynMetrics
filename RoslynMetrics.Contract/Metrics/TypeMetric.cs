using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts.Metrics
{
	public class TypeMetric : BaseMetric
	{
		public TypeMetricKind Kind
		{
			get;
			set;
		}

		public MemberMetrics MemberMetrics
		{
			get;
			set;
		}

		public TypeMetric()
		{
		}
	}
}