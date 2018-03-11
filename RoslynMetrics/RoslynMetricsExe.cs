using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VsCodeMetrics.Entities;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics
{
	public class RoslynMetricsExe
	{
		public const int ReturnCodeRoslynBinariesMissing = 4098;

		public RoslynMetricsExe()
		{
		}

		private static void BuildReport(IEnumerable<Project> projects, string reportFile, bool ignoreGeneratedCode = false)
		{
			IEnumerable<ModuleMetric> list = RoslynMetricsExe.CalculateModuleMetrics(projects, ignoreGeneratedCode).ToList<ModuleMetric>();
			RoslynMetricsExe.BuildReportFrom(list, reportFile);
		}

		private static void BuildReportFrom(IEnumerable<ModuleMetric> metrics, string reportFile)
		{
			CodeMetricsReport codeMetricsReport = (new MetricsReportBuilder()).Build(metrics);
			using (Stream stream = File.Open(reportFile, FileMode.OpenOrCreate, FileAccess.Write))
			{
				stream.SetLength((long)0);
				codeMetricsReport.WriteTo(stream);
				stream.Flush();
			}
			File.Create(RoslynMetricsExe.GetSuccessFileName(reportFile));
		}

		private static void BuildReportFromSolution(string solutionFile, string reportFile, bool ignoreGeneratedCode = false)
		{
			using (MSBuildWorkspace mSBuildWorkspace = MSBuildWorkspace.Create())
			{
				mSBuildWorkspace.LoadMetadataForReferencedProjects=true;
                mSBuildWorkspace.WorkspaceFailed += MSBuildWorkspace_WorkspaceFailed;
				Task<Solution> task = mSBuildWorkspace.OpenSolutionAsync(solutionFile, new CancellationToken());
				task.Wait();
				IEnumerable<Project> list = task.Result.Projects.ToList<Project>();
				RoslynMetricsExe.BuildReport(list, reportFile, ignoreGeneratedCode);
			}
		}

        private static void MSBuildWorkspace_WorkspaceFailed(object sender, WorkspaceDiagnosticEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void BuildReportFromStandAloneProject(string file, string reportFile, bool ignoreGeneratedCode = false)
		{
			using (MSBuildWorkspace mSBuildWorkspace = MSBuildWorkspace.Create())
			{
				Task<Project> task = mSBuildWorkspace.OpenProjectAsync(file, new CancellationToken());
				task.Wait();
				Project[] result = new Project[] { task.Result };
				RoslynMetricsExe.BuildReport(result, reportFile, ignoreGeneratedCode);
			}
		}

		private static IEnumerable<ModuleMetric> CalculateModuleMetrics(IEnumerable<Project> projects, bool ignoreGeneratedCode = false)
		{
			BlockingCollection<ModuleMetric> moduleMetrics = new BlockingCollection<ModuleMetric>();
            foreach (Project nextProject in projects)
            {
                ModuleMetric moduleMetric = (new ModuleMetricsCalculator()
                {
                    IgnoreGeneratedCode = ignoreGeneratedCode
                }).Calculate(nextProject);
                if (moduleMetric != null)
                {
                    moduleMetrics.Add(moduleMetric);
                }
            }
            /*
			Parallel.ForEach<Project>(projects, (Project nextProject) => {
				ModuleMetric moduleMetric = (new ModuleMetricsCalculator()
				{
					IgnoreGeneratedCode = ignoreGeneratedCode
				}).Calculate(nextProject);
				if (moduleMetric != null)
				{
					moduleMetrics.Add(moduleMetric);
				}
			});
            */
			return moduleMetrics.ToList<ModuleMetric>().AsReadOnly();
		}

		private static string GetSuccessFileName(string reportFile)
		{
			FileInfo fileInfo = new FileInfo(reportFile);
			return Path.Combine(Path.GetTempPath(), fileInfo.Name.Replace(fileInfo.Extension, ".lastcodemetricssucceeded"));
		}

		private static int Main(string[] args)
		{
			int num;
			try
			{
				RoslynMetricsOptions roslynMetricsOption = new RoslynMetricsOptions(args);
				if (!string.IsNullOrWhiteSpace(roslynMetricsOption.File) && !string.IsNullOrWhiteSpace(roslynMetricsOption.Output))
				{
					FileInfo fileInfo = new FileInfo(roslynMetricsOption.File);
					if (fileInfo.Exists)
					{
						string lower = fileInfo.Extension.ToLower();
						string str = lower;
						if (lower != null)
						{
							if (str == ".sln")
							{
								RoslynMetricsExe.BuildReportFromSolution(roslynMetricsOption.File, roslynMetricsOption.Output, roslynMetricsOption.IgnoreGeneratedCode);
							}
							else if (str == ".csproj")
							{
								RoslynMetricsExe.BuildReportFromStandAloneProject(roslynMetricsOption.File, roslynMetricsOption.Output, false);
							}
						}
					}
				}
				num = 0;
			}
			catch (FileNotFoundException fileNotFoundException)
			{
				Console.Error.Write(fileNotFoundException.Message);
				num = 4098;
			}
			catch (Exception exception)
			{
				Console.Error.Write(exception.Message);
				num = 1;
			}
			return num;
		}
	}
}