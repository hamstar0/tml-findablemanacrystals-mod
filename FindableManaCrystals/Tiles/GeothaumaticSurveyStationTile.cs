using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ModLibsCore.Libraries.TModLoader;


namespace FindableManaCrystals.Tiles {
	public class GeothaumaticSurveyStationTile : ModTile {
		public override void SetDefaults() {
			Main.tileFrameImportant[ this.Type ] = true;
			Main.tileLavaDeath[ this.Type ] = false;
			Main.tileLighted[ this.Type ] = true;

			TileObjectData.newTile.CopyFrom( TileObjectData.Style3x3Wall );
			TileObjectData.newTile.StyleHorizontal = true;
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
			r = 0.25f;
			g = 0.25f;
			b = 0.25f;
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
				/*if( config.Get<bool>( nameof(config.GeothaumSurveyStationDropsItem ) ) ) {
					Item.NewItem( i * 16, j * 16, 64, 32, ModContent.ItemType<GeothaumaticSurveyStationTileItem>() );
				}*/
			}
		}


		////////////////

		public override void MouseOver( int i, int j ) {
			Main.LocalPlayer.showItemIcon = true;
			Main.LocalPlayer.showItemIcon2 = ModContent.ItemType<GeothaumaticSurveyStationTileItem>();

			var myplayer = TmlLibraries.SafelyGetModPlayer<FMCPlayer>( Main.LocalPlayer );
			if( myplayer.AddDiscoveredGeothaumStation( i, j ) ) {
				Main.NewText( "Geothaumatic Surveillance Station located!", Color.Lime );
			}
		}
	}
}
