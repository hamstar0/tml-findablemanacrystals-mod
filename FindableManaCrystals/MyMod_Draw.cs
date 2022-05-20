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
					this.DrawMouseIcon_If( Main.spriteBatch );
					this.DrawMouseReticule_If( Main.spriteBatch );
					return true;
				},
				InterfaceScaleType.UI
			);

			//

			layers.Insert( layerIdx + 1, binocsLayer );
		}


		////////////////

		private bool DrawMouseIcon_If( SpriteBatch sb ) {
			Item heldItem = Main.LocalPlayer.HeldItem;
			bool isHoldingBinocs = heldItem?.active == true && heldItem.type == ItemID.Binoculars;

			var myplayer = Main.LocalPlayer.GetModPlayer<FMCPlayer>();
			if( (!isHoldingBinocs || myplayer.IsBinocFocus) && !myplayer.IsNearSurveyStation ) {
				return false;
			}

			//
			
			Texture2D mouseTex = isHoldingBinocs
				? ModContent.GetTexture( "FindableManaCrystals/MouseLeftIcon" )		// binocs
				: ModContent.GetTexture( "FindableManaCrystals/MouseRightIcon" );	// geothaum station

			Vector2 pos = Main.MouseScreen + new Vector2(-32f, -16f);

			sb.Draw(
				texture: mouseTex,
				position: pos,
				color: Color.White * 0.35f
			);

			//

			return true;
		}


		private bool DrawMouseReticule_If( SpriteBatch sb ) {
			var myplayer = Main.LocalPlayer.GetModPlayer<FMCPlayer>();

			if( !myplayer.IsBinocFocus ) {
				return false;
			}

			//

			sb.Draw(
				texture: Main.magicPixel,
				destinationRectangle: new Rectangle(
					( Main.screenWidth / 2 ) - 8,
					( Main.screenHeight / 2 ) - 1,
					16,
					2
				),
				color: Color.White * 0.35f
			);
			sb.Draw(
				texture: Main.magicPixel,
				destinationRectangle: new Rectangle(
					( Main.screenWidth / 2 ) - 1,
					( Main.screenHeight / 2 ) - 8,
					2,
					16
				),
				color: Color.White * 0.35f
			);

			//

			return true;
		}
	}
}
