using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using FindableManaCrystals.Items;


namespace FindableManaCrystals.Tiles {
	public partial class GeothaumaticSurveyStationTile : ModTile {
		public static void Activate( (int x, int y) topLeftTile ) {
			Main.tile[topLeftTile.x, topLeftTile.y].frameY = 54;
			Main.tile[topLeftTile.x+1, topLeftTile.y].frameY = 54;
			Main.tile[topLeftTile.x+2, topLeftTile.y].frameY = 54;
			Main.tile[topLeftTile.x, topLeftTile.y+1].frameY = 54 + 18;
			Main.tile[topLeftTile.x+1, topLeftTile.y+1].frameY = 54 + 18;
			Main.tile[topLeftTile.x+2, topLeftTile.y+1].frameY = 54 + 18;
			Main.tile[topLeftTile.x, topLeftTile.y+2].frameY = 54 + 36;
			Main.tile[topLeftTile.x+1, topLeftTile.y+2].frameY = 54 + 36;
			Main.tile[topLeftTile.x+2, topLeftTile.y+2].frameY = 54 + 36;
		}

		public static void Deactivate( (int x, int y) topLeftTile ) {
			Main.tile[topLeftTile.x, topLeftTile.y].frameY = 0;
			Main.tile[topLeftTile.x + 1, topLeftTile.y].frameY = 0;
			Main.tile[topLeftTile.x + 2, topLeftTile.y].frameY = 0;
			Main.tile[topLeftTile.x, topLeftTile.y + 1].frameY = 18;
			Main.tile[topLeftTile.x + 1, topLeftTile.y + 1].frameY = 18;
			Main.tile[topLeftTile.x + 2, topLeftTile.y + 1].frameY = 18;
			Main.tile[topLeftTile.x, topLeftTile.y + 2].frameY = 36;
			Main.tile[topLeftTile.x + 1, topLeftTile.y + 2].frameY = 36;
			Main.tile[topLeftTile.x + 2, topLeftTile.y + 2].frameY = 36;
		}


		////////////////

		public override void SetDefaults() {
			Main.tileFrameImportant[ this.Type ] = true;
			Main.tileLavaDeath[ this.Type ] = false;
			Main.tileLighted[ this.Type ] = true;

			TileObjectData.newTile.CopyFrom( TileObjectData.Style3x3Wall );
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile( this.Type );

			ModTranslation name = this.CreateMapEntryName();
			name.SetDefault( "Geothaumatic Surveillance Station" );

			this.AddMapEntry(
				color: new Color(255, 0, 255),
				name: name,
				nameFunc: ( string currTileName, int tileX, int tileY) => currTileName
			);

			this.dustType = 7;
			this.disableSmartCursor = true;
		}


		////////////////

		public override void ModifyLight( int i, int j, ref float r, ref float g, ref float b ) {
			if( Main.tile[i,j].frameY >= 54 ) {
				r = 1.25f;
				g = 2f;
				b = 2f;
			} else {
				r = 0.25f;
				g = 0.25f;
				b = 0.25f;
			}
		}


		////////////////

		public override bool CanKillTile( int i, int j, ref bool blockDamaged ) {
			var config = FMCConfig.Instance;
			
			return config.Get<bool>( nameof(config.IsGeothaumSurveyStationBreakable) );
		}

		public override void KillTile( int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem ) {
			var config = FMCConfig.Instance;

			if( !config.Get<bool>( nameof(config.IsGeothaumSurveyStationBreakable) ) ) {
				fail = true;
				effectOnly = false;
				noItem = true;
			}
		}

		public override void KillMultiTile( int i, int j, int frameX, int frameY ) {
			var config = FMCConfig.Instance;

			if( !config.Get<bool>( nameof(config.IsGeothaumSurveyStationBreakable) ) ) {
				for( int k=i; k<i+3; k++ ) {
					for( int l=j; l<j+3; l++ ) {
						if( Main.tile[k, l].wall <= 0 ) {
							Main.tile[k, l].wall = 2;
						}
					}
				}
			} else {
				if( config.Get<bool>( nameof(config.GeothaumSurveyStationDropsItem) ) ) {
					Item.NewItem(
						X: i * 16,
						Y: j * 16,
						Width: 32,
						Height: 32,
						Type: ModContent.ItemType<GeothaumaticSurveyStationTileItem>()
					);
				}
			}
		}
	}
}
