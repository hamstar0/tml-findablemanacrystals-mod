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
			PKEMeter.Logic.PKEGauge gauge = PKEMeter.PKEMeterAPI.GetGauge();

			Point? lastNearestShardTile = null;
			float lastGaugedManaShardPercent = 0f;
			int gaugeTimer = 0;

			PKEMeter.PKEMeterAPI.SetGauge( (plr, pos) => {
				var config = FMCConfig.Instance;
				float detectChancePerTick = config.Get<float>( nameof(config.PKEDetectChancePerTick) );

				(float b, float g, float y, float r) existingGauge = gauge?.Invoke( plr, pos )
					?? (0f, 0f, 0f, 0f);

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
					existingGauge.b = lastGaugedManaShardPercent;
					existingGauge.b += (1f - lastGaugedManaShardPercent) * Math.Min(illumAmt, 1f);
				} else {
					if( detectChancePerTick > Main.rand.NextFloat() ) {
						existingGauge.b = FMCMod.ApplyInterferenceToManaShardGauge( lastGaugedManaShardPercent );
					}
				}

				return existingGauge;
			} );

			PKEMeter.PKEMeterAPI.SetMeterText( "FindableManaCrystals", ( plr, pos, gauges ) => {
				return new PKEMeter.Logic.PKETextMessage(
					message: "CLASS II ETHEREAL GEOFORM",
					color: Color.Blue * ( 0.5f + ( Main.rand.NextFloat() * 0.5f ) ),
					priority: lastGaugedManaShardPercent * 0.99999f
				);
			} );
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

			nearestShardTile = TileFinderHelpers.GetNearestTile( worldPos, pattern, maxDist );
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
