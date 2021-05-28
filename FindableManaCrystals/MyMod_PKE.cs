using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsTiles.Classes.Tiles.TilePattern;
using ModLibsTiles.Libraries.Tiles;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	public partial class FMCMod : Mod {
		public static void InitializePKE() {
			PKEMeter.Logic.PKEGaugesGetter gauge = PKEMeter.PKEMeterAPI.GetGauge();

			Point? lastNearestShardTile = null;
			float lastGaugedManaShardPercent = 0f;
			int gaugeTimer = 0;

			PKEMeter.PKEMeterAPI.SetGauge( (plr, pos) => {
				var config = FMCConfig.Instance;
				float detectChancePerTick = config.Get<float>( nameof(config.PKEDetectChancePerTick) );

				PKEMeter.Logic.PKEGaugeValues existingGauge = gauge?.Invoke( plr, pos )
					?? new PKEMeter.Logic.PKEGaugeValues( 0f, 0f, 0f, 0f);

				if( gaugeTimer-- <= 0 ) {
					gaugeTimer = 15;
					lastGaugedManaShardPercent = FMCMod.GaugeNearbyManaShards( pos, out lastNearestShardTile );
				}

				float illumAmt = 0;
				if( lastNearestShardTile.HasValue ) {
					ManaCrystalShardTile.GetIlluminationAt(
						lastNearestShardTile.Value.X,
						lastNearestShardTile.Value.Y,
						out illumAmt
					);
				}

				if( illumAmt > 0f ) {
					existingGauge.BluePercent = lastGaugedManaShardPercent;
					existingGauge.BluePercent += (1f - lastGaugedManaShardPercent) * Math.Min(illumAmt, 1f);
				} else {
					if( detectChancePerTick > Main.rand.NextFloat() ) {
						existingGauge.BluePercent = FMCMod.ApplyInterferenceToManaShardGauge( lastGaugedManaShardPercent );
					}
				}

				return existingGauge;
			} );

			PKEMeter.PKEMeterAPI.SetMeterText( "FindableManaCrystals", ( plr, pos, gauges ) => {
				Color color = new Color( 32, 64, 255 );
				color = color * (0.5f + (Main.rand.NextFloat() * 0.5f));

				return new PKEMeter.Logic.PKETextMessage(
					message: "CLASS II ETHEREAL GEOFORM",
					color: color,
					priority: lastGaugedManaShardPercent * 0.99999f
				);
			} );

			PKEMeter.PKEMeterAPI.SetPKEBlueTooltip( () => "GEOFORM" );
		}


		////////////////

		public static float GaugeNearbyManaShards( Vector2 worldPos, out Point? nearestShardTile ) {
			var config = FMCConfig.Instance;
			int maxTileRange = config.Get<int>( nameof(config.ManaShardPKEDetectionTileRangeMax) );
			if( maxTileRange <= 0 ) {
				nearestShardTile = null;
				return 0f;
			}

			var pattern = new TilePattern( new TilePatternBuilder {
				IsActive = true,
				IsAnyOfType = new HashSet<int> { ModContent.TileType<ManaCrystalShardTile>() }
			} );
			int maxDist = maxTileRange * 16;

			nearestShardTile = TileFinderLibraries.GetNearestTile( worldPos, pattern, maxDist );
			if( !nearestShardTile.HasValue ) {
				return 0f;
			}

			Vector2 worldPosAt = new Vector2( nearestShardTile.Value.X * 16, nearestShardTile.Value.Y * 16 );

			float distPerc = (worldPosAt - worldPos).Length() / (float)maxDist;
			return Math.Max( 1f - distPerc, 0f );
		}


		////////////////

		public static float ApplyInterferenceToManaShardGauge( float gaugedPercent ) {
			var config = FMCConfig.Instance;

			if( config.Get<bool>( nameof(config.PKEDetectInterference) ) ) {
				gaugedPercent *= Main.rand.NextFloat();
			}

			return gaugedPercent;
		}
	}
}
