using System;

namespace FishNet.CodeAnalysis.Annotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class PreventUsageInside : Attribute
    {
    }
