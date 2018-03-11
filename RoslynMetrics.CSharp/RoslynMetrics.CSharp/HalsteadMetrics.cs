using RoslynMetrics.Contracts;
using System;
using System.Runtime.CompilerServices;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.CSharp
{
	internal sealed class HalsteadMetrics : IHalsteadMetrics
	{
		public readonly static IHalsteadMetrics GenericInstanceSetPropertyMetrics;

		public readonly static IHalsteadMetrics GenericStaticSetPropertyMetrics;

		public readonly static IHalsteadMetrics GenericInstanceGetPropertyMetrics;

		public readonly static IHalsteadMetrics GenericStaticGetPropertyMetrics;

		public int NumOperands
		{
			get
			{
				return JustDecompileGenerated_get_NumOperands();
			}
			set
			{
				JustDecompileGenerated_set_NumOperands(value);
			}
		}

		private int JustDecompileGenerated_NumOperands_k__BackingField;

		public int JustDecompileGenerated_get_NumOperands()
		{
			return this.JustDecompileGenerated_NumOperands_k__BackingField;
		}

		internal void JustDecompileGenerated_set_NumOperands(int value)
		{
			this.JustDecompileGenerated_NumOperands_k__BackingField = value;
		}

		public int NumOperators
		{
			get
			{
				return JustDecompileGenerated_get_NumOperators();
			}
			set
			{
				JustDecompileGenerated_set_NumOperators(value);
			}
		}

		private int JustDecompileGenerated_NumOperators_k__BackingField;

		public int JustDecompileGenerated_get_NumOperators()
		{
			return this.JustDecompileGenerated_NumOperators_k__BackingField;
		}

		internal void JustDecompileGenerated_set_NumOperators(int value)
		{
			this.JustDecompileGenerated_NumOperators_k__BackingField = value;
		}

		public int NumUniqueOperands
		{
			get
			{
				return JustDecompileGenerated_get_NumUniqueOperands();
			}
			set
			{
				JustDecompileGenerated_set_NumUniqueOperands(value);
			}
		}

		private int JustDecompileGenerated_NumUniqueOperands_k__BackingField;

		public int JustDecompileGenerated_get_NumUniqueOperands()
		{
			return this.JustDecompileGenerated_NumUniqueOperands_k__BackingField;
		}

		internal void JustDecompileGenerated_set_NumUniqueOperands(int value)
		{
			this.JustDecompileGenerated_NumUniqueOperands_k__BackingField = value;
		}

		public int NumUniqueOperators
		{
			get
			{
				return JustDecompileGenerated_get_NumUniqueOperators();
			}
			set
			{
				JustDecompileGenerated_set_NumUniqueOperators(value);
			}
		}

		private int JustDecompileGenerated_NumUniqueOperators_k__BackingField;

		public int JustDecompileGenerated_get_NumUniqueOperators()
		{
			return this.JustDecompileGenerated_NumUniqueOperators_k__BackingField;
		}

		internal void JustDecompileGenerated_set_NumUniqueOperators(int value)
		{
			this.JustDecompileGenerated_NumUniqueOperators_k__BackingField = value;
		}

		static HalsteadMetrics()
		{
			HalsteadMetrics halsteadMetric = new HalsteadMetrics()
			{
				NumOperands = 5,
				NumOperators = 3,
				NumUniqueOperands = 4,
				NumUniqueOperators = 3
			};
			HalsteadMetrics.GenericInstanceSetPropertyMetrics = halsteadMetric;
			HalsteadMetrics halsteadMetric1 = new HalsteadMetrics()
			{
				NumOperands = 4,
				NumOperators = 3,
				NumUniqueOperands = 3,
				NumUniqueOperators = 3
			};
			HalsteadMetrics.GenericStaticSetPropertyMetrics = halsteadMetric1;
			HalsteadMetrics halsteadMetric2 = new HalsteadMetrics()
			{
				NumOperands = 3,
				NumOperators = 2,
				NumUniqueOperands = 3,
				NumUniqueOperators = 2
			};
			HalsteadMetrics.GenericInstanceGetPropertyMetrics = halsteadMetric2;
			HalsteadMetrics halsteadMetric3 = new HalsteadMetrics()
			{
				NumOperands = 2,
				NumOperators = 1,
				NumUniqueOperands = 2,
				NumUniqueOperators = 1
			};
			HalsteadMetrics.GenericStaticGetPropertyMetrics = halsteadMetric3;
		}

		public HalsteadMetrics()
		{
		}

		public double? GetBugs()
		{
			double? effort = this.GetEffort();
			if (!effort.HasValue)
			{
				return null;
			}
            return new double?((double)effort.GetValueOrDefault() / 3000);
		}

		public double GetDifficulty()
		{
			return (double)this.NumUniqueOperators / 2 * ((double)this.NumOperands / (double)this.NumUniqueOperands);
		}

		public double? GetEffort()
		{
			double difficulty = this.GetDifficulty();
			double? volume = this.GetVolume();
			if (!volume.HasValue)
			{
				return null;
			}
			double num = difficulty;
			double? nullable = volume;
			if (!nullable.HasValue)
			{
				return null;
			}
			return new double?(num * (double)nullable.GetValueOrDefault());
		}

		public int GetLength()
		{
			return checked(this.NumOperators + this.NumOperands);
		}

		public int GetVocabulary()
		{
			return checked(this.NumUniqueOperators + this.NumUniqueOperands);
		}

		public double? GetVolume()
		{
			double num = 2;
			double vocabulary = (double)this.GetVocabulary();
			double length = (double)this.GetLength();
			if (vocabulary == 0)
			{
				return null;
			}
			return new double?(length * Math.Log(vocabulary, num));
		}
	}
}