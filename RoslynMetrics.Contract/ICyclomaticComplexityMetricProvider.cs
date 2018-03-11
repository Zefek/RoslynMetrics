using System;

namespace RoslynMetrics.Contracts
{
	public interface ICyclomaticComplexityMetricProvider
	{
		int Calculate(MemberNode node);
	}
}