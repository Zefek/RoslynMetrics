using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis
{
	public static class ProjectExtensions
	{
		public static Compilation GetCompilation(this Project project)
		{
			Task<Compilation> compilationAsync = project.GetCompilationAsync(new CancellationToken());
			compilationAsync.Wait();
			return compilationAsync.Result;
		}
	}
}