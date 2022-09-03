using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RunInAttribute : Attribute
{
	public RunInAttribute(HandlerMode mode)
	{
		Mode = mode;
	}

	public HandlerMode Mode { get; }
}