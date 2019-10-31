using HamstarHelpers.Classes.UI.ModConfig;
using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace FindableManaCrystals {
	class MyFloatInputElement : FloatInputElement { }




	public class FindableManaCrystalsConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;



		////

		public bool DebugModeInfo { get; set; } = false;


		////

		[Range( 0, 100 )]
		[DefaultValue( 4 )]
		public int ManaCrystalShardsPerManaCrystal { get; set; } = 4;

		[Range( 10, 1000 )]
		[DefaultValue( 80 )]
		public int ManaCrystalShardTeleportRadius { get; set; } = 80;

		[Range( 0.01f, 100f )]
		[DefaultValue( 20f )]
		public float ManaCrystalShardLightToleranceScale { get; set; } = 20f;

		[Range( 0, 100 )]
		[DefaultValue( 10 )]
		public int ManaCrystalShardMagicResonanceTileRange { get; set; } = 10;


		[Range( 0, 10000 )]
		[DefaultValue( 160 )]
		public int TinyWorldManaCrystalShards { get; set; } = 160;

		[Range( 0, 10000 )]
		[DefaultValue( 192 )]
		public int SmallWorldManaCrystalShards { get; set; } = 192;

		[Range( 0, 10000 )]
		[DefaultValue( 384 )]
		public int MediumWorldManaCrystalShards { get; set; } = 384;

		[Range( 0, 10000 )]
		[DefaultValue( 576 )]
		public int LargeWorldManaCrystalShards { get; set; } = 576;

		[Range( 0, 10000 )]
		[DefaultValue( 768 )]
		public int HugeWorldManaCrystalShards { get; set; } = 768;



		////

		/*public override ModConfig Clone() {
			var clone = (AdventureModeConfig)base.Clone();
			return clone;
		}*/
	}
}
