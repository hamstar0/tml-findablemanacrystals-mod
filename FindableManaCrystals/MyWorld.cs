using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.World;
using HamstarHelpers.Classes.Tiles.TilePattern;
using FindableManaCrystals.Tiles;
using FindableManaCrystals.NetProtocols;


namespace FindableManaCrystals {
	class FindableManaCrystalsWorld : ModWorld {
		internal static void InitializeSingleton() {
			var myworld = ModContent.GetInstance<FindableManaCrystalsWorld>();

			myworld.ManaCrystalShardPattern = new TilePattern( new TilePatternBuilder {
				HasSolidProperties = false,
				IsActuated = false,
				IsPlatform = false,
				HasLava = false,
				HasWire1 = false,
				HasWire2 = false,
				HasWire3 = false,
				HasWire4 = false,
				MaximumBrightness = 0.05f,
				CustomCheck = ( x, y ) => {
					if( Main.tile[x, y].type == ModContent.TileType<ManaCrystalShardTile>() ) {
						return false;
					}
					return ManaCrystalShardTile.PickFrameY( x, y ) != -1;
				}
			} );
		}



		////////////////

		private IDictionary<int, int> ManaCrystalShardIllumChecked = new Dictionary<int, int>();	// Technically incorrect; unimportant

		private (int tileX, int tileY)[] ManaCrystalShardIllumCheckQueue = new (int, int)[1];
		private int ManaCrystalShardIllumCheckQueueSize = 0;


		////////////////

		public TilePattern ManaCrystalShardPattern { get; private set; }



		////////////////

		public override void ModifyWorldGenTasks( List<GenPass> tasks, ref float totalWeight ) {
			int shards;
			WorldSize wldSize = WorldHelpers.GetSize();

			switch( wldSize ) {
			default:
			case WorldSize.SubSmall:
				shards = FindableManaCrystalsConfig.Instance.TinyWorldManaCrystalShards;
				break;
			case WorldSize.Small:
				shards = FindableManaCrystalsConfig.Instance.SmallWorldManaCrystalShards;
				break;
			case WorldSize.Medium:
				shards = FindableManaCrystalsConfig.Instance.MediumWorldManaCrystalShards;
				break;
			case WorldSize.Large:
				shards = FindableManaCrystalsConfig.Instance.LargeWorldManaCrystalShards;
				break;
			case WorldSize.SuperLarge:
				shards = FindableManaCrystalsConfig.Instance.HugeWorldManaCrystalShards;
				break;
			}

			tasks.Add( new FindableManaCrystalsWorldGenPass( shards ) );
		}


		////////////////

		public override void PreUpdate() {
			if( this.ManaCrystalShardIllumCheckQueueSize > 0 ) {
				this.ProcessManaCrystalShardQueue();
			}
		}


		////////////////

		public void QueueManaCrystalShardCheck( int tileX, int tileY, float brightness ) {
			if( Main.netMode == 1 ) {
				ManaCrystalShardCheckProtocol.QuickRequest( tileX, tileY, brightness );
			} else {
				if( this.ManaCrystalShardIllumCheckQueueSize == (this.ManaCrystalShardIllumCheckQueue.Length - 1) ) {
					Array.Resize( ref this.ManaCrystalShardIllumCheckQueue, this.ManaCrystalShardIllumCheckQueue.Length * 2 );
				}
				this.ManaCrystalShardIllumCheckQueue[ this.ManaCrystalShardIllumCheckQueueSize++ ] = (tileX, tileY);
			}
		}

		private void ProcessManaCrystalShardQueue() {
			int shardType = ModContent.TileType<ManaCrystalShardTile>();

			for( int i = 0; i < this.ManaCrystalShardIllumCheckQueueSize; i++ ) {
				(int tileX, int tileY) tileAt = this.ManaCrystalShardIllumCheckQueue[i];

				if( this.ManaCrystalShardIllumChecked.ContainsKey( tileAt.tileX ) && this.ManaCrystalShardIllumChecked[tileAt.tileX] == tileAt.tileY ) {
					continue;
				}
				this.ManaCrystalShardIllumChecked[tileAt.tileX] = tileAt.tileY;

				Tile tile = Framing.GetTileSafely( tileAt.tileX, tileAt.tileY );
				float brightness = Lighting.Brightness( tileAt.tileX, tileAt.tileY );

				if( tile.active() && tile.type == shardType ) {
					ManaCrystalShardTile.UpdateLightAversionForTile(
						tileAt.tileX,
						tileAt.tileY,
						FindableManaCrystalsConfig.Instance.ManaCrystalShardLightToleranceScale,
						brightness
					);
				}
			}

			this.ManaCrystalShardIllumChecked.Clear();
			this.ManaCrystalShardIllumCheckQueueSize = 0;
		}
	}
}
