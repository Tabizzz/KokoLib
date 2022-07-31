using System;
using System.IO;
using System.Linq;
using Terraria.ModLoader;

namespace KokoLib;

public abstract class ModHandler<T> : ModHandler where T : class
{
	public Action<BinaryReader, T>[] Methods;

	protected sealed override void Register()
	{
		ModTypeLookup<ModHandler<T>>.Register(this);
		Net.Register(this, Handler);
	}

	public abstract T Handler { get; }

	public sealed override void SetupContent()
	{
		base.SetupContent();
		var t = Net.GetAllInterfaceMethods(typeof(T)).ToList();
		t.Sort((m, o) => m.Name.CompareTo(o.Name));
		Methods = t.Select(m => Net<T>.WrapMethod(m)).ToArray();
		t.Clear();
		SetStaticDefaults();
	}

	internal sealed override void CreateProxy()
	{
		Net<T>.CreateProxy(Net.moduleBuilder, type);
	}

	public override void Handle(BinaryReader reader, byte method)
	{
		Methods[method](reader, Handler);
	}
}

public abstract class ModHandler : ModType
{
	public byte type;
	public int WhoAmI;

	internal abstract void CreateProxy();

	public abstract void Handle(BinaryReader reader, byte method);
}