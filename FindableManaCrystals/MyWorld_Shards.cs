using System;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using FindableManaCrystals.Tiles;
using FindableManaCrystals.NetProtocols;


namespace FindableManaCrystals {
	partial class FMCWorld : ModWorld {
		public const int ChunkTileSize = 16;

		public const int SurveyStationTileScanBoxRadius = FMCWorld.ChunkTileSize * 8;



		////////////////

		public static bool HasShardsInChunk( int tileX, int tileY ) {
			int minX = (tileX / FMCWorld.ChunkTileSize) * FMCWorld.ChunkTileSize;
			int minY = (tileY / FMCWorld.ChunkTileSize) * FMCWorld.ChunkTileSize;
			int maxX = minX + FMCWorld.ChunkTileSize;
			int maxY = minY + FMCWorld.ChunkTileSize;
			ushort shardType = (ushort)ModContent.TileType<ManaCrystalShardTile>();

			for( int j=minY; j<maxY; j++ ) {
				for( int i=minX; i<maxX; i++ ) {
					Tile tile = Main.tile[ i, j ];
					if( tile?.active() == true && tile.type == shardType ) {
						return true;
					}
				}
			}

			return false;
		}



		////////////////

		public void QueueManaCrystalShardCheck( int tileX, int tileY, float brightness ) {
			if( Main.netMode == 1 ) {
				ManaCrystalShardCheckProtocol.QuickRequest( tileX, tileY, brightness );
			} else {
				if( this.ManaCrystalShardIllumCheckQueueSize == (this.ManaCrystalShardIllumCheckQueue.Length - 1) ) {
					Array.Resize( ref this.ManaCrystalShardIllumCheckQueue, this.ManaCrystalShardIllumCheckQueue.Length * 2 );
				}
				this.ManaCrystalShardIllumCheckQueue[ this.ManaCrystalShardIllumCheckQueueSize++ ] = (tileX, tileY);
			}
		}


		////////////////

		private void ProcessManaCrystalShardIllumQueue() {
			var config = FMCConfig.Instance;
			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			for( int i = 0; i < this.ManaCrystalShardIllumCheckQueueSize; i++ ) {
				(int tileX, int tileY) tileAt = this.ManaCrystalShardIllumCheckQueue[i];

				if( this.ManaCrystalShardIllumChecked.ContainsKey(tileAt.tileX) ) {
					if( this.ManaCrystalShardIllumChecked[tileAt.tileX] == tileAt.tileY ) {
						continue;
					}
				}

				this.ManaCrystalShardIllumChecked[tileAt.tileX] = tileAt.tileY;

				Tile tile = Framing.GetTileSafely( tileAt.tileX, tileAt.tileY );
				float brightness = Lighting.Brightness( tileAt.tileX, tileAt.tileY );

				if( tile.active() && tile.type == shardType ) {
					ManaCrystalShardTile.UpdateLightAversionForTile(
						tileAt.tileX,
						tileAt.tileY,
						config.Get<float>( nameof(FMCConfig.ManaCrystalShardLightToleranceScale) ),
						brightness
					);
				}
			}

			this.ManaCrystalShardIllumChecked.Clear();
			this.ManaCrystalShardIllumCheckQueueSize = 0;
		}
	}
}
