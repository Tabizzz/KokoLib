using System;
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

	internal static bool IsSupported(Type parameterType) => Array.IndexOf(supportedTypes, parameterType) != -1;


}
