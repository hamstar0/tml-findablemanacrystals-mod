using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace FindableManaCrystals {
	partial class FindableManaCrystalsPlayer : ModPlayer {
		private int ScanTickElapsed = 0;

		////

		private float? PreBinocZoomPercent = null;

		private float BinocZoomPercent = 1f;


		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void SetupStartInventory( IList<Item> items, bool mediumcoreDeath ) {
			if( !mediumcoreDeath ) {
				var config = FMCConfig.Instance;

				if( config.Get<bool>( nameof( FMCConfig.StartPlayersWithBinoculars ) ) ) {
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
					Item item = this.player.HeldItem;
					bool isHoldingBinocs = item != null && !item.IsAir && item.type == ItemID.Binoculars;

					this.UpdateForBinocs( isHoldingBinocs );
				}
			}
		}


		////////////////

		public override void ModifyScreenPosition() {
			this.ApplyBinocZoomIf();
		}
	}
}
