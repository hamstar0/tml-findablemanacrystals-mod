using HamstarHelpers.Helpers.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		public override void ModifyLight( int i, int j, ref float r, ref float g, ref float b ) {
			float illum = this.GetIlluminationAt( i, j );

			r = illum * 0.32f;
			g = illum * 0.32f;
			b = illum;
		}

		public override void SetSpriteEffects( int i, int j, ref SpriteEffects spriteEffects ) {
			// Flips the sprite if x coord is odd. Makes the tile more interesting.
			if( (i % 2) == 1 ) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}


		////

		public override bool PreDraw( int i, int j, SpriteBatch spriteBatch ) {
			this.UpdateDrawnTileSlow( i, j );
			this.UpdateIlluminationAt( i, j );

			float illum = this.GetIlluminationAt( i, j );
			if( illum <= 0f ) {
				return false;
			} else if( illum >= 0.9f ) {
				Dust.NewDust( new Vector2(i<<4, j<<4), 16, 16, 20, 0f, 0f, 0, new Color(255, 255, 255), 0.5f );
			}

			SpriteEffects effects = SpriteEffects.None;
			this.SetSpriteEffects( i, j, ref effects );

			Tile tile = Main.tile[ i, j ];
			Texture2D texture = Main.tileTexture[ this.Type ];
			/*if (Main.canDrawColorTile(i, j)) {
				texture = Main.tileAltTexture[Type, (int)tile.color()];
			}
			else {
				texture = Main.tileTexture[Type];
			}*/

			Vector2 zero = !Main.drawToScreen
				? new Vector2( Main.offScreenRange, Main.offScreenRange )
				: Vector2.Zero;

			int x = (i<<4) - (int)Main.screenPosition.X;
			int y = (j<<4) - (int)Main.screenPosition.Y;

			spriteBatch.Draw(
				texture: texture,
				position: new Vector2(x, y) + zero,
				sourceRectangle: new Rectangle( tile.frameX, tile.frameY, 16, 16 ),
				color: Lighting.GetColor(i, j) * illum * 0.5f,
				rotation: 0f,
				origin: default(Vector2),
				scale: 1f,
				effects: effects,
				layerDepth: 0f
			);

			return false; // return false to stop vanilla draw.
		}


		public override void PostDraw( int i, int j, SpriteBatch spriteBatch ) {
			float brightness = Lighting.Brightness( i, j );

			if( brightness > 0.05f ) {
				var myworld = ModContent.GetInstance<FindableManaCrystalsWorld>();
				myworld.QueueManaCrystalShardCheck( i, j, brightness );
			}
		}
	}
}
