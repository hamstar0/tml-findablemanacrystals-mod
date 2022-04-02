using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.TModLoader;
using ModLibsCore.Services.Timers;
using ModLibsGeneral.Libraries.UI;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		public float? FindClosestOnScreenManaCrystalShardTile(
					out float percentToCenter,
					out (int, int) closestTile ) {
			var config = FMCConfig.Instance;
			int tileRadius = config.Get<int>( nameof(FMCConfig.BinocularsDetectionRadiusTiles) );
			int tileRadiusSqr = tileRadius * tileRadius;

			float closestTileDistSqr = -1;
			int closestTileX = -1;
			int closestTileY = -1;

			//

			Rectangle wldScr = UIZoomLibraries.GetWorldFrameOfScreen( null, true );
			//int midWldX = (int)Main.screenPosition.X + ( Main.screenWidth / 2 );
			//int midWldY = (int)Main.screenPosition.Y + ( Main.screenHeight / 2 );
			int midWldScrX = (int)wldScr.X + (wldScr.Width / 2);
			int midWldScrY = (int)wldScr.Y + (wldScr.Height / 2);

			int midWldScrTileX = midWldScrX / 16;
			int midWldScrTileY = midWldScrY / 16;
			int minWldScrTileX = Math.Max( 0, midWldScrTileX - tileRadius );
			int minWldScrTileY = Math.Max( 0, midWldScrTileY - tileRadius );
			int maxWldScrTileX = Math.Min( Main.maxTilesX - 1, midWldScrTileX + tileRadius );
			int maxWldScrTileY = Math.Min( Main.maxTilesY - 1, midWldScrTileY + tileRadius );
			var midWldScrTile = new Vector2( midWldScrTileX, midWldScrTileY );

			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			//

			for( int tileX = minWldScrTileX; tileX < maxWldScrTileX; tileX++ ) {
				for( int tileY = minWldScrTileY; tileY < maxWldScrTileY; tileY++ ) {
					Tile tile = Main.tile[tileX, tileY];
					if( tile?.active() != true || tile.type != shardType ) {
						continue;
					}

					//

					float tileDistSqr = Vector2.DistanceSquared( new Vector2(tileX, tileY), midWldScrTile );
					if( tileDistSqr >= tileRadiusSqr ) {
						continue;
					}

					//

					if( tileDistSqr < closestTileDistSqr || closestTileDistSqr == -1 ) {
						closestTileX = tileX;
						closestTileY = tileY;
						closestTileDistSqr = tileDistSqr;
					}
				}
			}

			//

			float? result;
			if( closestTileDistSqr == -1 ) {
				result = null;
				percentToCenter = 0f;
				closestTile = default;
			} else {
				float closestTileDist = (float)Math.Sqrt( closestTileDistSqr );

				result = closestTileDist;
				percentToCenter = 1f - (closestTileDist / tileRadius);
				closestTile = (closestTileX, closestTileY);
			}

			return result;
		}


		////////////////

		public void AnimateManaCrystalShardHintFxIf() {
			string timerName = "ManaCrystalShardHint";

			if( Timers.GetTimerTickDuration(timerName) > 0 ) {
				return;
			}

			//

			float percentToCenter;
			float? shardDistFromScrMid = this.FindClosestOnScreenManaCrystalShardTile(
				out percentToCenter,
				out _
			);

			if( !shardDistFromScrMid.HasValue ) {
				return;
			}

			//

			var config = FMCConfig.Instance;

			int beginTicks = config.Get<int>( nameof(FMCConfig.BinocularsHintBeginDurationTicks) );

			//

			Timers.SetTimer( timerName, beginTicks, false, () => {
				if( !this.IsNearSurveyStation ) {
					Item heldItem = Main.LocalPlayer.HeldItem;
					if( heldItem == null || heldItem.IsAir || heldItem.type != ItemID.Binoculars ) {
						return 0;
					}
				}

				//

				shardDistFromScrMid = this.FindClosestOnScreenManaCrystalShardTile( out percentToCenter, out _ );
				if( !shardDistFromScrMid.HasValue ) {
					return 0;
				}

				//

				this.CreateManaShardHintFxAt( percentToCenter, this.IsBinocFocus );

				//

				float binocSparkRateScale = this.IsBinocFocus
					? 3f	// distance increases contrast
					: 1f;
				float rateScaleOfSparks = config.Get<float>( nameof(FMCConfig.BinocularsHintIntensity) );
				float invRateScaleOfSparks = 1f - rateScaleOfSparks;

				float ticksUntilNextHint = shardDistFromScrMid.Value;
				ticksUntilNextHint *= binocSparkRateScale;
				ticksUntilNextHint *= invRateScaleOfSparks;
				ticksUntilNextHint = Math.Max( 4f, ticksUntilNextHint );

				//

				if( config.DebugModeInfo ) {
					DebugLibraries.Print( "FindableManaCrystals",
						"baseSparkRate: " + rateScaleOfSparks.ToString("N2")
						+ ", proximity: " + shardDistFromScrMid.Value.ToString("N2")
						+ ", ticksUntilNextSpark: " + ((int)ticksUntilNextHint).ToString("N2")
					);
				}

				//

				return (int)ticksUntilNextHint;
			} );
		}

		////

		public void CreateManaShardHintFxAt( float intensity, bool isFocusing ) {
			//int scrWid = (int)( (float)Main.screenWidth / Main.GameZoomTarget );
			//int scrHei = (int)( (float)Main.screenHeight / Main.GameZoomTarget );
			//
			//int offsetX = (Main.screenWidth - scrWid) / 2;
			//int offsetY = (Main.screenHeight - scrHei) / 2;
			Rectangle scr = UIZoomLibraries.GetWorldFrameOfScreen( null, true );
			UnifiedRandom rand = TmlLibraries.SafelyGetRand();

			//

			float speedMax = 3f;
			float addedScale = 2f;

			if( isFocusing ) {
				intensity = intensity * ((1f + intensity + intensity) / 3f);
				addedScale = 2.35f;
				speedMax = 4f;
			}

			float speedX = (2f * speedMax * rand.NextFloat()) - speedMax;
			speedX *= intensity;
			float speedY = (2f * speedMax * rand.NextFloat()) - speedMax;
			speedY *= intensity;

			float scale = 0.5f + (addedScale * intensity);

			//

			int dustIdx = Dust.NewDust(
				Position: new Vector2( scr.X, scr.Y ),
				Width: scr.Width,
				Height: scr.Height,
				Type: 59,
				SpeedX: speedX,
				SpeedY: speedY,
				Alpha: 128 - (int)(intensity * 128f),
				newColor: new Color( 255, 255, 255 ),
				Scale: scale
			);
			Dust dust = Main.dust[dustIdx];
			dust.noGravity = true;
			dust.noLight = true;
		}
	}
}
