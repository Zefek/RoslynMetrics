using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class MemberCollectorSyntaxWalker : CSharpSyntaxWalker
	{
		private readonly IList<MemberNode> members = new List<MemberNode>();

		public MemberCollectorSyntaxWalker() : base(0)
		{
		}

        private void AddAccessorNode(SyntaxNode node, AccessorListSyntax accessorList, SyntaxKind filter, MemberKind kind)
        {
            if (accessorList == null)
                return;
            if (accessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == filter) != null)
            {
                MemberNode memberNode = new MemberNode()
                {
                    SyntaxNode = node,
                    Kind = kind
                };
                this.members.Add(memberNode);
            }
        }

		public IEnumerable<MemberNode> GetMembers(SemanticModel semanticModel, TypeDeclarationSyntaxInfo type)
		{
			this.Visit(type.Syntax);
			MemberNameResolver memberNameResolver = new MemberNameResolver(semanticModel);
			this.members.ForEach<MemberNode>((MemberNode x) => {
				string str;
				memberNameResolver.TryResolveMemberSignatureString(x, out str);
				x.DisplayName = str;
				x.CodeFile = type.CodeFile;
				x.LineNumber = x.SyntaxNode.GetLineNumber();
			});
			return this.members.ToList<MemberNode>().AsReadOnly();
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			base.VisitConstructorDeclaration(node);
			MemberNode memberNode = new MemberNode()
			{
				SyntaxNode = node,
				Kind = MemberKind.Constructor
			};
			this.members.Add(memberNode);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			base.VisitDestructorDeclaration(node);
			MemberNode memberNode = new MemberNode()
			{
				SyntaxNode = node,
				Kind = MemberKind.Destructor
			};
			this.members.Add(memberNode);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			base.VisitEventDeclaration(node);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.AddAccessorDeclaration, MemberKind.AddEventHandler);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.RemoveAccessorDeclaration, MemberKind.RemoveEventHandler);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			MemberNode memberNode = new MemberNode()
			{
				SyntaxNode = node,
				Kind = MemberKind.Method
			};
			this.members.Add(memberNode);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			base.VisitPropertyDeclaration(node);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.GetAccessorDeclaration, MemberKind.GetProperty);
			this.AddAccessorNode(node, node.AccessorList, SyntaxKind.SetAccessorDeclaration, MemberKind.SetProperty);
		}
	}
}