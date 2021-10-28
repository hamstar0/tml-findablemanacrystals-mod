using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsGeneral.Libraries.Items.Attributes;


namespace FindableManaCrystals {
	partial class FMCItem : GlobalItem {
		public override void OnConsumeItem( Item item, Player player ) {
			switch( item.type ) {
			case ItemID.ManaCrystal:
				var config = FMCConfig.Instance;

				if( config.Get<bool>( nameof(FMCConfig.ReducedManaCrystalStatIncrease) ) ) {
					player.statManaMax -= 10;
				}
				this.ModifyPopupText();
				break;
			}
		}


		////////////////

		private void ModifyPopupText() {
			for( int idx = 0; idx < Main.combatText.Length; idx++ ) {
				CombatText txt = Main.combatText[idx];
				if( txt == null || !txt.active ) { continue; }

				if( txt.text.Equals( "20" ) ) {
					txt.text = "10";
					break;
				}
			}
		}
	}
}
