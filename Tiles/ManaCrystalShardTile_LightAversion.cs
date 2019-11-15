using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.TModLoader;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using HamstarHelpers.Helpers.Tiles;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		public static void UpdateLightAversionForTile( int tileX, int tileY, float tolerance, float brightness ) {
			float rand = TmlHelpers.SafelyGetRand().NextFloat();
			rand *= tolerance;

			if( rand < brightness ) {
				ManaCrystalShardTile.TeleportTile( tileX, tileY );
			}
		}


		public static void TeleportTile( int tileX, int tileY ) {
			(int newTileX, int newTileY) tileAt;
			TilePattern pattern = ModContent.GetInstance<FindableManaCrystalsWorld>().ManaCrystalShardPattern;
			int rad = FindableManaCrystalsMod.Config.ManaCrystalShardTeleportRadius;
			var within = new Rectangle(
				(int)MathHelper.Clamp( tileX - rad, 64, (Main.maxTilesX - 64) - (rad+rad) ),
				(int)MathHelper.Clamp( tileY - rad, 64, (Main.maxTilesY - 64) - (rad+rad) ),
				rad + rad,
				rad + rad
			);

			if( FindableManaCrystalsWorldGenPass.GetRandomShardAttachableTile( within, 100, pattern, out tileAt ) ) {
				TileHelpers.Swap1x1( tileX, tileY, tileAt.newTileX, tileAt.newTileY, true, true, true );

				for( int i = 0; i < 4; i++ ) {
					Dust.NewDust( new Vector2(tileX<<4, tileY<<4), 16, 17, 229 );
				}
			}
		}
	}
}
