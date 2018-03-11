using System;

namespace RoslynMetrics.Contracts
{
	public enum MemberMetricKind
	{
		Unknown,
		Method,
		PropertyAccessor,
		EventAccessor
	}
}