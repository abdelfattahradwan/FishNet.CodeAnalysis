using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OverrideMustCallBaseAttribute : Attribute
{
	public bool BaseCallMustBeFirstStatement { get; set; }
}
