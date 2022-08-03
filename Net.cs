using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib;

public static partial class Net
{
	internal const string ProxyModuleName = "KokoLib.Proxys";
	internal static readonly ModuleBuilder ModuleBuilder;

	/// <summary>
	/// This is the mod we use to generate and receive packages, by default it is Kokolib but it could be changed
	/// </summary>
	public static Mod HandlerMod;

	static Net()
	{
		// create a dynamic assembly to host all the proxies
		var assemblyName = new AssemblyName(ProxyModuleName);
		var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
		ModuleBuilder = assemblyBuilder.DefineDynamicModule(ProxyModuleName);
	}

	/// <summary>
	/// 
	/// </summary>
	internal static void Register<T>(ModHandler modHandler, T handler) where T : class
	{
		if (modHandler.Mod.Side != ModSide.Both)
		{
			throw new("Handlers can only be used in mods with \"Both\" side");
		}
		ValidateInterface<T>();
		ValidateMethods<T>();

		KokoLib.Handlers.Add(modHandler);
		HandlerMod ??= ModLoader.GetMod("KokoLib");
		Net<T>.mod = modHandler.Mod;
		Net<T>.handler = handler;
	}

	static void ValidateMethods<T>() where T : class
	{
		var methods = GetAllInterfaceMethods(typeof(T));
		foreach (var method in methods)
		{
			if(method.ReturnType != typeof(void))
			{
				throw new("ModHandler methods cant return");
			}
			if(method.IsGenericMethod || method.IsGenericMethodDefinition)
			{
				throw new("ModHandler methods cant be generic");
			}
			foreach (var param in method.GetParameters())
			{
				if (!TypeEmitter.IsSupported(param.ParameterType))
				{
					throw new($"Parameter {param.Name} has an unsupported type");
				}
			}
		}
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

	static void ValidateInterface<T>() where T : class
	{
		if(!typeof(T).IsInterface)
			throw new ArgumentException("Generic type in ModHandler<T> must be an interface");
	}
}