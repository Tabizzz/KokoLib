using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
internal class RunInAttribute : Attribute
{
	public RunInAttribute(HandlerMode mode = HandlerMode.Both, bool serverPreRun = false)
	{
		Mode = mode;
		ServerPreRun = serverPreRun;
	}

	public HandlerMode Mode { get; }
	public bool ServerPreRun { get; }
}