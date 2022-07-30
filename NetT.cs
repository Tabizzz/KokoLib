using System.Reflection.Emit;
using System.Reflection;
using System;
using Terraria.ModLoader;
using System.Linq;

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

	internal static void CreateProxy(ModuleBuilder moduleBuilder)
	{
		var clientType = GenerateInterfaceImplementation(moduleBuilder);
		mod.Logger.Info($"Generated proxy handler: {clientType}");
		proxy = (T)Activator.CreateInstance(clientType);
	}

	private static Type GenerateInterfaceImplementation(ModuleBuilder moduleBuilder)
	{
		var type = moduleBuilder.DefineType(
				Net.ProxyModuleName + "." + typeof(T).Name + "Impl",
				TypeAttributes.Public,
				typeof(object),
				new[] { typeof(T) });

		BuildConstructor(type);

		foreach (var method in Net.GetAllInterfaceMethods(typeof(T)))
		{
			BuildMethod(type, method);
		}

		return type.CreateType();
	}

	private static void BuildMethod(TypeBuilder type, MethodInfo interfaceMethodInfo)
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

		var gp = mod.GetPacket;
		var gpmi = gp.Method;

		var thisT = typeof(Net<T>);
		var modf = thisT.GetField("mod", BindingFlags.Static | BindingFlags.Public);
		
		
		generator.Emit(OpCodes.Ldsfld, modf);
		generator.Emit(OpCodes.Ldc_I4, 256);
		
		generator.Emit(OpCodes.Callvirt, gpmi);
		generator.Emit(OpCodes.Stloc_0);

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
}
