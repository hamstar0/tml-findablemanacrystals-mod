using FindableManaCrystals.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;


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
			if( !FindableManaCrystalsConfig.Instance.DebugModeCheatReveal ) {
				caller.Reply( "Cheats not enabled.", Color.Yellow );
				return;
			}

			int manaCrystalType = ModContent.TileType<ManaCrystalShardTile>();
			int shortestSqr = Main.maxTilesX * Main.maxTilesX;
			int nearestX = -1, nearestY = -1;

			for( int i=0; i<Main.maxTilesY; i++ ) {
				for( int j = 0; j < Main.maxTilesX; j++ ) {
					Tile tile = Main.tile[i, j];
					if( tile == null || !tile.active() || tile.type != manaCrystalType ) { continue; }

					if( (i*i)+(j*j) < shortestSqr ) {
						nearestX = i;
						nearestY = j;
					}
				}
			}

			caller.Reply( "Nearest mana crystal at x:"+nearestX+", y:"+nearestY, Color.Lime );
		}
	}
}
