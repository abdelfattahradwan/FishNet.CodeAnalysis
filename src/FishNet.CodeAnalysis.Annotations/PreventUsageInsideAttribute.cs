using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
public sealed class PreventUsageInsideAttribute(string fullyQualifiedTypeName, string memberName, string additionalInformation = "") : Attribute
{
	public string FullyQualifiedTypeName { get; } = fullyQualifiedTypeName;

	public string MemberName { get; } = memberName;

	public string AdditionalInformation { get; } = additionalInformation;
}
