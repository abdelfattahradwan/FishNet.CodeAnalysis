namespace FishNet.CodeAnalysis.Analyzers;

internal static class DiagnosticIds
{
	/// <summary>
	/// Using DontDestroyOnLoad on a NetworkObject or a NetworkBehaviour isn't allowed.
	/// </summary>
	public const string FN0001 = nameof(FN0001);

	/// <summary>
	/// Don't call DontDestroyOnLoad from inside NetworkBehaviour or a class deriving from it.
	/// </summary>
	public const string FN0002 = nameof(FN0002);

	/// <summary>
	/// Override must call base.
	/// </summary>
	public const string FN0003 = nameof(FN0003);

	/// <summary>
	/// Base call must be the first statement.
	/// </summary>
	public const string FN0004 = nameof(FN0004);

	/// <summary>
	/// NetworkBehaviour constructors are not allowed.
	/// </summary>
	public const string FN0005 = nameof(FN0005);

	/// <summary>
	/// Base calls are not allowed inside Remote procedure calls (ServerRpc, ObserversRpc, TargetRpc).
	/// </summary>
	public const string FN0006 = nameof(FN0006);

	/// <summary>
	/// Use of member not allowed here.
	/// </summary>
	public const string FN0007 = nameof(FN0007);	
}
