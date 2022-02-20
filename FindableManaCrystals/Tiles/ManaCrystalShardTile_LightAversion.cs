using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ModLibsCore.Libraries.TModLoader;
using ModLibsGeneral.Libraries.Tiles;
using ModLibsTiles.Classes.Tiles.TilePattern;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		public static void UpdateLightAversionForTile( int tileX, int tileY, float tolerance, float brightness ) {
			if( tolerance == 0 ) {
				return;
			}

			float rand = TmlLibraries.SafelyGetRand().NextFloat();
			rand *= tolerance;

			if( rand < brightness ) {
				ManaCrystalShardTile.TeleportTile( tileX, tileY );
			}
		}


		public static void TeleportTile( int tileX, int tileY ) {
			var config = FMCConfig.Instance;
			(int newTileX, int newTileY) tileAt;
			TilePattern pattern = ModContent.GetInstance<FMCWorld>().ManaCrystalShardPattern;
			int rad = config.Get<int>( nameof(FMCConfig.ManaCrystalShardTeleportRadius) );
			
			var within = new Rectangle(
				(int)MathHelper.Clamp( tileX - rad, 64, (Main.maxTilesX - 64) - (rad+rad) ),
				(int)MathHelper.Clamp( tileY - rad, 64, (Main.maxTilesY - 64) - (rad+rad) ),
				rad + rad,
				rad + rad
			);

			if( ManaShardWorldGenPass.GetRandomShardAttachableTile(within, 100, pattern, out tileAt) ) {
				TileLibraries.Swap1x1(
					fromTileX: tileX,
					fromTileY: tileY,
					toTileX: tileAt.newTileX,
					toTileY: tileAt.newTileY,
					preserveWall: true,
					preserveWire: true,
					preserveLiquid: true
				);

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
