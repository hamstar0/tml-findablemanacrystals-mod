using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using Terraria;
using Terraria.ModLoader;


namespace FindableManaCrystals.Tiles {
	partial class ManaCrystalShardTile : ModTile {
		public float GetIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			float illum = singleton.IlluminatedCrystals.Get2DOrDefault( i, j );

			return illum;
			/*// Animate flicker
			return ( (int)( illum * 100 ) % 2 ) == 0
				? illum
				: 0f;*/
		}

		public void SetIlluminateAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( !singleton.IlluminatedCrystals.ContainsKey(i) || !singleton.IlluminatedCrystals[i].ContainsKey(j) ) {
				LogHelpers.Warn( "Cannot illuminate "+i+","+j+"; no shard defined");
				return;
			}

			if( Main.tile[i,j].type != ModContent.TileType<ManaCrystalShardTile>() ) {
				LogHelpers.Warn( "Cannot illuminate "+i+","+j+"; incorrect tile");
				return;
			}

			singleton.IlluminatedCrystals[i][j] = 1f;
		}


		////

		private void UpdateIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( !singleton.IlluminatedCrystals.ContainsKey(i) || !singleton.IlluminatedCrystals[i].ContainsKey(j) ) {
				singleton.IlluminatedCrystals.Set2D( i, j, 0f );
				return;
			}

			if( singleton.IlluminatedCrystals[i][j] > 0f ) {
				singleton.IlluminatedCrystals[i][j] -= 0.05f;
			}
		}
	}
}
