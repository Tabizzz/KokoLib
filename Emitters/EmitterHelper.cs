using System.IO;
using Terraria.ModLoader;

namespace KokoLib.Emitters;

public static class EmitterHelper
{
	public static object CallReadEmitter(BinaryReader reader, ushort index)
	{
		return  TypeEmitter.Emitters[index].InternalRead(reader);
	}
	
	public static object CallReadArrayEmitter(BinaryReader reader, ushort index)
	{
		return  TypeEmitter.Emitters[index].InternalReadArray(reader);
	}
	
	public static void CallWriteEmitter(BinaryWriter writer,object ins, ushort index)
	{
		TypeEmitter.Emitters[index].InternalWrite(writer, ins);
	}
	
	public static void CallWriteArrayEmitter(BinaryWriter writer,object ins, ushort index)
	{
		TypeEmitter.Emitters[index].InternalWriteArray(writer, ins);
	}

	public static void SendAndReset(ModPacket packet)
	{
		packet.Send(Net.ToClient, Net.IgnoreClient);
		Net.ToClient = -1;
		Net.IgnoreClient = -1;
	}
}