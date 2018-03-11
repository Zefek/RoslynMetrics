using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts.Metrics
{
	public class MemberMetric : BaseMetric
	{
		public string CodeFile
		{
			get;
			set;
		}

		public IHalsteadMetrics Halstead
		{
			get;
			set;
		}

		public MemberMetricKind Kind
		{
			get;
			set;
		}

		public int LineNumber
		{
			get;
			set;
		}
		public MemberMetric()
		{
		}
	}
}