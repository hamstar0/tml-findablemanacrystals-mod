using System;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using HamstarHelpers.Classes.UI.ModConfig;


namespace FindableManaCrystals {
	class MyFloatInputElement : FloatInputElement { }




	public partial class FMCConfig : ModConfig {
		public static FMCConfig Instance { get; internal set; }



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;



		////

		public bool DebugModeInfo { get; set; } = false;
		public bool DebugModeWorldGenInfo { get; set; } = false;
		public bool DebugModeCheatReveal { get; set; } = false;


		////

		[Range( 0, 100 )]
		[DefaultValue( 4 )]
		public int ManaCrystalShardsPerManaCrystal { get; set; } = 4;

		[Range( 10, 1000 )]
		[DefaultValue( 80 )]
		public int ManaCrystalShardTeleportRadius { get; set; } = 80;

		[Label("Mana Crystal Shard Light Tolerance (0 = unlimited)")]
		[Range( 0f, 1000f )]
		[DefaultValue( 0f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float ManaCrystalShardLightToleranceScale { get; set; } = 0f;

		[Range( 0, 200 )]
		[DefaultValue( 9 )]
		public int ManaCrystalShardMagicResonanceTileRange { get; set; } = 9;

		[Range( 0.001f, 10f )]
		[DefaultValue( 0.04f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
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

		[DefaultValue( true )]
		public bool StartPlayersWithBinoculars { get; set; } = true;

		[Range( 10, 1000 )]
		[DefaultValue( 84 )]
		public int BinocularDetectionRadiusTiles { get; set; } = 84;

		[Range( 1, 60 * 60 )]
		[DefaultValue( 30 )]
		public int BinocularsHintBeginDurationTicks { get; set; } = 30;

		[Range( 0f, 1f )]
		[DefaultValue( 0.6f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float BinocularsHintIntensity { get; set; } = 0.6f;

		////

		[DefaultValue( true )]
		public bool ReducedManaCrystalStatIncrease { get; set; } = true;

		////

		[Range( 0f, 1f )]
		[DefaultValue( 1f / 60f )]
		[CustomModConfigItem( typeof( MyFloatInputElement ) )]
		public float PKEDetectChancePerTick { get; set; } = 1f / 60f;

		[DefaultValue( true )]
		public bool PKEDetectInterference { get; set; } = true;

		[Range( -1, 4048 )]
		[DefaultValue( 256 )]
		public int ManaShardPKEDetectionTileRangeMax { get; set; } = 256;



		////

		/*public override ModConfig Clone() {
			var clone = (AdventureModeConfig)base.Clone();
			return clone;
		}*/
	}
}
