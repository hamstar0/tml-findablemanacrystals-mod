﻿using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.TModLoader;
using ModLibsTiles.Classes.Tiles.TilePattern;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals.WorldGeneration {
	class ManaShardsWorldGenPass : GenPass {
		public static bool GetRandomShardAttachableTile(
				Rectangle within,
				int maxAttempts,
				TilePattern pattern,
				out (int TileX, int TileY) randTile ) {
			var rand = TmlLibraries.SafelyGetRand();
			int attempts = 0;
			int randCaveTileX, randCaveTileY;
			
			do {
				randCaveTileX = rand.Next( within.X, within.X + within.Width );
				randCaveTileY = rand.Next( within.Y, within.Y + within.Height );

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

		public ManaShardsWorldGenPass( int shards ) : base( "Findable Mana Crystals: Populate Shards", 1f ) {
			this.NeededShards = shards;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			(int TileX, int TileY) randCenterTile;
			float stepWeight = 1f / (float)this.NeededShards;

			TilePattern pattern = ModContent.GetInstance<FMCWorld>()
				.ManaCrystalShardPattern;

			var within = new Rectangle(
				64,
				(int)Main.worldSurface,
				Main.maxTilesX - 128,
				Main.maxTilesY - ( 220 + 64 )
			);

			if( progress != null ) {
				progress.Message = "Pre-placing Mana Crystal Shards: %";
			}

			for( int i = 0; i < this.NeededShards; i++ ) {
				progress?.Set( stepWeight * (float)i );

				if( ManaShardsWorldGenPass.GetRandomShardAttachableTile(within, 10000, pattern, out randCenterTile) ) {
					if( !this.SpawnShard(randCenterTile.TileX, randCenterTile.TileY) ) {
						i--;
					}
				}
			}
		}


		////////////////

		private bool SpawnShard( int tileX, int tileY ) {
			ushort shardTileType = (ushort)ModContent.TileType<ManaCrystalShardTile>();

			//WorldGen.Place1x1( tileX, tileY, shardTileType, 0 );
			WorldGen.PlaceObject( tileX, tileY, shardTileType );

			if( Main.tile[tileX, tileY].type == shardTileType ) {
				if( FMCConfig.Instance.DebugModeWorldGenInfo ) {
					LogLibraries.Log( "Placed Mana Crystal Shard (of " + this.NeededShards + ")" +
						" at " + tileX + "," + tileY +
						" (" + ( tileX << 4 ) + "," + ( tileY << 4 ) + ")"
					);
				}

				return true;
			}

			return false;
		}
	}
}
