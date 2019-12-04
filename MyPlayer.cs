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
			float closest = -1, current;
			int maxWldX = (int)Main.screenPosition.X + Main.screenWidth;
			int maxWldY = (int)Main.screenPosition.Y + Main.screenHeight;
			int midWldX = maxWldX - (Main.screenWidth / 2);
			int midWldY = maxWldY - (Main.screenHeight / 2);

			int minTileX = (int)Main.screenPosition.X >> 4;
			int minTileY = (int)Main.screenPosition.Y >> 4;
			int midTileX = midWldX >> 4;
			int midTileY = midWldY >> 4;
			int maxTileX = maxWldX >> 4;
			int maxTileY = maxWldY >> 4;

			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			for( int x = minTileX; x < maxTileX; x++ ) {
				for( int y = minTileY; y < maxTileY; y++ ) {
					Tile tile = Main.tile[x, y];
					if( tile == null || !tile.active() || tile.type != shardType ) {
						continue;
					}

					current = Vector2.DistanceSquared(
						new Vector2( x, y ),
						new Vector2( midTileX, midTileY )
					);
					if( closest == -1 || current < closest ) {
						closest = current;
					}
				}
			}

			return closest == -1
				? null
				: (float?)Math.Sqrt( closest );
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

				float newTileProximity = newTileProximityIf.Value
					* (1f - FindableManaCrystalsConfig.Instance.BinocularsHintIntensity);
				return (int)Math.Max( 6, newTileProximity );
			} );
		}
	}
}
