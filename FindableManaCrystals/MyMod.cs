using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Recipes;
using FindableManaCrystals.Items;


namespace FindableManaCrystals {
	public class FMCMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-findablemanacrystals-mod";


		////////////////

		public static FMCMod Instance { get; private set; }



		////////////////

		public FMCMod() {
			FMCMod.Instance = this;
		}

		////////////////

		public override void Load() {
			FMCWorld.InitializeSingleton();
			FMCProjectile.InitializeSingleton();
		}

		public override void Unload() {
			FMCMod.Instance = null;
		}


		////////////////

		public override void PostSetupContent() {
			/*CustomHotkeys.BindActionToKey1( "Illuminate", () => {
				var manaTileSingleton = ModContent.GetInstance<ManaCrystalShardTile>();
				foreach( (int tileX, IDictionary<int, float> tileYs) in manaTileSingleton.IlluminatedCrystals.ToArray() ) {
					foreach( (int tileY, float illum) in tileYs.ToArray() ) {
						manaTileSingleton.IlluminatedCrystals[tileX][tileY] = 1f;
					}
				}
				Main.NewText("Lit!");
			} );*/
		}

		public override void PostAddRecipes() {
			var config = FMCConfig.Instance;

			if( config.Get<int>( nameof(FMCConfig.ManaCrystalShardsPerManaCrystal) ) == 0 ) {
				return;
			}

			int shardType = ModContent.ItemType<ManaCrystalShardItem>();

			foreach( Recipe recipe in RecipeFinderHelpers.GetRecipesOfItem( ItemID.ManaCrystal ) ) {
				for( int i = 0; i < recipe.requiredItem.Length; i++ ) {
					if( recipe.requiredItem[i] != null && !recipe.requiredItem[i].IsAir ) {
						continue;
					}

					recipe.requiredItem[i] = new Item();
					recipe.requiredItem[i].SetDefaults( shardType, true );
					recipe.requiredItem[i].stack = config.Get<int>(
						nameof(FMCConfig.ManaCrystalShardsPerManaCrystal)
					);
					break;
				}
			}
		}
	}
}
