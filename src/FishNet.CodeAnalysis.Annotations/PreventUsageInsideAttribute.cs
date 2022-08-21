using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class PreventUsageInsideAttribute : Attribute
{
	public string FullyQualifiedTypeName { get; }
	public string MemberName { get; }
	public string AdditionalInformation { get; }

	public PreventUsageInsideAttribute(string fullyQualifiedTypeName, string memberName, string additionalInformation = "")
	{
		FullyQualifiedTypeName = fullyQualifiedTypeName;
		MemberName = memberName;
		AdditionalInformation = additionalInformation;
	}
}
