using FindableManaCrystals.Tiles;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Timers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


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

		public float? MeasureClosestOnScreenManaCrystalShardTileDistance() {
			float closestSqr = -1;
			int midWldX = (int)Main.screenPosition.X - ( Main.screenWidth / 2 );
			int midWldY = (int)Main.screenPosition.Y - ( Main.screenHeight / 2 );

			int radius = FindableManaCrystalsConfig.Instance.BinocularDetectionRadiusTiles;
			int maxDistSqr = radius * radius;

			int midTileX = Math.Max( 0, midWldX >> 4 );
			int midTileY = Math.Max( 0, midWldY >> 4 );
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

			return closestSqr == -1
				? null
				: (float?)Math.Sqrt( closestSqr );
		}


		public void AnimateManaCrystalShardHint() {
			if( Timers.GetTimerTickDuration("ManaCrystalShardHint") > 0 ) {
				return;
			}

			if( this.MeasureClosestOnScreenManaCrystalShardTileDistance() == null ) {
				return;
			}

			int beginTicks = FindableManaCrystalsConfig.Instance.BinocularsHintBeginDurationTicks;
			Timers.SetTimer( "ManaCrystalShardHint", beginTicks, false, () => {
				Item heldItem = Main.LocalPlayer.HeldItem;
				if( heldItem == null || heldItem.IsAir || heldItem.type != ItemID.Binoculars ) {
					return 0;
				}

				float? newTileProximityIf = this.MeasureClosestOnScreenManaCrystalShardTileDistance();
				if( !newTileProximityIf.HasValue ) {
					return 0;
				}

				int dustIdx = Dust.NewDust(
					Main.screenPosition,
					Main.screenWidth,
					Main.screenHeight,
					59,
					0f,
					0f,
					0,
					new Color( 255, 255, 255 ),
					1.25f
				);
				Dust dust = Main.dust[dustIdx];
				dust.noGravity = true;
				dust.noLight = true;

				float intensity = 1f - FindableManaCrystalsConfig.Instance.BinocularsHintIntensity;
				float newTileProximity = newTileProximityIf.Value * intensity;

				return (int)Math.Max( 5, newTileProximity );
			} );
		}
	}
}
