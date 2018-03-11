using System;

namespace RoslynMetrics.Contracts
{
	public enum MemberKind
	{
		Method,
		Constructor,
		Destructor,
		GetProperty,
		SetProperty,
		AddEventHandler,
		RemoveEventHandler
	}
}