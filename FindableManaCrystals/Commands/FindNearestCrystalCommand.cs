using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals.Commands {
	class FindNearestCrystalCommand : ModCommand {
		/// @private
		public override CommandType Type => CommandType.Chat | CommandType.Console;
		/// @private
		public override string Command => "fmc-findnearest";
		/// @private
		public override string Usage => "/" + this.Command;
		/// @private
		public override string Description => "Outputs the coordinates of the nearest mana crystal.";



		////////////////

		/// @private
		public override void Action( CommandCaller caller, string input, string[] args ) {
			if( !FMCConfig.Instance.DebugModeCheatReveal ) {
				caller.Reply( "Cheats not enabled.", Color.Yellow );
				return;
			}

			int manaCrystalType = ModContent.TileType<ManaCrystalShardTile>();
			int shortestSqr = Main.maxTilesX * Main.maxTilesX;
			int nearestX = -1, nearestY = -1;
			int count = 0;

			for( int y=0; y<Main.maxTilesY; y++ ) {
				for( int x=0; x<Main.maxTilesX; x++ ) {
					Tile tile = Main.tile[x, y];
					if( tile == null || !tile.active() || tile.type != manaCrystalType ) {
						continue;
					}

					int diffX = (int)(Main.LocalPlayer.position.X / 16f) - x;
					int diffY = (int)(Main.LocalPlayer.position.Y / 16f) - y;
					int diffSqr = ( diffY * diffY ) + ( diffX * diffX );

					if( diffSqr < shortestSqr ) {
						shortestSqr = diffSqr;
						nearestX = x;
						nearestY = y;
					}

					count++;
				}
			}

			caller.Reply( "Nearest mana crystal at x:"+nearestX+", y:"+nearestY+" (of "+count+")", Color.Lime );
		}
	}
}
