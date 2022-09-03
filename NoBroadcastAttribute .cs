using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method)]
public class NoBroadcastAttribute : Attribute
{
	public NoBroadcastAttribute()
	{
	}
}