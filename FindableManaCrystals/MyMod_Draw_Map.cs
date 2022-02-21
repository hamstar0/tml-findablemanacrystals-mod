using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.Draw;
using ModLibsGeneral.Libraries.HUD;


namespace FindableManaCrystals {
	public partial class FMCMod : Mod {
		public static bool IsNearer( Vector2 source, Vector2 nearerThan, Vector2 lastKnownNearest ) {
			return (source - nearerThan).LengthSquared() < (source - lastKnownNearest).LengthSquared();
		}



		////////////////

		public override void PostDrawFullscreenMap( ref string mouseText ) {
			var myplayer = Main.LocalPlayer.GetModPlayer<FMCPlayer>();

			if( myplayer.IsNearSurveyStation ) {
				this.DrawAllShardMapChunks();
			}
		}


		////////////////

		private void DrawAllShardMapChunks() {
			int chunkSize = FMCWorld.ChunkTileSize;

			(int x, int y, bool isOnScreen) topLeftTile = HUDMapLibraries.FindTopLeftTileOfFullscreenMap();
//Main.spriteBatch.DrawString( Main.fontMouseText, "top left: "+topLeft.x+","+topLeft.y, new Vector2(16,400), Color.White );
			int minTileX = Math.Max( topLeftTile.x, 0 );
			int minTileY = Math.Max( topLeftTile.y, 0 );
			minTileX = (minTileX / chunkSize) * chunkSize;
			minTileY = (minTileY / chunkSize) * chunkSize;

			this.DrawAllShardMapChunksWithin( chunkSize, minTileX, minTileY );
		}


		private void DrawAllShardMapChunksWithin( int chunkSize, int minTileX, int minTileY ) {
			((int x, int y) tilePos, Vector2 scrPos) closestChunkToCursor = default;

			bool rowIsInBounds = false;
			bool colIsInBounds = false;

			for( int tileY = minTileY; tileY < Main.maxTilesY; tileY += chunkSize ) {
				rowIsInBounds = false;

				for( int tileX = minTileX; tileX < Main.maxTilesX; tileX += chunkSize ) {
					var wldPos = new Vector2( tileX * 16, tileY * 16 );
					(Vector2 scrPos, bool isOnScreen) mapScrPos = HUDMapLibraries.GetFullMapPositionAsScreenPosition(
						wldPos
					);

					if( !mapScrPos.isOnScreen ) {
						if( !rowIsInBounds ) {
							continue;	// Skip until map is on screen
						} else {
							break;	// We're now offscreen
						}
					}

					//

					rowIsInBounds = true;

					if( FMCMod.IsNearer(Main.MouseScreen, mapScrPos.scrPos, closestChunkToCursor.scrPos) ) {
						closestChunkToCursor = (
							tilePos: (tileX, tileY),
							scrPos: mapScrPos.scrPos
						);
					}

					if( this.DrawShardMapChunk_If( chunkSize, tileX, tileY, mapScrPos.scrPos, false) ) {
						//drawnChunks++;
					}
				}

				if( rowIsInBounds ) {
					colIsInBounds = true;
				} else if( colIsInBounds ) {
					break;
				}
			}

			//

			this.DrawShardMapChunk_If(
				chunkSize: chunkSize,
				tileX: closestChunkToCursor.tilePos.x,
				tileY: closestChunkToCursor.tilePos.y,
				screenPos: closestChunkToCursor.scrPos,
				isHighlighted: true
			);
		}


		////////////////

		private bool DrawShardMapChunk_If( int chunkSize, int tileX, int tileY, Vector2 screenPos, bool isHighlighted ) {
			if( tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY ) {
				return false;
			}

			if( !FMCWorld.HasShardsInChunk(tileX, tileY) ) {
				return false;
			}

			//

			float scale = HUDMapLibraries.GetFullMapScale();

			var rect = new Rectangle(
				x: (int)screenPos.X,
				y: (int)screenPos.Y,
				width: (int)((float)chunkSize * scale),
				height: (int)((float)chunkSize * scale)
			);

			//

			float pulse = (float)Main.mouseTextColor / 255f;

			float bgLit = isHighlighted ? 0.35f * pulse : 0.15f;
			float fgLit = isHighlighted ? 0.8f * pulse : 0.65f;

			DrawLibraries.DrawBorderedRect(
				sb: Main.spriteBatch,
				bgColor: Color.White * bgLit,
				borderColor: Color.White * fgLit,
				rect: rect,
				borderWidth: 2
			);

			if( isHighlighted ) {
				Utils.DrawBorderStringFourWay(
					sb: Main.spriteBatch,
					font: Main.fontMouseText,
					text: "Shards Found",
					x: Main.MouseScreen.X + 12,
					y: Main.MouseScreen.Y + 16,
					textColor: Color.White * pulse,
					borderColor: Color.Black * pulse,
					origin: default,
					scale: 1f
				);
			}

			return true;
		}
	}
}