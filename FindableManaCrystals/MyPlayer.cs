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
				if( this.player.whoAmI == Main.myPlayer ) {	// Current player
					this.UpdateForSurveyStationProximity( out bool wasNear );

					if( this.IsNearSurveyStation != wasNear ) {
						if( this.IsNearSurveyStation ) {
							Main.NewText( "Geothaumatic Surveillance Station active.", Color.Lime );
						}
					}

					//

					Item item = this.player.HeldItem;
					bool isHoldingBinocs = item != null && !item.IsAir && item.type == ItemID.Binoculars;

					//

					this.UpdateForShardViewing( isHoldingBinocs );
				}
			}
		}


		////////////////
		
		public override void ModifyScreenPosition() {
			this.ApplyBinocZoomIf();

			if( this.IsNearSurveyStation ) {
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

			//

			Vector2 scrOffset = Vector2.Normalize(mouseFromScrMid) * percFromScrMid * distance;

			//

			Main.screenPosition = (this.player.MountedCenter - scrDimHalf) + scrOffset;
		}
	}
}
