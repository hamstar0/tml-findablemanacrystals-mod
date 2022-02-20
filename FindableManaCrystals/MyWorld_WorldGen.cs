using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.World;
using FindableManaCrystals.WorldGeneration;


namespace FindableManaCrystals {
	partial class FMCWorld : ModWorld {
		private GenPass GetManaShardWorldGenTask() {
			var config = FMCConfig.Instance;
			int shards;
			WorldSize wldSize = WorldLibraries.GetSize();

			switch( wldSize ) {
			default:
			case WorldSize.SubSmall:
				shards = config.Get<int>( nameof(FMCConfig.TinyWorldManaCrystalShards ) );
				break;
			case WorldSize.Small:
				shards = config.Get<int>( nameof(FMCConfig.SmallWorldManaCrystalShards) );
				break;
			case WorldSize.Medium:
				shards = config.Get<int>( nameof(FMCConfig.MediumWorldManaCrystalShards) );
				break;
			case WorldSize.Large:
				shards = config.Get<int>( nameof(FMCConfig.LargeWorldManaCrystalShards) );
				break;
			case WorldSize.SuperLarge:
				shards = config.Get<int>( nameof(FMCConfig.HugeWorldManaCrystalShards) );
				break;
			}

			return new ManaShardsWorldGenPass(shards);
		}

		private GenPass GetSurveyStationsWorldGenTask() {
			var config = FMCConfig.Instance;
			int stations;
			WorldSize wldSize = WorldLibraries.GetSize();

			switch( wldSize ) {
			default:
			case WorldSize.SubSmall:
				stations = config.Get<int>( nameof(FMCConfig.TinyWorldSurveyStations) );
				break;
			case WorldSize.Small:
				stations = config.Get<int>( nameof(FMCConfig.SmallWorldSurveyStations) );
				break;
			case WorldSize.Medium:
				stations = config.Get<int>( nameof(FMCConfig.MediumWorldSurveyStations) );
				break;
			case WorldSize.Large:
				stations = config.Get<int>( nameof(FMCConfig.LargeWorldSurveyStations) );
				break;
			case WorldSize.SuperLarge:
				stations = config.Get<int>( nameof(FMCConfig.HugeWorldSurveyStations) );
				break;
			}

			return new SurveyStationsWorldGenPass( stations );
		}
	}
}
