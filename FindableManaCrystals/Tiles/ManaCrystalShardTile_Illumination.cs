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

			//

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton.IlluminatedCrystals == null ) {
					singleton.IlluminatedCrystals = new Dictionary<(int, int), float>();
				}

				return singleton.IlluminatedCrystals.GetOrDefault( (i, j) ) >= minIllum;
			}
		}

		public static bool GetIlluminationAt( int i, int j, out float illum ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) {
				illum = 0f;

				return false;
			}

			//

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton.IlluminatedCrystals == null ) {
					singleton.IlluminatedCrystals = new Dictionary<(int, int), float>();
				}

				return singleton.IlluminatedCrystals.TryGetValue( (i, j), out illum );
			}
		}

		////

		public static void RemoveIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			//

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton.IlluminatedCrystals == null ) {
					singleton.IlluminatedCrystals = new Dictionary<(int, int), float>();
				}
				singleton.IlluminatedCrystals.Remove( (i, j) );
			}
		}

		public static void SetIlluminationAt( int i, int j, float illum ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			//

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton.IlluminatedCrystals == null ) {
					singleton.IlluminatedCrystals = new Dictionary<(int, int), float>();
				}
				singleton.IlluminatedCrystals[ (i, j) ] = illum;
			}
		}


		////////////////

		private static void UpdateIlluminationAt( int i, int j ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			if( singleton == null ) { return; }

			//

			var key = (i, j);

			lock( ManaCrystalShardTile.MyLock ) {
				if( singleton.IlluminatedCrystals == null ) {
					singleton.IlluminatedCrystals = new Dictionary<(int, int), float>();
				}

				if( !singleton.IlluminatedCrystals.ContainsKey(key) ) {
					singleton.IlluminatedCrystals[key] = 0f;

					return;
				}

				//

				if( singleton.IlluminatedCrystals[key] > 0f ) {
					var config = FMCConfig.Instance;
					float dim = config.Get<float>( nameof(config.IlluminationDimRate) );

					singleton.IlluminatedCrystals[key] -= dim;
				}
			}
		}



		////////////////

		private IDictionary<(int tileX, int tileY), float> IlluminatedCrystals = null;
	}
}
