using System;

namespace RoslynMetrics.Contracts
{
	public static class MetricNames
	{
		public const string MaintainabilityIndex = "MaintainabilityIndex";

		public const string CyclomaticComplexity = "CyclomaticComplexity";

		public const string ClassCoupling = "ClassCoupling";

		public const string LinesOfCode = "LinesOfCode";

		public const string LinesOfComment = "LinesOfComment";

		public const string DepthOfInheritance = "DepthOfInheritance";

		public const string NumberOfParameters = "NumberOfParameters";

		public const string NumberOfLocalVariables = "NumberOfLocalVariables";

		public const string LackOfCohesionOfMethods = "LackOfCohesionOfMethods";

		public const string LackOfCohesionOfMethodsHendersonSellers = "LackOfCohesionOfMethodsHendersonSellers";

		public const string NumberOfFields = "NumberOfFields";

		public const string NumberOfMethods = "NumberOfMethods";

		public const string NumberOfOverloads = "NumberOfOverloads";

		public const string NumberOfTypes = "NumberOfTypes";

		public const string NumberOfNamespaces = "NumberOfNamespaces";
        public const string NumberOfDeliverdBugs = "NumberOfDeliveredBugs";
	}
}