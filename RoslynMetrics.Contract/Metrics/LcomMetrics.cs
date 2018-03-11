using System;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts.Metrics
{
	public class LcomMetrics
	{
		public double Lcom
		{
			get;
			set;
		}

		public double LcomHendersonSellers
		{
			get;
			set;
		}

		public int NumFields
		{
			get;
			set;
		}

		public int NumMethods
		{
			get;
			set;
		}

		public LcomMetrics()
		{
		}
	}
}