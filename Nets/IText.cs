using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KokoLib.Nets;

[Broadcast] // this indicate that he server will resend the packet to clients
public interface IText
{
	void New(string msg);

	private class TextImp : ModHandler<IText>, IText
	{
		public override IText Handler => this;

		public void New(string msg)
		{
			if(Main.netMode != NetmodeID.Server)
				Main.NewText(msg);
		}
	}
} 