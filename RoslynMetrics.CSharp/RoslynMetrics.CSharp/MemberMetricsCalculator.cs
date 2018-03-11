using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using RoslynMetrics.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using RoslynMetrics.Contracts.Metrics;
using RoslynMetrics.Contracts.Calculators;

namespace RoslynMetrics.CSharp
{
    internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator, IMemberMetricsCalculator
    {
        List<Func<MemberNode, MetricResult>> memberNodeActions = new List<Func<MemberNode, MetricResult>>();
        List<Func<SyntaxNode, MetricResult>> syntaxNodeActions = new List<Func<SyntaxNode, MetricResult>>();

        public MemberMetricsCalculator(SemanticModel semanticModel) : base(semanticModel)
        {
            Initialize();
        }

        public MemberMetricsCalculator(ICalculatorContext context) : base(context)
        {
            Initialize();
        }

        private void Initialize()
        {
            memberNodeActions.Add(CalculateClassCoupling);
            memberNodeActions.Add(CalculateCyclomaticComplexity);
            memberNodeActions.Add(CalculateLinesOfCode);
            memberNodeActions.Add(CalculateLinesOfComment);
            syntaxNodeActions.Add(CalculateNumberOfLocalVariables);
            syntaxNodeActions.Add(CalculateNumberOfOverloads);
            syntaxNodeActions.Add(CalculateNumberOfParameters);
        }

        public MemberMetrics Calculate(TypeDeclarationSyntaxInfo typeNode)
        {
            SyntaxNode root = base.Context.Root;
            MemberCollectorSyntaxWalker memberCollectorSyntaxWalker = new MemberCollectorSyntaxWalker();
            IEnumerable<MemberNode> members = memberCollectorSyntaxWalker.GetMembers(base.Context.Model, typeNode);
            MemberMetrics memberMetric = new MemberMetrics()
            {
                Results = this.CalculateMemberMetrics(members).ToList<MemberMetric>().AsReadOnly()
            };
            return memberMetric;
        }

        private ClassCouplingMetricResult CalculateClassCoupling(MemberNode node)
        {
            List<string> metrics = (new MemberClassCouplingAnalyzer(base.Context.Model)).Calculate(node).ToList();
            return new ClassCouplingMetricResult
            {
                Name = MetricNames.ClassCoupling,
                Value = metrics.Count,
                Types = metrics
            };
        }

        private MetricResult CalculateCyclomaticComplexity(MemberNode node)
        {
            return new MetricResult
            {
                Name = MetricNames.CyclomaticComplexity,
                Value = (new CyclomaticComplexityAnalyzer()).Calculate(node)
            };
        }

        private MetricResult CalculateLinesOfCode(MemberNode node)
        {
            return new MetricResult
            {
                Name = MetricNames.LinesOfCode,
                Value = (new StatementsAnalyzer()).Calculate(node)
            };
        }

        private MetricResult CalculateLinesOfComment(MemberNode memberNode)
        {
            int num = 0;
            IEnumerable<SyntaxTrivia> list = memberNode.SyntaxNode.DescendantTrivia(null, false).Where<SyntaxTrivia>((SyntaxTrivia node) =>
            {
                return node.Kind() == SyntaxKind.MultiLineCommentTrivia || node.Kind() == SyntaxKind.SingleLineCommentTrivia;
            }).ToList<SyntaxTrivia>();
            foreach (SyntaxTrivia syntaxTrivium in list)
            {
                FileLinePositionSpan lineSpan = syntaxTrivium.GetLocation().GetLineSpan();
                LinePosition endLinePosition = lineSpan.EndLinePosition;
                int line = checked(endLinePosition.Line - lineSpan.StartLinePosition.Line);
                num = checked(num + checked(line + 1));
            }
            return new MetricResult
            {
                Name = MetricNames.LinesOfComment,
                Value = num
            };
        }

        private MetricResult CalculateMaintainablityIndex(IHalsteadMetrics halsteadMetrics, MemberMetric metric)
        {
            double linesOfCode = metric.GetMetricsByName(MetricNames.LinesOfCode).Value.Value;
            double cyclomatic = metric.GetMetricsByName(MetricNames.CyclomaticComplexity).Value.Value;
            double num1 = 1;
            if (linesOfCode == 0)
            {
                num1 = 100;
            }
            else
            {
                double? volume = halsteadMetrics.GetVolume();
                double num = 1;
                if (volume.HasValue)
                {
                    num = Math.Log(volume.Value);
                }
                num1 = (171 - 5.2 * num - 0.23 * cyclomatic - 16.2 * Math.Log(linesOfCode)) * 100 / 171;
            }
            return new MetricResult
            {
                Name = MetricNames.MaintainabilityIndex,
                Value = Math.Round(Math.Max(0, num1))
            };
        }
        private void RegisterActions()
        {
            Func<MemberNode, MetricResult> t = CalculateCyclomaticComplexity;
        }

        private IEnumerable<MemberMetric> CalculateMemberMetrics(IEnumerable<MemberNode> nodes)
        {
            foreach (MemberNode memberNode in nodes)
            {
                SyntaxNode syntaxNode = memberNode.SyntaxNode;
                MemberMetricKind memberMetricKind = MemberMetricsCalculator.GetMemberMetricKind(memberNode);
                IHalsteadMetrics halsteadMetric = (new HalsteadAnalyzer()).Calculate(memberNode);
                if (halsteadMetric == null)
                {
                    continue;
                }
                MemberMetric memberMetric = new MemberMetric()
                {
                    Name = memberNode.DisplayName,
                    CodeFile = memberNode.CodeFile,
                    LineNumber = memberNode.LineNumber,
                    Halstead = halsteadMetric,
                    Kind = memberMetricKind
                };
                MemberMetric memberMetric1 = memberMetric;
                foreach (Func<MemberNode, MetricResult> action in memberNodeActions)
                {
                    memberMetric.AddMetricResult(action(memberNode));
                }
                foreach (Func<SyntaxNode, MetricResult> action in syntaxNodeActions)
                {
                    memberMetric.AddMetricResult(action(syntaxNode));
                }
                memberMetric.AddMetricResult(CalculateMaintainablityIndex(halsteadMetric, memberMetric));
                //memberMetric.AddMetricResult(CalculateNumberOfDeliveredBugs(halsteadMetric));
                yield return memberMetric;
            }
        }
        private MetricResult CalculateNumberOfDeliveredBugs(IHalsteadMetrics halsteadMetrics)
        {
            return new MetricResult
            {
                Name = MetricNames.NumberOfDeliverdBugs,
                Value = halsteadMetrics.GetBugs()
            };
        }
        private MetricResult CalculateNumberOfLocalVariables(SyntaxNode node)
        {
            return new MetricResult
            {
                Name = MetricNames.NumberOfLocalVariables,
                Value = (new MethodLocalVariablesAnalyzer()).Calculate(node)
            };
        }

        private MetricResult CalculateNumberOfOverloads(SyntaxNode node)
        {
            return new MetricResult
            {
                Name = MetricNames.NumberOfOverloads,
                Value = (new MethodOverloadsAnalyzer(base.Context.Model)).Calculate(node)
            };
        }

        private MetricResult CalculateNumberOfParameters(SyntaxNode node)
        {
            return new MetricResult
            {
                Name = MetricNames.NumberOfParameters,
                Value = (new MethodParameterAnalyzer()).Calculate(node)
            };
        }

        private static MemberMetricKind GetMemberMetricKind(MemberNode memberNode)
        {
            switch (memberNode.Kind)
            {
                case MemberKind.Method:
                case MemberKind.Constructor:
                case MemberKind.Destructor:
                    {
                        return MemberMetricKind.Method;
                    }
                case MemberKind.GetProperty:
                case MemberKind.SetProperty:
                    {
                        return MemberMetricKind.PropertyAccessor;
                    }
                case MemberKind.AddEventHandler:
                case MemberKind.RemoveEventHandler:
                    {
                        return MemberMetricKind.EventAccessor;
                    }
            }
            return MemberMetricKind.Unknown;
        }
    }
}