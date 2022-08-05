using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RunInAttribute : Attribute
{
	public RunInAttribute(HandlerMode client)
	{
		Client = client;
	}

	public HandlerMode Client { get; }
}