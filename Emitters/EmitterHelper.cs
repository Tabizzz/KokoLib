using System.Collections.Generic;
using System.IO;
namespace KokoLib.Emitters;

public static class EmitterHelper
{
	internal static List<TypedModHandlerEmitter> TypedEmitters = new();

	public static object CallReadEmitter(BinaryReader reader, ushort index)
	{
		return TypedEmitters[index].InternalRead(reader);
	}
	
	public static void CallWriteEmitter(BinaryWriter writer, ushort index, object ins)
	{
		TypedEmitters[index].InternalWrite(writer, ins);
	}
}