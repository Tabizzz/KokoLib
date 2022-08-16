using System.IO;

namespace KokoLib.Emitters;

internal class IntEmitter : ModHandlerEmitter<int>
{
	public override int Read(BinaryReader reader) => reader.ReadInt32();

	public override void Write(BinaryWriter writer, int ins) => writer.Write(ins);
}

internal class StringEmitter : ModHandlerEmitter<string>
{
	public override string Read(BinaryReader reader) => reader.ReadString();

	public override void Write(BinaryWriter writer, string ins) => writer.Write(ins);
}

internal class BoolEmitter : ModHandlerEmitter<bool>
{
	public override bool Read(BinaryReader reader) => reader.ReadBoolean();

	public override void Write(BinaryWriter writer, bool ins) => writer.Write(ins);
}


internal class ByteEmitter : ModHandlerEmitter<byte>
{
	public override byte Read(BinaryReader reader) => reader.ReadByte();

	public override void Write(BinaryWriter writer, byte ins) => writer.Write(ins);
}

internal class SByteEmitter : ModHandlerEmitter<sbyte>
{
	public override sbyte Read(BinaryReader reader) => reader.ReadSByte();

	public override void Write(BinaryWriter writer, sbyte ins) => writer.Write(ins);
}

internal class ShortEmitter : ModHandlerEmitter<short>
{
	public override short Read(BinaryReader reader) => reader.ReadInt16();

	public override void Write(BinaryWriter writer, short ins) => writer.Write(ins);
}

internal class UShortEmitter : ModHandlerEmitter<ushort>
{
	public override ushort Read(BinaryReader reader) => reader.ReadUInt16();

	public override void Write(BinaryWriter writer, ushort ins) => writer.Write(ins);
}

internal class FloatEmitter : ModHandlerEmitter<float>
{
	public override float Read(BinaryReader reader) => reader.ReadSingle();

	public override void Write(BinaryWriter writer, float ins) => writer.Write(ins);
}

internal class DoubleEmitter : ModHandlerEmitter<double>
{
	public override double Read(BinaryReader reader) => reader.ReadDouble();

	public override void Write(BinaryWriter writer, double ins) => writer.Write(ins);
}