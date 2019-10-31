using FindableManaCrystals.Items;
using FindableManaCrystals.Tiles;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Recipes;
using HamstarHelpers.Services.Debug.CustomHotkeys;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace FindableManaCrystals {
	public class FindableManaCrystalsMod : Mod {
		public static string GithubUserName => "hamstar0";
		public static string GithubProjectName => "tml-findablemanacrystals-mod";


		////////////////

		public static FindableManaCrystalsMod Instance { get; private set; }

		public static FindableManaCrystalsConfig Config => ModContent.GetInstance<FindableManaCrystalsConfig>();



		////////////////

		public FindableManaCrystalsMod() {
			FindableManaCrystalsMod.Instance = this;
		}

		////////////////

		public override void Load() {
			ManaCrystalShardTile.InitializeSingleton();
			FindableManaCrystalsWorld.InitializeSingleton();
			FindableManaCrystalsProjectile.InitializeSingleton();
		}

		public override void Unload() {
			FindableManaCrystalsMod.Instance = null;
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
			if( FindableManaCrystalsMod.Config.ManaCrystalShardsPerManaCrystal == 0 ) {
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
					recipe.requiredItem[i].stack = FindableManaCrystalsMod.Config.ManaCrystalShardsPerManaCrystal;
					break;
				}
			}
		}
	}
}
