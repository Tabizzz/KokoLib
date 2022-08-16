using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class BroadcastAttribute : Attribute
{
	public BroadcastAttribute(bool excludeSender = false)
	{
		ExcludeSender = excludeSender;
	}

	public bool ExcludeSender { get; }
}