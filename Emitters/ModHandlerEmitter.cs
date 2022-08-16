using System;
using System.IO;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib.Emitters;

public abstract class ModHandlerEmitter : ModType, IIndexed
{
	public static readonly Type BinaryReader = typeof(BinaryReader);

	public static readonly Type ModPacket = typeof(ModPacket);
	
	/// <summary>
	/// The type this emitter adds support for, you should not define more than one emitter with the same type.
	/// </summary>
	public abstract Type Type { get; }

	protected sealed override void Register()
	{
		TypeEmitter.RegisterForMod(this);
	} 

	public void EmitRead(ILGenerator il)
	{
		var read = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallReadEmitter));

		// load the reader to stack
		il.Emit(OpCodes.Ldarg_0);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, Index);

		// call the reader
		il.Emit(OpCodes.Call, read!);

		// now we have a Object in the stack, convert to our type
		il.Emit(OpCodes.Unbox_Any, Type);
	}
	
	public void EmitReadArray(ILGenerator il)
	{
		var read = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallReadArrayEmitter));

		// load the reader to stack
		il.Emit(OpCodes.Ldarg_0);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, Index);

		// call the reader
		il.Emit(OpCodes.Call, read!);

		// now we have a Object in the stack, convert to our type[]
		il.Emit(OpCodes.Unbox_Any, Type.MakeArrayType());
	}

	public void EmitWrite(ILGenerator il, Action<ILGenerator> instance)
	{
		var write = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallWriteEmitter));

		// load the writer
		il.Emit(OpCodes.Ldloc_0);
		
		// load the instance to write
		instance(il);

		// if we want to pass a value type we need to box into a object
		if (Type.IsValueType) il.Emit(OpCodes.Box, Type);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, Index);

		//call the writer
		il.Emit(OpCodes.Call, write!);
	}
	
	public void EmitWriteArray(ILGenerator il, Action<ILGenerator> instance)
	{
		var write = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallWriteArrayEmitter));

		// load the writer
		il.Emit(OpCodes.Ldloc_0);
		
		// load the instance to write
		instance(il);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, Index);

		//call the writer
		il.Emit(OpCodes.Call, write!);
	}

	internal abstract object InternalRead(BinaryReader reader);
	
	internal abstract void InternalWrite(BinaryWriter writer, object ins);

	public ushort Index { get; internal set; }

	internal abstract object InternalReadArray(BinaryReader reader);

	internal abstract void InternalWriteArray(BinaryWriter writer, object ins);
}
