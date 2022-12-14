using KokoLib.Emitters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib;

internal static class TypeEmitter
{
	internal static readonly List<ModHandlerEmitter> Emitters = new(); 
	static readonly Dictionary<string, ushort[]> EmittersByMod = new();
	
	internal static void Emit(string modName, ILGenerator generator, int i, Type type)
	{
		foreach (var index in EmittersByMod[modName])
		{
			if (Emitters[index].Type != type)
				continue;
			Emitters[index].EmitWrite(generator, il=>il.Emit(OpCodes.Ldarg_S, i + 1) );
			return;
		}
		if (!type.IsArray)
			return;
		EmitArray(modName, generator, i, type, type.GetElementType());
	}

	static void EmitArray(string modName, ILGenerator generator, int i, Type type, Type getElementType)
	{
		foreach (var index in EmittersByMod[modName])
		{
			if (Emitters[index].Type != getElementType)
				continue;
		
			Emitters[index].EmitWriteArray(generator, il=>il.Emit(OpCodes.Ldarg_S, i + 1) );
			return;
			
		}
	}

	internal static void Emit(string modName, ILGenerator il, Type type)
	{
		foreach (var index in EmittersByMod[modName])
		{
			if (Emitters[index].Type != type)
				continue;
			Emitters[index].EmitRead(il);
			return;
		}
		if (!type.IsArray)
			return;
		EmitArray(modName, il, type, type.GetElementType());
	}

	private static void EmitArray(string modName, ILGenerator il, Type type1, Type type2)
	{
		foreach (var index in EmittersByMod[modName])
		{
			if (Emitters[index].Type != type2)
				continue;
			Emitters[index].EmitReadArray(il);
			return;
		}
	}

	internal static bool IsSupported(Type parameterType) => 
		Emitters.Find(e => e.Type == parameterType) != null;

	public static void EmittersForMod(Mod mod)
	{
		var list = new Dictionary<Type, ushort>();
		// can be changed this code?
		foreach (var bases in Emitters.Where(e=>e.Mod.Name == "KokoLib"))
			list[bases.Type] = bases.Index;
		foreach (var bases in Emitters.Where(e=>e.Mod.Name != "KokoLib" && e.Mod.Name != mod.Name))
			list[bases.Type] = bases.Index;
		foreach (var bases in Emitters.Where(e=>e.Mod.Name == mod.Name))
			list[bases.Type] = bases.Index;
		EmittersByMod.Add(mod.Name, list.Values.ToArray());
	}

	internal static void RegisterForMod(ModHandlerEmitter modHandlerEmitter)
	{ 
		if (Emitters.Find(p => p.Mod == modHandlerEmitter.Mod && p.Type == modHandlerEmitter.Type) != null)
		{
			throw new($"Mod {modHandlerEmitter.Mod.Name} already register an emitter for type {modHandlerEmitter.Type.Name}");
		}
		modHandlerEmitter.Index = (ushort)Emitters.Count;

		Emitters.Add(modHandlerEmitter);
		
	}
}