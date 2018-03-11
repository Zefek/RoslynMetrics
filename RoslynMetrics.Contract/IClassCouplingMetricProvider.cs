using System;
using System.Collections.Generic;

namespace RoslynMetrics.Contracts
{
	public interface IClassCouplingMetricProvider
	{
		IEnumerable<string> Calculate(MemberNode commonSyntaxNode);
	}
}