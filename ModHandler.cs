using Terraria.ModLoader;

namespace KokoLib;

public abstract class ModHandler<T> : ModType where T : class
{
	public int type;

	protected sealed override void Register()
	{
		ModTypeLookup<ModHandler<T>>.Register(this);
		Net.Register(Mod, this, Handler);
	}

	public abstract T Handler { get; }
}
