using System;

namespace RoslynMetrics.Contracts
{
	public interface IStatementCounterMetricProvider
	{
		int Calculate(MemberNode node);
	}
}