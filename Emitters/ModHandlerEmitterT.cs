using System;
using System.IO;
namespace KokoLib.Emitters;

public abstract class ModHandlerEmitter<T> : ModHandlerEmitter
{
	public sealed override Type Type => typeof(T);

	internal sealed override object InternalRead(BinaryReader reader) => Read(reader);

	internal sealed override void InternalWrite(BinaryWriter writer, object ins) => Write(writer, (T)ins);

	internal override object InternalReadArray(BinaryReader reader) => ReadArray(reader);
	
	internal sealed override void InternalWriteArray(BinaryWriter writer, object ins) => Write(writer, (T[])ins);

	public abstract T Read(BinaryReader reader);
	
	public abstract void Write(BinaryWriter writer, T ins);

	public T[] ReadArray(BinaryReader reader)
	{
		var len =reader.ReadInt32();
		var ret = new T[len];
		for (var i = 0; i < len; i++)
			ret[i] = Read(reader);
		return ret;
	}
	
	public void Write(BinaryWriter writer, T[] ins)
	{
		writer.Write(ins.Length);
		foreach (var item in ins)
			Write(writer, item);
	}
}
