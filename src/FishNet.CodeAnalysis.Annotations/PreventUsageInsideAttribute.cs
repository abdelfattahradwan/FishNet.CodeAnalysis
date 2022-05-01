using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PreventUsageInsideAttribute : Attribute
{
	public string FullyQualifiedTypeName { get; }
	public string MemberName { get; }

	public PreventUsageInsideAttribute(string fullyQualifiedTypeName, string memberName)
	{
		FullyQualifiedTypeName = fullyQualifiedTypeName;
		MemberName = memberName;
	}
}
