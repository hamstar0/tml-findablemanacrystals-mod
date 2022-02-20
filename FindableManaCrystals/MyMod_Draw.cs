using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace FindableManaCrystals {
	public partial class FMCMod : Mod {
		public override void ModifyInterfaceLayers( List<GameInterfaceLayer> layers ) {
			int layerIdx = layers.FindIndex( layer => layer.Name.Equals("Vanilla: Mouse Over") );
			if( layerIdx == -1 ) {
				return;
			}

			//

			var binocsLayer = new LegacyGameInterfaceLayer(
				"FMC: Binocs Icon",
				() => {
					this.DrawBinocsIconIf( Main.spriteBatch );
					return true;
				},
				InterfaceScaleType.UI
			);

			//

			layers.Insert( layerIdx + 1, binocsLayer );
		}


		////////////////

		private bool DrawBinocsIconIf( SpriteBatch sb ) {
			Item heldItem = Main.LocalPlayer.HeldItem;

			if( heldItem?.active != true ) {
				return false;
			}
			if( heldItem.type != ItemID.Binoculars ) {
				return false;
			}

			var myplayer = Main.LocalPlayer.GetModPlayer<FMCPlayer>();
			if( myplayer.IsBinocFocus ) {
				return false;
			}

			//

			Texture2D mouseTex = ModContent.GetTexture( "FindableManaCrystals/MouseLeftIcon" );

			Vector2 pos = Main.MouseScreen + new Vector2(-32f, -16f);

			sb.Draw(
				texture: mouseTex,
				position: pos,
				color: Color.White * 0.35f
			);

			return true;
		}
	}
}
