﻿using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using ModLibsCore.Classes.UI.ModConfig;


namespace FindableManaCrystals {
	class MyFloatInputElement : FloatInputElement { }




	public partial class FMCConfig : ModConfig {
		public static FMCConfig Instance => ModContent.GetInstance<FMCConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;



		////

		public bool DebugModeInfo { get; set; } = false;
		public bool DebugModeWorldGenInfo { get; set; } = false;
		public bool DebugModeCheatReveal { get; set; } = false;


		////

		[Range( 0, 100 )]
		[DefaultValue( 3 )]
		public int ManaCrystalShardsPerManaCrystal { get; set; } = 3;

		[Range( 10, 1000 )]
		[DefaultValue( 80 )]
		public int ManaCrystalShardTeleportRadius { get; set; } = 80;

		[Label("Mana Crystal Shard Light Tolerance (0 = unlimited)")]
		[Range( 0f, 1000f )]
		[DefaultValue( 0f )]
		[CustomModConfigItem( typeof(MyFloatInputElement) )]
		public float ManaCrystalShardLightToleranceScale { get; set; } = 0f;

		[Range( 0, 200 )]
		[DefaultValue( 9 )]
		public int ManaCrystalShardMagicResonanceTileRange { get; set; } = 9;

		[Range( 0.001f, 10f )]
		[DefaultValue( 0.04f )]
		[CustomModConfigItem( typeof(MyFloatInputElement) )]
		public float IlluminationDimRate { get; set; } = 0.04f;


		[Range( 0, 10000 )]
		[DefaultValue( 200 )]
		public int TinyWorldManaCrystalShards { get; set; } = 200;

		[Range( 0, 10000 )]
		[DefaultValue( 400 )]
		public int SmallWorldManaCrystalShards { get; set; } = 400;

		[Range( 0, 10000 )]
		[DefaultValue( 800 )]
		public int MediumWorldManaCrystalShards { get; set; } = 800;

		[Range( 0, 10000 )]
		[DefaultValue( 1600 )]
		public int LargeWorldManaCrystalShards { get; set; } = 1600;

		[Range( 0, 10000 )]
		[DefaultValue( 2400 )]
		public int HugeWorldManaCrystalShards { get; set; } = 2400;

		////

		[Range( 0, 1000 )]
		[DefaultValue( 16 )]
		public int TinyWorldSurveyStations { get; set; } = 20;    // SmallWorldPortals / 2

		[Range( 0, 1000 )]
		[DefaultValue( 28 )]
		public int SmallWorldSurveyStations { get; set; } = 28;  // 4200 x 1200 = 5040000

		[Range( 0, 1000 )]
		[DefaultValue( 54 )]
		public int MediumWorldSurveyStations { get; set; } = 54; // 6400 x 1800 = 11520000

		[Range( 0, 1000 )]
		[DefaultValue( 108 )]
		public int LargeWorldSurveyStations { get; set; } = 108;  // 8400 x 2400 = 20160000

		[Range( 0, 10000 )]
		[DefaultValue( 160 )]
		public int HugeWorldSurveyStations { get; set; } = 160;

		////

		[Range( 0, 10000 )]
		[DefaultValue( 128 )]
		public int MinimumSurveyStationTileSpacing { get; set; } = 128;

		////

		[DefaultValue( true )]
		public bool IsGeothaumSurveyStationBreakable { get; set; } = true;

		[DefaultValue( true )]
		public bool GeothaumSurveyStationDropsItem { get; set; } = true;

		////

		[DefaultValue( true )]
		public bool EnableGeothaumSurveyStationRecipe { get; set; } = true;

		////

		[DefaultValue( true )]
		public bool StartPlayersWithBinoculars { get; set; } = true;

		[Range( 10, 1000 )]
		[DefaultValue( 52 )]
		public int BinocularsDetectionRadiusTiles { get; set; } = 52;

		[Range( 1, 60 * 60 )]
		[DefaultValue( 20 )]
		public int BinocularsHintBeginDurationTicks { get; set; } = 20;

		[Range( 0f, 1f )]
		[DefaultValue( 0.7f )]	// was 0.6f
		[CustomModConfigItem( typeof(MyFloatInputElement) )]
		public float BinocularsHintIntensity { get; set; } = 0.7f;


		////

		[DefaultValue( true )]
		public bool ReducedManaCrystalStatIncrease { get; set; } = true;

		////

		[Range( 0f, 1f )]
		[DefaultValue( 1f / 28f )]	// was 1/32
		[CustomModConfigItem( typeof(MyFloatInputElement) )]
		public float PKEDetectChancePerTick { get; set; } = 1f / 28f;

		[DefaultValue( true )]
		public bool PKEDetectInterference { get; set; } = true;

		[Range( -1, 4048 )]
		[DefaultValue( 128 )]
		public int ManaShardPKEDetectionTileRangeMax { get; set; } = 128;



		////

		/*public override ModConfig Clone() {
			var clone = (AdventureModeConfig)base.Clone();
			return clone;
		}*/
	}
}
