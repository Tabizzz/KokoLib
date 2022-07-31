using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace KokoLib;

public class KokoLib : Mod
{
	public static ModHandler[] ModHandlers;
	internal static List<ModHandler> Handlers = new();

	public override void PostSetupContent()
	{
		Handlers.Sort((h, o) => h.Name.CompareTo(o.Name));

		ModHandlers = Handlers.ToArray();
		byte vid = 0;
		foreach (var handler in ModHandlers)
		{
			handler.type = vid++;
		}

		foreach (var handle in Handlers)
		{
			handle.CreateProxy();
		}

		Handlers.Clear();
		Handlers = null;
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		var index = reader.ReadByte();
		var method = reader.ReadByte();
		ModHandlers[index].WhoAmI = whoAmI;
		ModHandlers[index].Handle(reader, method);
		ModHandlers[index].WhoAmI = -1;
	}
}