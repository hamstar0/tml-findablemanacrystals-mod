using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ModLibsGeneral.Libraries.Items.Attributes;


namespace FindableManaCrystals {
	partial class FMCItem : GlobalItem {
		public override void ModifyTooltips( Item item, List<TooltipLine> tooltips ) {
			switch( item.type ) {
			case ItemID.Binoculars:
				this.ModifyTooltips_Binocs( tooltips );
				break;
			case ItemID.ManaCrystal:
				this.ModifyTooltips_ManaCrystal( tooltips );
				break;
			}
		}

		////
		
		private void ModifyTooltips_Binocs( List<TooltipLine> tooltips ) {
			string modName = "[c/FFFF88:FMC] - ";

			var tip1 = new TooltipLine(
				this.mod,
				"FindableManaCrystalsBinoculars_Shards",
				modName+"Now detects hints of certain magical phenomena"
			);
			var tip2 = new TooltipLine(
				this.mod,
				"FindableManaCrystalsBinoculars_Zoom",
				modName+"Hold right-click to improve detection + cast light"
			);

			ItemInformationAttributeLibraries.AppendTooltipAtEnd( tooltips, tip1 );
			ItemInformationAttributeLibraries.AppendTooltipAtEnd( tooltips, tip2 );
		}

		private void ModifyTooltips_ManaCrystal( List<TooltipLine> tooltips ) {
			var config = FMCConfig.Instance;
			if( !config.Get<bool>( nameof(FMCConfig.ReducedManaCrystalStatIncrease) ) ) {
				return;
			}

			string modName = "[c/FFFF88:FMC] - ";
			
			int idx = tooltips.FindIndex( t => t.Name == "ManaCrystal" );
			var tip = new TooltipLine(
				this.mod,
				"FindableManaCrystalsManaCrystal",
				modName + "Permanently increases maximum mana by 10"
			);

			if( idx >= 0 ) {
				tooltips[idx] = tip;
			} else {
				ItemInformationAttributeLibraries.AppendTooltipAtEnd( tooltips, tip );
			}
		}
	}
}
