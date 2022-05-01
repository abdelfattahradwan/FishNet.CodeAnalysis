using System;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class TypeExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetFullyQualifiedName(this Type type)
	{
		return $"global::{type.FullName}";
	}
}