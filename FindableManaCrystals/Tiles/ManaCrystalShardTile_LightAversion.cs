using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using HamstarHelpers.Classes.Tiles.TilePattern;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Helpers.TModLoader;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		public static void UpdateLightAversionForTile( int tileX, int tileY, float tolerance, float brightness ) {
			if( tolerance == 0 ) {
				return;
			}

			float rand = TmlHelpers.SafelyGetRand().NextFloat();
			rand *= tolerance;

			if( rand < brightness ) {
				ManaCrystalShardTile.TeleportTile( tileX, tileY );
			}
		}


		public static void TeleportTile( int tileX, int tileY ) {
			var config = FindableManaCrystalsConfig.Instance;
			(int newTileX, int newTileY) tileAt;
			TilePattern pattern = ModContent.GetInstance<FindableManaCrystalsWorld>().ManaCrystalShardPattern;
			int rad = config.Get<int>( nameof(FindableManaCrystalsConfig.ManaCrystalShardTeleportRadius) );
			
			var within = new Rectangle(
				(int)MathHelper.Clamp( tileX - rad, 64, (Main.maxTilesX - 64) - (rad+rad) ),
				(int)MathHelper.Clamp( tileY - rad, 64, (Main.maxTilesY - 64) - (rad+rad) ),
				rad + rad,
				rad + rad
			);

			if( FindableManaCrystalsWorldGenPass.GetRandomShardAttachableTile( within, 100, pattern, out tileAt ) ) {
				TileHelpers.Swap1x1Synced( tileX, tileY, tileAt.newTileX, tileAt.newTileY, true, true, true );

				for( int i = 0; i < 4; i++ ) {
					Dust.NewDust(
						Position: new Vector2(tileX<<4, tileY<<4),
						Width: 16,
						Height: 17,
						Type: 229
					);
				}
			}
		}
	}
}
