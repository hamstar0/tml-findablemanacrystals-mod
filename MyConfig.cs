using HamstarHelpers.Classes.UI.ModConfig;
using HamstarHelpers.Services.Configs;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace FindableManaCrystals {
	class MyFloatInputElement : FloatInputElement { }




	public class FindableManaCrystalsConfig : StackableModConfig {
		public static FindableManaCrystalsConfig Instance => ModConfigStack.GetMergedConfigs<FindableManaCrystalsConfig>();



		////////////////

		public override ConfigScope Mode => ConfigScope.ServerSide;



		////

		public bool DebugModeInfo { get; set; } = false;
		public bool DebugModeCheatReveal { get; set; } = false;


		////

		[Range( 0, 100 )]
		[DefaultValue( 4 )]
		public int ManaCrystalShardsPerManaCrystal { get; set; } = 4;

		[Range( 10, 1000 )]
		[DefaultValue( 80 )]
		public int ManaCrystalShardTeleportRadius { get; set; } = 80;

		[Range( 0.01f, 300f )]
		[DefaultValue( 60f )]
		public float ManaCrystalShardLightToleranceScale { get; set; } = 60f;

		[Range( 0, 100 )]
		[DefaultValue( 10 )]
		public int ManaCrystalShardMagicResonanceTileRange { get; set; } = 10;

		[Range( 0.001f, 10f )]
		[DefaultValue( 0.04f )]
		public float IlluminationDimRate { get; set; } = 0.04f;


		[Range( 0, 10000 )]
		[DefaultValue( 160 )]
		public int TinyWorldManaCrystalShards { get; set; } = 192;

		[Range( 0, 10000 )]
		[DefaultValue( 192 )]
		public int SmallWorldManaCrystalShards { get; set; } = 384;

		[Range( 0, 10000 )]
		[DefaultValue( 384 )]
		public int MediumWorldManaCrystalShards { get; set; } = 768;

		[Range( 0, 10000 )]
		[DefaultValue( 576 )]
		public int LargeWorldManaCrystalShards { get; set; } = 1536;

		[Range( 0, 10000 )]
		[DefaultValue( 768 )]
		public int HugeWorldManaCrystalShards { get; set; } = 2304;

		////

		[DefaultValue( true )]
		public bool StartPlayersWithBinoculars { get; set; } = true;



		////

		/*public override ModConfig Clone() {
			var clone = (AdventureModeConfig)base.Clone();
			return clone;
		}*/
	}
}
