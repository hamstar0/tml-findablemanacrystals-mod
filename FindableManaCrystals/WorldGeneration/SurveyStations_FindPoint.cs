using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.World.Generation;
using ModLibsCore.Classes.Errors;
using ModLibsTiles.Classes.Tiles.TilePattern;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.DotNET.Extensions;


namespace FindableManaCrystals.WorldGeneration {
	partial class SurveyStationsWorldGenPass : GenPass {
		private (int TileX, int TileY)? GetRandomOpenStationableCenterTile( int maxAttempts ) {
			(int TileX, int TileY)? myRandTileCenter = null;
			int attempts = 0;

			do {
				myRandTileCenter = this.GetRandomStationableCenterTile( maxAttempts );
				if( !myRandTileCenter.HasValue ) {
					break;
				}

				if( !this.HasNearbyStations(myRandTileCenter.Value.TileX, myRandTileCenter.Value.TileY) ) {
					return myRandTileCenter;
				}
			} while( attempts++ < maxAttempts );

			return null;
		}


		////////////////

		private (int TileX, int TileY)? GetRandomStationableCenterTile( int maxAttempts ) {
			int attempts = 0;
			int randTileX, randTileY;
			var bounds = SurveyStationsWorldGenPass.GetTileBoundsForWorld();

			do {
				randTileX = WorldGen.genRand.Next( bounds.minTileX, bounds.maxTileX );
				randTileY = WorldGen.genRand.Next( bounds.minTileY, bounds.maxTileY );

				if( this.ValidateStationableCenterTile(randTileX, ref randTileY) ) {
					return (randTileX, randTileY);
				}
			} while( attempts++ < maxAttempts );

			return null;
		}


		////////////////

		private bool HasNearbyStations( int tileX, int tileY ) {
			var config = FMCConfig.Instance;
			int minTileDist = config.Get<int>( nameof(config.MinimumSurveyStationTileSpacing) );
			int minTileDistSqt = minTileDist * minTileDist;

			foreach( (int otherTileX, ISet<int> otherTileYs) in this.StationPositions ) {
				foreach( int otherTileY in otherTileYs ) {
					int xDist = otherTileX - tileX;
					int yDist = otherTileY - tileY;
					int xDistSqr = xDist * xDist;
					int yDistSqr = yDist * yDist;

					if( (xDistSqr + yDistSqr) < minTileDistSqt ) {
						return true;
					}
				}
			}

			return false;
		}


		////

		private bool ValidateStationableCenterTile( int centerTileX, ref int centerTileY ) {
			if( !this.PlacementSpacePattern.Check(centerTileX, centerTileY) ) {
				return false;
			}

			while( this.PlacementSpacePattern.Check(centerTileX, centerTileY+1) ) {
				centerTileY += 1;
			}

			return true;
		}
	}
}
