using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Items.Attributes;


namespace FindableManaCrystals {
	class FMCItem : GlobalItem {
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			string modName = "[c/FFFF88:" + FMCMod.Instance.DisplayName + "] - ";
			TooltipLine tip;

			switch( item.type ) {
			case ItemID.Binoculars:
				tip = new TooltipLine(
					this.mod,
					"FindableManaCrystalsBinoculars",
					modName+"May detect hints of nearby magical treasure that Spelunker Potions may miss"
				);

				ItemInformationAttributeHelpers.ApplyTooltipAt( tooltips, tip );
				break;
			case ItemID.ManaCrystal:
				var config = FMCConfig.Instance;

				if( config.Get<bool>( nameof(FMCConfig.ReducedManaCrystalStatIncrease) ) ) {
					int idx = tooltips.FindIndex( t => t.Name == "ManaCrystal" );

					if( idx >= 0 ) {
						tooltips[idx] = new TooltipLine(
							this.mod,
							"FindableManaCrystalsManaCrystal",
							modName+"Permanently increases maximum mana by 10"
						);
					}
				}

				break;
			}
		}


		////////////////

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
