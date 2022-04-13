using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using ModLibsCore.Classes.Errors;
using ModLibsTiles.Classes.Tiles.TilePattern;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.DotNET.Extensions;
using FindableManaCrystals.Tiles;

namespace FindableManaCrystals.WorldGeneration {
	partial class SurveyStationsWorldGenPass : GenPass {
		public static int GetMountedMagicMirrorTileType() {
			if( ModLoader.GetMod( "MountedMagicMirrors" ) == null ) {
				return -1;
			}
			return SurveyStationsWorldGenPass.GetMountedMagicMirrorTileType_WeakRef();
		}

		public static int GetMountedMagicMirrorTileType_WeakRef() {
			return ModLoader.GetMod( "MountedMagicMirrors" )
				.TileType( "MountedMagicMirrorTile" );
		}


		////////////////

		public static (int minTileX, int maxTileX, int minTileY, int maxTileY) GetTileBoundsForWorld() {
			int minTileX = 64;
			int maxTileX = Main.maxTilesX - minTileX;
			if( Main.maxTilesX < 64 || maxTileX <= minTileX ) {
				minTileX = 0;
				maxTileX = Main.maxTilesX;
			}

			int minTileY = (int)Main.worldSurface;
			int maxTileY = Main.maxTilesY - 220;
			if( Main.maxTilesY <= 220 || maxTileY <= minTileY ) {
				minTileY = 0;
				maxTileY = Main.maxTilesY;
			}

			return (minTileX, maxTileX, minTileY, maxTileY);
		}



		////////////////

		private TilePattern PlacementSpacePattern;
		private int NeededStations;

		private IDictionary<int, ISet<int>> StationPositions = new Dictionary<int, ISet<int>>();



		////////////////

		public SurveyStationsWorldGenPass( int stations ) : base( "PopulateGeothaumSurveyStations", 1f ) {
			this.PlacementSpacePattern = new TilePattern( new TilePatternBuilder {
				AreaFromCenter = new Rectangle( -1, -3, 3, 6 ),
				HasLava = false,
				IsActive = false,
				IsNotAnyOfWallType = new HashSet<int> {
					WallID.SpiderUnsafe,
					WallID.HiveUnsafe,
					WallID.CorruptionUnsafe1,
					WallID.CorruptionUnsafe2,
					WallID.CorruptionUnsafe3,
					WallID.CorruptionUnsafe4,
					WallID.CorruptGrassUnsafe,
					WallID.CrimsonUnsafe1,
					WallID.CrimsonUnsafe2,
					WallID.CrimsonUnsafe3,
					WallID.CrimsonUnsafe4,
					WallID.CrimstoneUnsafe,
					WallID.CrimsonGrassUnsafe,
				}
			} );
			this.NeededStations = stations;
		}


		////////////////

		public override void Apply( GenerationProgress progress ) {
			if( progress != null ) {
				progress.Message = "Pre-placing Geothaumatic Survey Stations: %";
			}

			//

			var config = FMCConfig.Instance;
			int minTileDist = config.Get<int>( nameof(config.MinimumSurveyStationTileSpacing) );
			if( Main.maxTilesX <= (minTileDist + 4) || Main.maxTilesY <= (minTileDist + 4) ) {
				LogLibraries.Warn( "Invalid world size." );
				return;
			}

			//

			for( int i = 0; i < 1000; i++ ) {
				WorldGen.genRand.Next();    // Desyncs this from Wormholes?
			}

			//

			(int TileX, int TileY)? myRandCenterTile;
			(int TileX, int TileY) randCenterTile;

			try {
				for( int i = 0; i < this.NeededStations; i++ ) {
					myRandCenterTile = this.GetRandomOpenStationableCenterTile( 1000 );
					if( !myRandCenterTile.HasValue ) {
						break;
					}

					randCenterTile = myRandCenterTile.Value;

					//

					this.StationPositions.Set2D( randCenterTile.TileX, randCenterTile.TileY );

					this.SpawnStation( randCenterTile.TileX, randCenterTile.TileY );

					//

					progress?.Set( (float)i / (float)this.NeededStations );
				}
			} catch( Exception e ) {
				throw new ModLibsException( "Mounted Mirrors world gen failed.", e );
			}
		}


		////////////////

		private void SpawnStation( int centerTileX, int centerTileY ) {
			this.SpawnStationWalls( centerTileX, centerTileY );

			//

			int gssTile = ModContent.TileType<GeothaumaticSurveyStationTile>();
			WorldGen.Place3x3Wall( centerTileX - 1, centerTileY - 2, (ushort)gssTile, 0 );

			int mmmTile = SurveyStationsWorldGenPass.GetMountedMagicMirrorTileType();
			if( mmmTile != -1 ) {
				WorldGen.Place3x3Wall( centerTileX - 1, centerTileY + 1, (ushort)mmmTile, 0 );
			}

			//

			if( FMCConfig.Instance.DebugModeWorldGenInfo ) {
				LogLibraries.Log( "Placed geothaum survey station ("
					+this.StationPositions.Count+" of "+this.NeededStations+")"+
					" at "+centerTileX+","+centerTileY+
					" ("+(centerTileX << 4)+","+(centerTileY << 4)+")"
				);
			}
		}

		////

		private void SpawnStationWalls( int centerTileX, int centerTileY ) {
			int minX = centerTileX - 3;
			int maxX = centerTileX + 2;
			int minY = centerTileY - 4;
			int maxY = centerTileY + 4;

			for( int x=minX; x<maxX; x++ ) {
				bool isEdge = x <= minX || x >= (maxX-1);

				for( int y=minY; y<maxY; y++ ) {
					bool isTop = y == minY;

					if( (isEdge || isTop) && Main.tile[x, y].wall != WallID.None ) {
						continue;
					}

					//

					Main.tile[x, y].wall = isEdge || isTop
						? WallID.SillyBalloonPurpleWall
						: WallID.LunarBrickWall;

					//

					if( !isEdge && y <= (centerTileY+2) ) {
						Main.tile[x, y]?.active( false );
					}

					//

					WorldGen.SquareWallFrame( x, y );
				}
			}
		}
	}
}
