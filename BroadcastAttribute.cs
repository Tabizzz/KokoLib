using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BroadcastAttribute : Attribute
{
	public BroadcastAttribute(HandlerMode serverOnly)
	{
		ServerOnly = serverOnly;
	}

	public HandlerMode ServerOnly { get; }
}