using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KokoLib.Nets;

public interface IText
{
	[Broadcast(false, true)] // this indicate that he server will resend the packet to clients
	[RunIn(HandlerMode.Client)] // this mark this method as only run in clients, including SinglePlayer
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
