using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts.Metrics
{
	public class ModuleMetric : BaseMetric
	{
		public string AssemblyVersion
		{
			get;
			set;
		}

		public string FileVersion
		{
			get;
			set;
		}

		public NamespaceMetrics NamespaceMetrics
		{
			get;
			set;
		}

		public string ProjectFile
		{
			get;
			set;
		}

		public ModuleProjectKind ProjectKind
		{
			get;
			set;
		}
        public string ModuleName { get; set; }

        public ModuleMetric()
		{
		}
	}
}