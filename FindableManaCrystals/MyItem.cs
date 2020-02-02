﻿using HamstarHelpers.Helpers.Items.Attributes;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace FindableManaCrystals {
	class FindableManaCrystalsItem : GlobalItem {
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			TooltipLine tip;

			switch( item.type ) {
			case ItemID.Binoculars:
				tip = new TooltipLine(
					this.mod,
					"FindableManaCrystalsBinoculars",
					"May detect hints of nearby magical treasure that Spelunker Potions may miss"
				);

				ItemInformationAttributeHelpers.ApplyTooltipAt( tooltips, tip );
				break;
			case ItemID.ManaCrystal:
				if( !FindableManaCrystalsConfig.Instance.ReducedManaCrystalStatIncrease ) {
					break;
				}

				tip = new TooltipLine(
					this.mod,
					"FindableManaCrystalsManaCrystal",
					"Permanently increases maximum mana by 10"
				);

				int idx = tooltips.FindIndex( t => t.Name == "ManaCrystal" );
				tooltips[ idx ] = tip;
				break;
			}
		}


		////////////////

		public override void OnConsumeItem( Item item, Player player ) {
			switch( item.type ) {
			case ItemID.ManaCrystal:
				if( FindableManaCrystalsConfig.Instance.ReducedManaCrystalStatIncrease ) {
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
