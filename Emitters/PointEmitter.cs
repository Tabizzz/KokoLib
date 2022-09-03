using System.IO;
using Microsoft.Xna.Framework;
namespace KokoLib.Emitters;

class PointEmitter : ModHandlerEmitter<Point>
{
	public override Point Read(BinaryReader reader)
	{
		return new(reader.ReadInt32(), reader.ReadInt32());
	}

	public override void Write(BinaryWriter writer, Point ins)
	{
		writer.Write(ins.X);
		writer.Write(ins.Y);
	}
}