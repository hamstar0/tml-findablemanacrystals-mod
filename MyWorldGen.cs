using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Debug;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	class FindableManaCrystalsWorldGenPass : GenPass {
		public static bool GetRandomShardAttachableTile(
				Rectangle within,
				int maxAttempts,
				TilePattern pattern,
				out (int TileX, int TileY) randTile ) {
			int attempts = 0;
			int randCaveTileX, randCaveTileY;
			
			do {
				randCaveTileX = WorldGen.genRand.Next( within.X, within.X + within.Width );
				randCaveTileY = WorldGen.genRand.Next( within.Y, within.Y + within.Height );

				if( pattern.Check( randCaveTileX, randCaveTileY ) ) {
					break;
				}
			} while( attempts++ < maxAttempts );

			randTile = (randCaveTileX, randCaveTileY);
			return attempts < maxAttempts;
		}



		////////////////

		private int NeededShards;



		////////////////

		public FindableManaCrystalsWorldGenPass( int shards ) : base( "PopulateManaCrystalShards", 1f ) {
			this.NeededShards = shards;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			(int TileX, int TileY) randCenterTile;
			float stepWeight = 1f / (float)this.NeededShards;
			TilePattern pattern = ModContent.GetInstance<FindableManaCrystalsWorld>().ManaCrystalShardPattern;
			var within = new Rectangle( 64, (int)Main.worldSurface, Main.maxTilesX - 128, Main.maxTilesY - ( 220 + 64 ) );

			if( progress != null ) {
				progress.Message = "Pre-placing Mana Crystal Shards: %";
			}

			for( int i = 0; i < this.NeededShards; i++ ) {
				progress?.Set( stepWeight * (float)i );

				if( FindableManaCrystalsWorldGenPass.GetRandomShardAttachableTile(within, 10000, pattern, out randCenterTile) ) {
					this.SpawnShard( randCenterTile.TileX, randCenterTile.TileY );
				}
			}
		}


		////////////////

		private void SpawnShard( int centerTileX, int centerTileY ) {
			ushort shardTile = (ushort)ModContent.TileType<ManaCrystalShardTile>();

			WorldGen.Place1x1( centerTileX, centerTileY, shardTile, 0 );

			if( FindableManaCrystalsMod.Config.DebugModeInfo ) {
				LogHelpers.Log( "Placed Mana Crystal Shard (of " + this.NeededShards + ")" +
					" at " + centerTileX + "," + centerTileY +
					" (" + ( centerTileX << 4 ) + "," + ( centerTileY << 4 ) + ")"
				);
			}
		}
	}
}
