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
				tip = new TooltipLine( this.mod, "AdventureModeBinoculars", "May detect hints of nearby magical treasure when used" );
				tooltips.Add( tip );
				break;
			}
		}
	}
}
