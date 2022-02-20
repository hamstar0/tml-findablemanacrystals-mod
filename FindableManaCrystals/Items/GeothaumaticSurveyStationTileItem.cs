using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals.Items {
	public class GeothaumaticSurveyStationTileItem : ModItem {
		public const int Width = 30;
		public const int Height = 30;



		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Geothaumatic Surveillance Station" );
			this.Tooltip.SetDefault(
				"Mount on a wall to enable searching for magical phenomena in nearby terrain."
			);
		}

		public override void SetDefaults() {
			this.item.width = GeothaumaticSurveyStationTileItem.Width;
			this.item.height = GeothaumaticSurveyStationTileItem.Height;
			this.item.value = Item.buyPrice( 0, 10, 0, 0 );
			this.item.rare = 4;
			this.item.maxStack = 99;
			this.item.useTurn = true;
			this.item.autoReuse = true;
			this.item.useAnimation = 15;
			this.item.useTime = 10;
			this.item.useStyle = 1;
			this.item.consumable = true;
			this.item.createTile = ModContent.TileType<GeothaumaticSurveyStationTile>();
		}


		////

		public override void AddRecipes() {
			var recipe = new GeothaumaticSurveyStationTileItemRecipe( this );
			recipe.AddRecipe();
		}
	}




	class GeothaumaticSurveyStationTileItemRecipe : ModRecipe {
		public GeothaumaticSurveyStationTileItemRecipe( GeothaumaticSurveyStationTileItem myitem )
				: base( FMCMod.Instance ) {
			this.AddTile( TileID.Anvils );

			this.AddIngredient( ItemID.Wire, 50 );
			this.AddIngredient( ItemID.Teleporter, 1 );
			this.AddIngredient( ItemID.SoulofFlight, 10 );

			this.SetResult( myitem );
		}


		public override bool RecipeAvailable() {
			var config = FMCConfig.Instance;
			return config.Get<bool>( nameof(config.EnableGeothaumSurveyStationRecipe) );
		}
	}
}