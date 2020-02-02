using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using FindableManaCrystals.Tiles;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.Timers;


namespace FindableManaCrystals {
	partial class FindableManaCrystalsPlayer : ModPlayer {
		private int ScanTickElapsed = 0;

		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void PreUpdate() {
			if( this.ScanTickElapsed++ == 10 ) {
				this.ScanTickElapsed = 0;

				Item item = this.player.HeldItem;

				if( item != null && !item.IsAir && item.type == ItemID.Binoculars ) {
					this.AnimateManaCrystalShardHint();
				}
			}
		}


		////////////////

		public override void SetupStartInventory( IList<Item> items, bool mediumcoreDeath ) {
			if( !mediumcoreDeath ) {
				if( FindableManaCrystalsConfig.Instance.StartPlayersWithBinoculars ) {
					var binocs = new Item();
					binocs.SetDefaults( ItemID.Binoculars );
					binocs.stack = 1;

					items.Add( binocs );
				}
			}
		}


		////////////////

		public float? MeasureClosestOnScreenManaCrystalShardTileDistance( out float percentOfMaxRange ) {
			float closestSqr = -1;
			int midWldX = (int)Main.screenPosition.X + ( Main.screenWidth / 2 );
			int midWldY = (int)Main.screenPosition.Y + ( Main.screenHeight / 2 );

			int radius = FindableManaCrystalsConfig.Instance.BinocularDetectionRadiusTiles;
			int maxDistSqr = radius * radius;

			int midTileX = midWldX >> 4;
			int midTileY = midWldY >> 4;
			int minTileX = Math.Max( 0, midTileX - radius );
			int minTileY = Math.Max( 0, midTileY - radius );
			int maxTileX = Math.Min( Main.maxTilesX - 1, midTileX + radius );
			int maxTileY = Math.Min( Main.maxTilesY - 1, midTileY + radius );

			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			for( int x = minTileX; x < maxTileX; x++ ) {
				for( int y = minTileY; y < maxTileY; y++ ) {
					float distSqr = Vector2.DistanceSquared(
						new Vector2( x, y ),
						new Vector2( midTileX, midTileY )
					);
					if( distSqr >= maxDistSqr ) {
						continue;
					}

					Tile tile = Main.tile[x, y];
					if( tile == null || !tile.active() || tile.type != shardType ) {
						continue;
					}

					if( closestSqr == -1 || distSqr < closestSqr ) {
						closestSqr = distSqr;
					}
				}
			}

			float? result;
			if( closestSqr == -1 ) {
				result = null;
				percentOfMaxRange = 0f;
			} else {
				result = (float?)Math.Sqrt( closestSqr );
				percentOfMaxRange = 1f - (result.Value / radius);
			}

			return result;
		}


		public void AnimateManaCrystalShardHint() {
			if( Timers.GetTimerTickDuration("ManaCrystalShardHint") > 0 ) {
				return;
			}

			float percent;
			if( this.MeasureClosestOnScreenManaCrystalShardTileDistance( out percent ) == null ) {
				return;
			}

			int beginTicks = FindableManaCrystalsConfig.Instance.BinocularsHintBeginDurationTicks;
			Timers.SetTimer( "ManaCrystalShardHint", beginTicks, false, () => {
				Item heldItem = Main.LocalPlayer.HeldItem;
				if( heldItem == null || heldItem.IsAir || heldItem.type != ItemID.Binoculars ) {
					return 0;
				}

				float? newTileProximityIf = this.MeasureClosestOnScreenManaCrystalShardTileDistance( out percent );
				if( !newTileProximityIf.HasValue ) {
					return 0;
				}

				float rateScaleOfSparks = 1f - FindableManaCrystalsConfig.Instance.BinocularsHintIntensity;
				float rateOfSparks = newTileProximityIf.Value * rateScaleOfSparks;
				UnifiedRandom rand = TmlHelpers.SafelyGetRand();

				int dustIdx = Dust.NewDust(
					Position: Main.screenPosition,
					Width: Main.screenWidth,
					Height: Main.screenHeight,
					Type: 59,
					SpeedX: (4f * rand.NextFloat() * percent * percent) - 2f,
					SpeedY: (4f * rand.NextFloat() * percent * percent) - 2f,
					Alpha: 128 - (int)(percent * 128f),
					newColor: new Color( 255, 255, 255 ),
					Scale: 1.25f + (2f * percent * percent)
				);
				Dust dust = Main.dust[dustIdx];
				dust.noGravity = true;
				dust.noLight = true;

				if( FindableManaCrystalsConfig.Instance.DebugModeInfo ) {
					DebugHelpers.Print(
						"FindableManaCrystals",
						"rateOfSparks: " + rateScaleOfSparks.ToString("N2")
							+", proximity: "+newTileProximityIf.Value.ToString("N2")
							+", rate: "+rateOfSparks.ToString("N2")
					);
				}

				return (int)Math.Max( 5, rateOfSparks );
			} );
		}
	}
}
