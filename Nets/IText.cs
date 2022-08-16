﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KokoLib.Nets;

public interface IText
{
	void New(string msg);

	private class TextImp : ModHandler<IText>, IText
	{
		public override IText Handler => this;

		[Broadcast] // this indicate that he server will resend the packet to clients
		[RunIn(HandlerMode.Client)] // this mark this method as only run in clients, including SinglePlayer
		public void New(string msg)
		{
			if(Main.netMode is NetmodeID.MultiplayerClient or NetmodeID.SinglePlayer)
			{
				Main.NewText(msg);
			}
			else
			{
				Net.Text.New(msg);
			}
		}
	}
} 
