using FindableManaCrystals.Items;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.DotNET.Extensions;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Helpers.TModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		private static int AnimationFrameWidth = 18;



		////////////////

		public static short PredictFrameY( int i, int j ) {
			Tile uTile = Main.tile[i, j - 1];
			Tile dTile = Main.tile[i, j + 1];
			Tile lTile = Main.tile[i - 1, j];
			Tile rTile = Main.tile[i + 1, j];
			int uTileType = -1;
			int dTileType = -1;
			int lTileType = -1;
			int rTileType = -1;

			if( uTile != null && uTile.nactive() && !uTile.bottomSlope() ) {
				uTileType = (int)uTile.type;
			} else if( dTile != null && dTile.nactive() && !dTile.halfBrick() && !dTile.topSlope() ) {
				dTileType = (int)dTile.type;
			} else if( lTile != null && lTile.nactive() ) {
				lTileType = (int)lTile.type;
			} else if( rTile != null && rTile.nactive() ) {
				rTileType = (int)rTile.type;
			}

			if( dTileType >= 0 && Main.tileSolid[dTileType] && !Main.tileSolidTop[dTileType] ) {
				return 0;
			} else if( lTileType >= 0 && Main.tileSolid[lTileType] && !Main.tileSolidTop[lTileType] ) {
				return (short)(ManaCrystalShardTile.AnimationFrameWidth * 3);
			} else if( rTileType >= 0 && Main.tileSolid[rTileType] && !Main.tileSolidTop[rTileType] ) {
				return (short)(ManaCrystalShardTile.AnimationFrameWidth * 2);
			} else if( uTileType >= 0 && Main.tileSolid[uTileType] && !Main.tileSolidTop[uTileType] ) {
				return (short)ManaCrystalShardTile.AnimationFrameWidth;
			} else {
				return -1;
			}
		}


		////////////////

		internal static void InitializeSingleton() {
			var instance = ModContent.GetInstance<ManaCrystalShardTile>();
			instance.IlluminatedCrystals = new Dictionary<int, IDictionary<int, float>>();
		}



		////////////////

		internal IDictionary<int, IDictionary<int, float>> IlluminatedCrystals;



		////////////////

		public override void SetDefaults() {
			Main.tileLighted[ this.Type ] = true;
			Main.tileValue[ this.Type ] = 790;	// just below life crystals
			Main.tileFrameImportant[ this.Type ] = true;
			Main.tileObsidianKill[ this.Type ] = false;
			Main.tileNoAttach[ this.Type ] = true;

			TileObjectData.newTile.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.addTile( this.Type );
			
			//ModTranslation name = this.CreateMapEntryName();
			//name.SetDefault( "Mana Crystal Shard" );
			//this.AddMapEntry( new Color( 238, 145, 105 ), name );
		}

		////////////////

		public override bool CanPlace( int i, int j ) {
			int frameY = ManaCrystalShardTile.PredictFrameY( i, j );
			return frameY != -1;
		}

		public override void PlaceInWorld( int i, int j, Item item ) {
			Tile tile = Main.tile[i, j];
			tile.frameX = (short)(TmlHelpers.SafelyGetRand().Next(3) * ManaCrystalShardTile.AnimationFrameWidth);
			tile.frameY = ManaCrystalShardTile.PredictFrameY( i, j );
		}

		////

		public override bool TileFrame( int i, int j, ref bool resetFrame, ref bool noBreak ) {
			short frameY = ManaCrystalShardTile.PredictFrameY( i, j );

			if( Main.tile[i,j].frameY != frameY ) {
				if( frameY == -1 ) {
					TileHelpers.KillTileSynced( i, j, false, true );
				} else {
					Main.tile[i, j].frameY = frameY;
					resetFrame = true;
				}
			}

			return false;
		}


		////////////////

		public override bool KillSound( int i, int j ) {
			Main.PlaySound( SoundID.Item27, i * 16, j * 16 );
			return false;
		}

		public override void KillTile( int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem ) {
			var singleton = ModContent.GetInstance<ManaCrystalShardTile>();
			singleton.IlluminatedCrystals.Remove2D( i, j );

			Item.NewItem( (i << 4), (j << 4), 16, 16, ModContent.ItemType<ManaCrystalShardItem>() );
		}


		////////////////

		private void UpdateDrawnTileSlow( int i, int j ) {
			var projSingleton = ModContent.GetInstance<FindableManaCrystalsProjectile>();
			int resonanceDistSqr = FindableManaCrystalsConfig.Instance.ManaCrystalShardMagicResonanceTileRange * 16;
			resonanceDistSqr *= resonanceDistSqr;

			int tileWldX = i << 4;
			int tileWldY = j << 4;

			foreach( int projWho in projSingleton.GetAllMagicProjectiles() ) {
				Vector2 pos = Main.projectile[projWho].Center;
				int distX = ((int)pos.X) - tileWldX;
				int distY = ((int)pos.Y) - tileWldY;

				int distSqr = (distX*distX) + (distY*distY);
				if( distSqr < resonanceDistSqr ) {
					this.SetIlluminateAt( i, j );
					break;
				}
			}

			if( FindableManaCrystalsConfig.Instance.DebugModeCheatReveal ) {
				var pos = new Vector2( (i << 4) + 8, (j << 4) + 8 );
				Dust dust = Dust.NewDustPerfect(
					Position: pos,
					Type: 61,
					Velocity: default(Vector2),
					Alpha: 0,
					newColor: Color.Blue,
					Scale: 3f
				);
				dust.noGravity = true;
				dust.noLight = true;
			}
		}
	}
}
