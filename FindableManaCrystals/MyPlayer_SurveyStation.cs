using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		private void UpdateForSurveyStation() {
			this.UpdateForSurveyStationProximity( out bool isNearStation, out (int, int)? topLeftStationTile );

			if( this.IsNearSurveyStation != isNearStation ) {
				if( isNearStation ) {
					Main.NewText( "Geothaumatic Surveillance Station active.", Color.Lime );
				}

				this.IsNearSurveyStation = isNearStation;
			}

			if( this.CurrentNearbySurveyStationTile != topLeftStationTile ) {
				if( this.CurrentNearbySurveyStationTile.HasValue ) {
					GeothaumaticSurveyStationTile.Deactivate( this.CurrentNearbySurveyStationTile.Value );
				}
				if( topLeftStationTile.HasValue ) {
					GeothaumaticSurveyStationTile.Activate( topLeftStationTile.Value );
				}

				this.CurrentNearbySurveyStationTile = topLeftStationTile;
			}
		}


		////

		private void UpdateForSurveyStationProximity( out bool isNearStation, out (int x, int y)? topLeftStationTile ) {
			if( this.player.dead ) {
				isNearStation = false;
				topLeftStationTile = null;

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

			isNearStation = false;
			topLeftStationTile = null;

			for( int x=minX; x<maxX; x++ ) {
				for( int y=minY; y<maxY; y++ ) {
					Tile tile = Main.tile[x, y];
					if( tile?.active() != true || tile.type != stationType ) {
						continue;
					}

					isNearStation = true;
					topLeftStationTile = (x, y);

					//

					while( tile.frameY != 0 || tile.frameY != 54 ) {
						y--;
						tile = Main.tile[x, y];
						if( tile?.active() != true || tile.type != stationType ) {
							return;
						}
						topLeftStationTile = (x, y);
					}

					while( tile.frameX != 0 ) {
						x--;
						tile = Main.tile[x, y];
						if( tile?.active() != true || tile.type != stationType ) {
							return;
						}
						topLeftStationTile = (x, y);
					}

					return;
				}
			}
		}
	}
}
