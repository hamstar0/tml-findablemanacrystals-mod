using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		private int ScanTickElapsed = 0;

		////

		private float? PreBinocZoomPercent = null;

		private float BinocZoomPercent = 1f;


		////////////////

		public bool IsBinocFocus => this.PreBinocZoomPercent.HasValue;

		public bool IsNearSurveyStation { get; private set; } = false;

		public ( int tileX, int tileY)? CurrentNearbySurveyStationTile { get; private set; } = null;


		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void SetupStartInventory( IList<Item> items, bool mediumcoreDeath ) {
			if( !mediumcoreDeath ) {
				var config = FMCConfig.Instance;

				if( config.Get<bool>( nameof(FMCConfig.StartPlayersWithBinoculars) ) ) {
					var binocs = new Item();
					binocs.SetDefaults( ItemID.Binoculars );
					binocs.stack = 1;

					items.Add( binocs );
				}
			}
		}


		////////////////

		public override void PreUpdate() {
			if( Main.netMode != NetmodeID.Server ) {	// Non-server
				if( this.player.whoAmI == Main.myPlayer ) { // Current player
					this.UpdateForLocalPlayer();
				}
			}
		}

		////

		private void UpdateForLocalPlayer() {
			this.UpdateForSurveyStation();

			//

			Item item = this.player.HeldItem;
			bool isHoldingBinocs = item != null && !item.IsAir && item.type == ItemID.Binoculars;

			this.UpdateForShardViewing( isHoldingBinocs );
		}


		////////////////
		
		public override void ModifyScreenPosition() {
			this.ApplyBinocZoomIf();

			if( this.IsNearSurveyStation && Main.mouseRight ) {
				this.MoveScreenWithMouse( 1024f );
			}
		}


		////////////////
		
		public void MoveScreenWithMouse( float distance ) {
			var scrDim = new Vector2( Main.screenWidth, Main.screenHeight );
			var scrDimHalf = scrDim * 0.5f;

			Vector2 mouseFromScrMid = Main.MouseScreen - scrDimHalf;
			float percFromScrMidX = Math.Abs( mouseFromScrMid.X / scrDimHalf.X ); 
			float percFromScrMidY = Math.Abs( mouseFromScrMid.Y / scrDimHalf.Y );
			float percFromScrMid = Math.Max( percFromScrMidX, percFromScrMidY );
			if( percFromScrMid < 0f || percFromScrMid > 1f ) {
				return;
			}

			//

			Vector2 scrOffsetDir = Vector2.Normalize( mouseFromScrMid );
			if( scrOffsetDir.X == 0f && scrOffsetDir.Y == 0f ) {
				return;
			}
			if( float.IsInfinity(scrOffsetDir.X) || float.IsNaN(scrOffsetDir.X) ) {
				return;
			}
			if( float.IsInfinity(scrOffsetDir.Y) || float.IsNaN(scrOffsetDir.Y) ) {
				return;
			}

			Vector2 scrOffset = scrOffsetDir * percFromScrMid * distance;

			//

			Main.screenPosition = (this.player.MountedCenter - scrDimHalf) + scrOffset;
		}
	}
}
