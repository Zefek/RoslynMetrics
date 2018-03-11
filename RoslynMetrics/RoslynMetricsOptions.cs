using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics
{
	internal sealed class RoslynMetricsOptions
	{
		private const string SwitchToken = "/";

		private readonly IDictionary<string, string> options;

		public string File
		{
			get
			{
				return this.GetOption("file");
			}
		}

		public bool IgnoreGeneratedCode
		{
			get
			{
				return this.IsOptionSpecified("igc");
			}
		}

		public string Output
		{
			get
			{
				return this.GetOption("out");
			}
		}

		public RoslynMetricsOptions(IEnumerable<string> args)
		{
			this.options = RoslynMetricsOptions.ParseOptions(args);
		}

		private string GetOption(string name)
		{
			string str;
			name = string.Concat("/", name);
			this.options.TryGetValue(name, out str);
			return str;
		}

		private bool IsOptionSpecified(string name)
		{
			name = string.Concat("/", name);
			return this.options.ContainsKey(name);
		}

		private static IDictionary<string, string> ParseOptions(IEnumerable<string> args)
		{
			return args.Select((string x) => {
				IEnumerable<string> strs = x.Split(new char[] { ':' }, 2);
				return new { Switch = strs.FirstOrDefault<string>(), Value = strs.Skip<string>(1).FirstOrDefault<string>() };
			}).Where((x) => x.Switch.StartsWith("/")).ToDictionary((k) => k.Switch, (v) => v.Value);
		}
	}
}