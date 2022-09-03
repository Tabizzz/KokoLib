using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class BroadcastAttribute : Attribute
{
	public BroadcastAttribute(bool toSenderOnly = false, bool excludeSender = false)
	{
		ExcludeSender = excludeSender;
		ToSenderOnly = toSenderOnly;
	}

	public bool ExcludeSender { get; }
	public bool ToSenderOnly { get; }
}