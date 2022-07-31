using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace KokoLib.Nets;

public interface IText
{
	void New(string msg);

	private class TextImp : ModHandler<IText>, IText
	{
		public override IText Handler => this;
			
		public void New(string msg)
		{
			if(Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.MultiplayerClient)
			{
				Main.NewText(msg);
			}
			else
			{
				// we are in server, sent the messsage to clients
				Net.Text.New(msg);
			}
		}
	}
}