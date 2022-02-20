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
		public float? MeasureClosestOnScreenManaCrystalShardTileDistance( out float percentToCenter ) {
			float closestDist = -1;
			int midWldX = (int)Main.screenPosition.X + ( Main.screenWidth / 2 );
			int midWldY = (int)Main.screenPosition.Y + ( Main.screenHeight / 2 );

			var config = FMCConfig.Instance;
			int radius = config.Get<int>( nameof(FMCConfig.BinocularsDetectionRadiusTiles) );

			int midTileX = midWldX >> 4;
			int midTileY = midWldY >> 4;
			int minTileX = Math.Max( 0, midTileX - radius );
			int minTileY = Math.Max( 0, midTileY - radius );
			int maxTileX = Math.Min( Main.maxTilesX - 1, midTileX + radius );
			int maxTileY = Math.Min( Main.maxTilesY - 1, midTileY + radius );

			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			for( int x = minTileX; x < maxTileX; x++ ) {
				for( int y = minTileY; y < maxTileY; y++ ) {
					Tile tile = Main.tile[x, y];
					if( tile == null || !tile.active() || tile.type != shardType ) {
						continue;
					}

					float dist = Vector2.Distance(
						new Vector2( x, y ),
						new Vector2( midTileX, midTileY )
					);
					if( dist >= radius ) {
						continue;
					}

					if( closestDist == -1 || dist < closestDist ) {
						closestDist = dist;
					}
				}
			}

			float? result;
			if( closestDist == -1 ) {
				result = null;
				percentToCenter = 0f;
			} else {
				result = closestDist;
				percentToCenter = 1f - (closestDist / radius);
			}

			return result;
		}


		////////////////

		public void AnimateManaCrystalShardHintFxIf() {
			string timerName = "ManaCrystalShardHint";
			if( Timers.GetTimerTickDuration(timerName) > 0 ) {
				return;
			}

			float percentToCenter;
			if( this.MeasureClosestOnScreenManaCrystalShardTileDistance(out percentToCenter) == null ) {
				return;
			}

			//

			var config = FMCConfig.Instance;

			int beginTicks = config.Get<int>( nameof(FMCConfig.BinocularsHintBeginDurationTicks) );

			//

			Timers.SetTimer( timerName, beginTicks, false, () => {
				Item heldItem = Main.LocalPlayer.HeldItem;
				if( heldItem == null || heldItem.IsAir || heldItem.type != ItemID.Binoculars ) {
					return 0;
				}

				float? newTileProximityIf = this.MeasureClosestOnScreenManaCrystalShardTileDistance( out percentToCenter );
				if( !newTileProximityIf.HasValue ) {
					return 0;
				}

				//

				if( this.IsBinocFocus ) {
					percentToCenter = percentToCenter * percentToCenter * percentToCenter;
					newTileProximityIf *= 3f;
				}

				this.CreateManaShardHintFxAt( percentToCenter, this.IsBinocFocus );

				//

				float rateScaleOfSparks = config.Get<float>( nameof(FMCConfig.BinocularsHintIntensity) );
				float invRateScaleOfSparks = 1f - rateScaleOfSparks;
				int tickDurationUntilNextSpark = (int)(newTileProximityIf.Value * invRateScaleOfSparks);

				if( config.DebugModeInfo ) {
					DebugLibraries.Print(
						"FindableManaCrystals",
							"baseSparkRate: " + rateScaleOfSparks.ToString( "N2" )
							+ ", proximity: " + newTileProximityIf.Value.ToString( "N2" )
							+ ", ticksUntilNextSpark: " + tickDurationUntilNextSpark.ToString( "N2" )
					);
				}

				return (int)Math.Max( 5, tickDurationUntilNextSpark );
			} );
		}

		////

		public void CreateManaShardHintFxAt( float percentToCenter, bool isFocusing ) {
			//int scrWid = (int)( (float)Main.screenWidth / Main.GameZoomTarget );
			//int scrHei = (int)( (float)Main.screenHeight / Main.GameZoomTarget );
			//
			//int offsetX = (Main.screenWidth - scrWid) / 2;
			//int offsetY = (Main.screenHeight - scrHei) / 2;
			Rectangle scr = UIZoomLibraries.GetWorldFrameOfScreen( null, true );
			UnifiedRandom rand = TmlLibraries.SafelyGetRand();

			float maxVel = 2f;

			if( isFocusing ) {
				percentToCenter *= percentToCenter;
				maxVel = 2.5f;
			}

			int dustIdx = Dust.NewDust(
				Position: new Vector2( scr.X, scr.Y ),
				Width: scr.Width,
				Height: scr.Height,
				Type: 59,
				SpeedX: ((2f * maxVel * rand.NextFloat()) - maxVel) * percentToCenter,
				SpeedY: ((2f * maxVel * rand.NextFloat()) - maxVel) * percentToCenter,
				Alpha: 128 - (int)(percentToCenter * 128f),
				newColor: new Color( 255, 255, 255 ),
				Scale: 1f + (2f * percentToCenter)
			);
			Dust dust = Main.dust[dustIdx];
			dust.noGravity = true;
			dust.noLight = true;
		}
	}
}
