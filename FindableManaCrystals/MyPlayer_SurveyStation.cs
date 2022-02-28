using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		private void UpdateForSurveyStationProximity( out bool wasNear ) {
			wasNear = this.IsNearSurveyStation;

			if( this.player.dead ) {
				this.IsNearSurveyStation = false;

				return;
			}

			//

			int range = 6;
			int stationType = ModContent.TileType<GeothaumaticSurveyStationTile>();
			int tileX = (int)this.player.MountedCenter.X / 16;
			int tileY = (int)this.player.MountedCenter.Y / 16;

			int minX = (tileX - range) <= 0
				? 1
				: tileX - range;
			int minY = (tileY - range) <= 0
				? 1
				: tileY - range;
			int maxX = (tileX + range) >= Main.maxTilesX
				? Main.maxTilesX - 1
				: tileX + range;
			int maxY = (tileY + range) >= Main.maxTilesY
				? Main.maxTilesY - 1
				: tileY + range;

			//

			this.IsNearSurveyStation = false;

			for( int x=minX; x<maxX; x++ ) {
				for( int y=minY; y<maxY; y++ ) {
					Tile tile = Main.tile[x, y];
					if( tile?.active() == true && tile.type == stationType ) {
						this.IsNearSurveyStation = true;

						goto done;
					}
				}
			}

			done:;
		}
	}
}
