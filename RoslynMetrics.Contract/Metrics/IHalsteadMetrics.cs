using System;

namespace RoslynMetrics.Contracts.Metrics
{
	public interface IHalsteadMetrics
	{
		int NumOperands
		{
			get;
		}

		int NumOperators
		{
			get;
		}

		int NumUniqueOperands
		{
			get;
		}

		int NumUniqueOperators
		{
			get;
		}

		double? GetBugs();

		double GetDifficulty();

		double? GetEffort();

		int GetLength();

		int GetVocabulary();

		double? GetVolume();
	}
}