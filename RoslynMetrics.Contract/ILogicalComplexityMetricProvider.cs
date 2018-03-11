using System;

namespace RoslynMetrics.Contracts
{
	public interface ILogicalComplexityMetricProvider
	{
		int Calculate(MemberNode node);
	}
}