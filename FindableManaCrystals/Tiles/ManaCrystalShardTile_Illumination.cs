using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.DotNET.Extensions;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		private static object MyLock = new object();



		////////////////

		public static bool ContainsIlluminationAt( int i, int j, float minIllum ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) {
				return false;
			}

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton._IlluminatedCrystals == null ) {
					singleton._IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
				}
				return singleton._IlluminatedCrystals.Get2DOrDefault( i, j ) >= minIllum;
			}
		}

		public static bool GetIlluminationAt( int i, int j, out float illum ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) {
				illum = 0f;
				return false;
			}

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton._IlluminatedCrystals == null ) {
					singleton._IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
				}
				return singleton._IlluminatedCrystals.TryGetValue2D( i, j, out illum );
			}
		}

		////

		public static void RemoveIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton._IlluminatedCrystals == null ) {
					singleton._IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
				}
				singleton._IlluminatedCrystals.Remove2D( i, j );
			}
		}

		public static void SetIlluminationAt( int i, int j, float illum ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton._IlluminatedCrystals == null ) {
					singleton._IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
				}
				singleton._IlluminatedCrystals.Set2D( i, j, illum );
			}
		}


		////////////////

		private static void UpdateIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton._IlluminatedCrystals == null ) {
					singleton._IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
				}
				if( !singleton._IlluminatedCrystals.ContainsKey(i) || !singleton._IlluminatedCrystals[i].ContainsKey(j) ) {
					singleton._IlluminatedCrystals.Set2D( i, j, 0f );
					return;
				}

				if( singleton._IlluminatedCrystals[i][j] > 0f ) {
					var config = FMCConfig.Instance;
					string entryName = nameof( FMCConfig.IlluminationDimRate );
					singleton._IlluminatedCrystals[i][j] -= config.Get<float>( entryName );
				}
			}
		}



		////////////////

		private IDictionary<int, IDictionary<int, float>> _IlluminatedCrystals = null;
	}
}
