using System.Reflection.Emit;
using System.Reflection;
using System;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;

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
		mod.Logger.Info($"Generated proxy handler: {clientType}");
		proxy = (T)Activator.CreateInstance(clientType);
	}

	private static Type GenerateInterfaceImplementation(ModuleBuilder moduleBuilder, byte id)
	{
		var type = moduleBuilder.DefineType(
				Net.ProxyModuleName + "." + typeof(T).Name + "Proxy",
				TypeAttributes.Public,
				typeof(object),
				new[] { typeof(T) });

		BuildConstructor(type);

		var t = Net.GetAllInterfaceMethods(typeof(T)).ToList();
		t.Sort((m, o) => m.Name.CompareTo(o.Name));
		byte i = 0;
		foreach (var method in t)
		{
			BuildMethod(type, method, i++, id);
		}

		return type.CreateType();
	}

	private static void BuildMethod(TypeBuilder type, MethodInfo interfaceMethodInfo, byte v, byte id)
	{
		MethodAttributes methodAttributes =
				 MethodAttributes.Public
			   | MethodAttributes.Virtual
			   | MethodAttributes.Final
			   | MethodAttributes.HideBySig
			   | MethodAttributes.NewSlot;

		ParameterInfo[] parameters = interfaceMethodInfo.GetParameters();
		Type[] paramTypes = parameters.Select(param => param.ParameterType).ToArray();

		MethodBuilder methodBuilder = type.DefineMethod(interfaceMethodInfo.Name, methodAttributes);

		methodBuilder.SetReturnType(interfaceMethodInfo.ReturnType);
		methodBuilder.SetParameters(paramTypes);

		ILGenerator generator = methodBuilder.GetILGenerator();
		var lc0 = generator.DeclareLocal(typeof(ModPacket));
		var netlabel = generator.DefineLabel();

		var gp = mod.GetPacket;
		var gpmi = gp.Method;

		var thisT = typeof(Net<T>);
		var modf = thisT.GetField("mod", BindingFlags.Static | BindingFlags.Public);

		generator.Emit(OpCodes.Ldsfld, typeof(Main).GetField("netMode"));
		generator.Emit(OpCodes.Ldc_I4_0);
		generator.Emit(OpCodes.Ceq);

		generator.Emit(OpCodes.Brfalse_S, netlabel);

		generator.Emit(OpCodes.Call, thisT.GetProperty("Handler").GetMethod);

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
		var write = typeof(ModPacket).GetMethod("Write", new Type[] { typeof(byte) });
		generator.Emit(OpCodes.Callvirt, write);
		generator.Emit(OpCodes.Ldloc_0);
		generator.Emit(OpCodes.Ldc_I4_S, v);
		generator.Emit(OpCodes.Callvirt, write);

		for (int i = 0; i < paramTypes.Length; i++)
		{
			TypeEmitter.Emit(generator, i, paramTypes[i]);
		}

		generator.Emit(OpCodes.Ldloc_0);
		generator.Emit(OpCodes.Ldc_I4_M1);
		generator.Emit(OpCodes.Ldc_I4_M1);
		var m = typeof(ModPacket).GetMethod("Send");
		generator.Emit(OpCodes.Call, m);

		generator.Emit(OpCodes.Ret);
	}

	private static void BuildConstructor(TypeBuilder type)
	{

		var method = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, Type.EmptyTypes);

		ConstructorInfo ctor = typeof(object).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,	null, new Type[] { }, null);

		ILGenerator generator = method.GetILGenerator();

		// Call object constructor
		generator.Emit(OpCodes.Ldarg_0);
		generator.Emit(OpCodes.Call, ctor);
		generator.Emit(OpCodes.Ret);
	}

	internal static Action<BinaryReader, T> WrapMethod(MethodInfo m)
	{
		var paramTypes = m.GetParameters().Select(param => param.ParameterType).ToArray();
		var met = new DynamicMethod(m.Name + "Wrap", typeof(void), new Type[] { typeof(BinaryReader) , typeof(T) });
		var il = met.GetILGenerator();

		il.Emit(OpCodes.Ldarg_1);

		foreach (var v in paramTypes)
		{
			TypeEmitter.Emit(il, v);
		}

		il.Emit(OpCodes.Callvirt, m);

		il.Emit(OpCodes.Ret);

		return met.CreateDelegate<Action<BinaryReader, T>>();
	}
}
