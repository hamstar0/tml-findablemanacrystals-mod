using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Tiles;
using HamstarHelpers.Classes.Tiles.TilePattern;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	public partial class FMCMod : Mod {
		public static void InitializePKE() {
			PKEMeter.Logic.PKEText meterTextFunc = PKEMeter.PKEMeterAPI.GetMeterText();
			PKEMeter.Logic.PKEGauge gauge = PKEMeter.PKEMeterAPI.GetGauge();
			float lastGaugedManaShardPercent = 0f;

			int gaugeTimer = 0;

			PKEMeter.PKEMeterAPI.SetGauge( (plr, pos) => {
				(float b, float g, float y, float r) existingGauge = gauge?.Invoke( plr, pos )
					?? (0f, 0f, 0f, 0f);

				if( FMCConfig.Instance.PKEDetectChancePerTick > 0f && gaugeTimer-- <= 0 ) {
					gaugeTimer = 15;
					lastGaugedManaShardPercent = FMCMod.GaugeManaShards( pos );
				}

				if( FMCConfig.Instance.PKEDetectChancePerTick <= Main.rand.NextFloat() ) {
					return existingGauge;
				}
				
				existingGauge.b = FMCMod.ApplyInterferenceToManaShardGauge( lastGaugedManaShardPercent );

				return existingGauge;
			} );

			PKEMeter.PKEMeterAPI.SetMeterText( ( plr, pos, gauges ) => {
				(string text, Color color) currText = meterTextFunc?.Invoke( plr, pos, gauges )
					?? ("", Color.Transparent);
				if( currText.text != "" ) {	// yield
					return currText;
				}

				if( gauges.b > 0.75f ) {
					currText.color = Color.Blue;
					currText.color = currText.color * (0.5f + (Main.rand.NextFloat() * 0.5f));
					currText.text = "CLASS II ETHEREAL GEOFORM";
				}

				return currText;
			} );
		}

		////

		public static float GaugeManaShards( Vector2 worldPos ) {
			var config = FMCConfig.Instance;
			int maxTileRange = config.Get<int>( nameof(config.ManaShardPKEDetectionTileRangeMax) );
			if( maxTileRange <= 0 ) {
				return 0f;
			}

			var pattern = new TilePattern( new TilePatternBuilder {
				IsActive = true,
				IsAnyOfType = new HashSet<int> { ModContent.TileType<ManaCrystalShardTile>() }
			} );
			int maxDist = maxTileRange * 16;

			Point? tileAt = TileFinderHelpers.GetNearestTile( worldPos, pattern, maxDist );
			if( !tileAt.HasValue ) {
				return 0f;
			}

			Vector2 worldPosAt = new Vector2( tileAt.Value.X * 16, tileAt.Value.Y * 16 );

			float distPerc = (worldPosAt - worldPos).Length() / (float)maxDist;
			return Math.Max( 1f - distPerc, 0f );
		}

		public static float ApplyInterferenceToManaShardGauge( float gaugedPercent ) {
			if( FMCConfig.Instance.PKEDetectInterference ) {
				gaugedPercent *= Main.rand.NextFloat();
			}

			return gaugedPercent;
		}
	}
}
