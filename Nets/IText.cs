using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KokoLib.Nets;

public interface IText
{
	[Broadcast] // this indicate that he server will resend the packet to clients
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
