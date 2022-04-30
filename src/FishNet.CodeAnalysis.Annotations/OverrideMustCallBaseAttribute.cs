using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OverrideMustCallBaseAttribute : Attribute
{
	public bool BaseCallMustBeFirstStatement { get; set; }
}
