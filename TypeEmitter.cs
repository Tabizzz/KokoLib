using System;
using System.IO;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib;

internal static class TypeEmitter
{
	private static Type[] supportedTypes = new Type[]
	{
		typeof(int),
		typeof(string),
		typeof(bool),
		typeof(byte),
		typeof(short),
		typeof(float),
		typeof(double),
	};

	internal static void Emit(ILGenerator generator, int i, Type type)
	{
		generator.Emit(OpCodes.Ldloc_0);
		generator.Emit(OpCodes.Ldarg_S, (short) i + 1);
		var m = typeof(ModPacket).GetMethod("Write", new Type[] { type });
		generator.Emit(OpCodes.Callvirt, m);
	}

	internal static void Emit(ILGenerator il, Type type)
	{
		il.Emit(OpCodes.Ldarg_0);
		var t = typeof(BinaryReader);

		if (type == supportedTypes[0])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadInt32"));
		}
		else if (type == supportedTypes[1])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadString"));
		}
		else if (type == supportedTypes[2])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadBoolean"));
		}
		else if (type == supportedTypes[3])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadByte"));
		}
		else if (type == supportedTypes[4])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadInt16"));
		}
		else if (type == supportedTypes[5])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadSingle"));
		}
		else if (type == supportedTypes[6])
		{
			il.Emit(OpCodes.Call, t.GetMethod("ReadDouble"));
		}

	}

	internal static bool IsSupported(Type parameterType) => Array.IndexOf(supportedTypes, parameterType) != -1;


}
