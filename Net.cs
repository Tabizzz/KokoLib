using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib;

public class Net
{
	internal const string ProxyModuleName = "KokoLib.Proxys";
	private static ModuleBuilder moduleBuilder;

	public static void Handle(BinaryReader reader, int whoAmI)
	{

	}

	static Net()
	{
		var assemblyName = new AssemblyName(ProxyModuleName);
		var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
		moduleBuilder = assemblyBuilder.DefineDynamicModule(ProxyModuleName);
	}

	internal static int Register<T>(Mod mod, ModHandler<T> modHandler, T handler) where T : class
	{
		ValidateInterface<T>();
		ValidateMethods<T>();

		Net<T>.mod = mod;
		Net<T>.handler = handler;
		Net<T>.CreateProxy(moduleBuilder);

		return 0;
	}

	private static int ValidateMethods<T>() where T : class
	{
		var methods = GetAllInterfaceMethods(typeof(T));
		var ret = 0;
		foreach (var method in methods)
		{
			ret++;
			if(method.ReturnType != typeof(void))
			{
				throw new Exception("ModHandler methods cant return");
			}
			if(method.IsGenericMethod || method.IsGenericMethodDefinition)
			{
				throw new Exception("ModHandler methods cant be generic");
			}
			foreach (var param in method.GetParameters())
			{
				if (!TypeEmitter.IsSupported(param.ParameterType))
				{
					throw new Exception($"Parameter {param.Name} has an unsupported type");
				}
			}
		}
		return ret;
	}

	internal static IEnumerable<MethodInfo> GetAllInterfaceMethods(Type interfaceType)
	{
		foreach (var parent in interfaceType.GetInterfaces())
		{
			foreach (var parentMethod in GetAllInterfaceMethods(parent))
			{
				yield return parentMethod;
			}
		}

		foreach (var method in interfaceType.GetMethods())
		{
			yield return method;
		}
	}

	private static void ValidateInterface<T>() where T : class
	{
		if(!typeof(T).IsInterface)
			throw new ArgumentException("Generic type in ModHandler<T> must be an interface");
	}
}