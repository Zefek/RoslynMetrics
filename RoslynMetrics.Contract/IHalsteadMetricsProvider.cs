using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.Contracts
{
	public interface IHalsteadMetricsProvider
	{
		IHalsteadMetrics Calculate(MemberNode node);
	}
}