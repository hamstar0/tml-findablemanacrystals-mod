using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ModLibsCore.Libraries.Debug;
using ModLibsCore.Libraries.TModLoader;
using ModLibsGeneral.Libraries.Tiles;
using FindableManaCrystals.Items;


namespace FindableManaCrystals.Tiles {
	public partial class ManaCrystalShardTile : ModTile {
		public const int AnimationFrameWidth = 18;



		////////////////

		public static short PickFrameX( int i, int j ) {
			return (short)( TmlLibraries.SafelyGetRand().Next(3) * ManaCrystalShardTile.AnimationFrameWidth );
		}

		public static short PickFrameY( int i, int j ) {
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

		public override void SetDefaults() {
			Main.tileLighted[ this.Type ] = true;
			//Main.tileValue[ this.Type ] = 790;	// just below life crystals
			Main.tileFrameImportant[ this.Type ] = true;
			Main.tileNoAttach[ this.Type ] = true;
			this.dustType = DustID.BlueCrystalShard;
			
			//TileObjectData.newTile.CopyFrom( TileObjectData.GetTileData(TileID.Crystals, 0) );
			TileObjectData.newTile.CopyFrom( TileObjectData.Style1x1 );
			TileObjectData.addTile( this.Type );

			//ModTranslation name = this.CreateMapEntryName();
			//name.SetDefault( "Mana Crystal Shard" );
			//this.AddMapEntry( new Color(32, 48, 160, 0), name );
		}

		public override void PostSetDefaults() {
			Main.tileObsidianKill[this.Type] = false;
		}

		////////////////

		public override bool CanPlace( int i, int j ) {
			int frameY = ManaCrystalShardTile.PickFrameY( i, j );
			return frameY != -1;
		}

		public override void PlaceInWorld( int i, int j, Item item ) {
			Tile tile = Main.tile[i, j];
			tile.frameX = ManaCrystalShardTile.PickFrameX( i, j );
			tile.frameY = ManaCrystalShardTile.PickFrameY( i, j );
		}

		////

		public override bool TileFrame( int i, int j, ref bool resetFrame, ref bool noBreak ) {
			Tile tile = Main.tile[i, j];
			short frameY = ManaCrystalShardTile.PickFrameY( i, j );

			if( tile.frameY != frameY ) {
				if( frameY == -1 ) {
					TileLibraries.KillTileSynced( i, j, false, true, true );
				} else {
					tile.frameY = frameY;
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
			ManaCrystalShardTile.RemoveIlluminationAt( i, j );

			if( !fail && !effectOnly && !noItem ) {
				Item.NewItem( (i << 4), (j << 4), 16, 16, ModContent.ItemType<ManaCrystalShardItem>() );
			}
		}


		////////////////

		private void UpdateDrawnTileSlow( int i, int j ) {
			var config = FMCConfig.Instance;
			var projSingleton = ModContent.GetInstance<FMCProjectile>();

			string manaTileEntry = nameof( config.ManaCrystalShardMagicResonanceTileRange );
			int resonanceDistSqr = config.Get<int>( manaTileEntry ) * 16;
			resonanceDistSqr *= resonanceDistSqr;

			int tileWldX = i * 16;
			int tileWldY = j * 16;

			foreach( int projWho in projSingleton.GetAllMagicProjectiles() ) {
				Vector2 pos = Main.projectile[projWho].Center;
				int distX = ((int)pos.X) - tileWldX;
				int distY = ((int)pos.Y) - tileWldY;

				int distSqr = (distX*distX) + (distY*distY);
				if( distSqr < resonanceDistSqr ) {
					ManaCrystalShardTile.SetIlluminationAt( i, j, 1f );
					break;
				}
			}

			if( config.DebugModeCheatReveal ) {
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
