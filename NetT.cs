using System.Reflection.Emit;
using System.Reflection;
using System;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using KokoLib.Emitters;

namespace KokoLib;

public class Net<T>
{
	internal static T handler;
	internal static T proxy;
	public static Mod mod;

	/// <summary>
	/// Use to invoke the method in the other side
	/// </summary>
	public static T Proxy => proxy;

	public static T Handler => handler;

	internal static void CreateProxy(ModuleBuilder moduleBuilder, byte type)
	{
		var clientType = GenerateInterfaceImplementation(moduleBuilder, type);
		mod.Logger.Info($"Generated proxy handler: {clientType} in mod {Net.HandlerMod.Name} from {mod.Name}");
		proxy = (T)Activator.CreateInstance(clientType);
	}

	static Type GenerateInterfaceImplementation(ModuleBuilder moduleBuilder, byte id)
	{
		var type = moduleBuilder.DefineType(
				Net.ProxyModuleName + "." + mod.Name + "." + typeof(T).Name + "Proxy",
				TypeAttributes.Public,
				typeof(object),
				new[] { typeof(T) });

		BuildConstructor(type);

		var t = Net.GetAllInterfaceMethods(typeof(T)).ToList();
		t.Sort((m, o) => string.Compare(m.Name, o.Name, StringComparison.Ordinal));
		byte i = 0;
		foreach (var method in t)
		{
			var mi = BuildInternalMethod(type, method);
			BuildMethod(type, method, i++, id, mi);
		}

		return type.CreateType();
	}

	private static object BuildInternalMethod(TypeBuilder type, MethodInfo method)
	{
		// for future use
		return null;
	}

	static void BuildMethod(TypeBuilder type, MethodInfo interfaceMethodInfo, byte v, byte id, object mi)
	{
		const MethodAttributes methodAttributes = MethodAttributes.Public
		                                          | MethodAttributes.Virtual
		                                          | MethodAttributes.Final
		                                          | MethodAttributes.HideBySig
		                                          | MethodAttributes.NewSlot;

		var parameters = interfaceMethodInfo.GetParameters();
		var paramTypes = parameters.Select(param => param.ParameterType).ToArray();

		var methodBuilder = type.DefineMethod(interfaceMethodInfo.Name, methodAttributes);

		methodBuilder.SetReturnType(interfaceMethodInfo.ReturnType);
		methodBuilder.SetParameters(paramTypes);

		var generator = methodBuilder.GetILGenerator();
		var lc0 = generator.DeclareLocal(typeof(ModPacket));
		var netlabel = generator.DefineLabel();

		var gp = mod.GetPacket;
		var gpmi = gp.Method;

		var thisT = typeof(Net<T>);
		var modf = typeof(Net).GetField("HandlerMod", BindingFlags.Static | BindingFlags.Public);

		generator.Emit(OpCodes.Ldsfld, typeof(Main).GetField("netMode")!);
		generator.Emit(OpCodes.Ldc_I4_0);
		generator.Emit(OpCodes.Ceq);

		generator.Emit(OpCodes.Brfalse_S, netlabel);

		generator.Emit(OpCodes.Call, thisT.GetProperty("Handler")!.GetMethod!);

		for (int i = 0; i < paramTypes.Length; i++)
		{
			generator.Emit(OpCodes.Ldarg_S, (short)i + 1);
		}

		generator.Emit(OpCodes.Callvirt, interfaceMethodInfo);

		generator.Emit(OpCodes.Ret);


		generator.MarkLabel(netlabel);
		
		generator.Emit(OpCodes.Ldsfld, modf);
		generator.Emit(OpCodes.Ldc_I4, 256);
		
		generator.Emit(OpCodes.Callvirt, gpmi);
		generator.Emit(OpCodes.Stloc_0);

		generator.Emit(OpCodes.Ldloc_0);
		generator.Emit(OpCodes.Ldc_I4_S, id);
		var write = typeof(ModPacket).GetMethod("Write", new[] { typeof(byte) });
		generator.Emit(OpCodes.Callvirt, write);
		generator.Emit(OpCodes.Ldloc_0);
		generator.Emit(OpCodes.Ldc_I4_S, v);
		generator.Emit(OpCodes.Callvirt, write);

		for (int i = 0; i < paramTypes.Length; i++)
		{
			TypeEmitter.Emit(mod.Name ,generator, i, paramTypes[i]);
		}

		generator.Emit(OpCodes.Ldloc_0);
		var m = typeof(EmitterHelper).GetMethod("SendAndReset");
		generator.Emit(OpCodes.Call, m);

		generator.Emit(OpCodes.Ret);
	}

	static void BuildConstructor(TypeBuilder type)
	{

		var method = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, Type.EmptyTypes);

		ConstructorInfo ctor = typeof(object).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,	null, new Type[] { }, null);

		ILGenerator generator = method.GetILGenerator();

		// Call object constructor
		generator.Emit(OpCodes.Ldarg_0);
		generator.Emit(OpCodes.Call, ctor);
		generator.Emit(OpCodes.Ret);
	}

	internal static Action<BinaryReader, T, int> WrapMethod(MethodInfo m, BroadcastAttribute broadcast = null)
	{
		var paramTypes = m.GetParameters().Select(param => param.ParameterType).ToArray();
		var met = new DynamicMethod(m.Name + "Wrap", typeof(void), new[] { typeof(BinaryReader) , typeof(T), typeof(int) });
		var il = met.GetILGenerator();

		broadcast = m.GetCustomAttribute<BroadcastAttribute>() ?? broadcast;

		if (broadcast != null)
			return emitWithBroadcast(il, met, paramTypes, broadcast, m);

		il.Emit(OpCodes.Ldarg_1);

		foreach (var v in paramTypes)
		{
			TypeEmitter.Emit(mod.Name, il, v);
		}

		il.Emit(OpCodes.Callvirt, m);
	
		il.Emit(OpCodes.Ret);

		return met.CreateDelegate<Action<BinaryReader, T, int>>();
	}

	private static Action<BinaryReader, T, int> emitWithBroadcast(ILGenerator il, DynamicMethod met, Type[] paramTypes, BroadcastAttribute broadcast, MethodInfo m)
	{
		var locals = new LocalBuilder[paramTypes.Length];
		for (int i = 0; i < paramTypes.Length; i++)
		{
			locals[i] = il.DeclareLocal(paramTypes[i]);
		}

		for (int i = 0; i < paramTypes.Length; i++)
		{
			Type v = paramTypes[i];
			TypeEmitter.Emit(mod.Name, il, v);
			il.Emit(OpCodes.Stloc_S, locals[i]);
		}

		il.Emit(OpCodes.Ldarg_1);

		for (int i = 0; i < paramTypes.Length; i++)
		{
			il.Emit(OpCodes.Ldloc_S, locals[i]);
		}

		il.Emit(OpCodes.Callvirt, m);

		var end = il.DefineLabel();

		il.Emit(OpCodes.Ldsfld, typeof(Main).GetField("netMode")!);
		il.Emit(OpCodes.Ldc_I4_2);
		il.Emit(OpCodes.Ceq);

		il.Emit(OpCodes.Brfalse_S, end);


		if(broadcast.ToSenderOnly == true)
		{
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Stsfld, typeof(Net).GetField("ToClient")!);
		}
		if (broadcast.ExcludeSender == true)
		{
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Stsfld, typeof(Net).GetField("IgnoreClient")!);
		}

		var proxy = typeof(Net<T>).GetProperty(nameof(Proxy)).GetMethod!;

		il.Emit(OpCodes.Call, proxy);

		for (int i = 0; i < paramTypes.Length; i++)
		{
			il.Emit(OpCodes.Ldloc_S, locals[i]);
		}

		il.Emit(OpCodes.Callvirt, m);

		il.MarkLabel(end);

		il.Emit(OpCodes.Ret);

		return met.CreateDelegate<Action<BinaryReader, T, int>>();
	}

}
