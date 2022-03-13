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

			//

			PKEMeter.PKEMeterAPI.SetGauge( (plr, pos) => {
				PKEMeter.Logic.PKEGaugeValues existingGauge = gauge?.Invoke( plr, pos )
					?? new PKEMeter.Logic.PKEGaugeValues( 0f, 0f, 0f, 0f);

				// Update gauge every 1/4s
				if( gaugeTimer-- <= 0 ) {
					gaugeTimer = 15;

					lastGaugedManaShardPercent = FMCMod.GaugeNearbyManaShards( pos, out lastNearestShardTile );
				}

				existingGauge.BluePercent = FMCMod.ComputePKEGauge(lastGaugedManaShardPercent, lastNearestShardTile)
					?? existingGauge.BluePercent;

				return existingGauge;
			} );

			//

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
		
		public static float? ComputePKEGauge( float priorGaugedAmount, Point? nearestShardTile ) {
			float? gauge = null;

			float tileIllumAmt = 0;
			if( nearestShardTile.HasValue ) {
				ManaCrystalShardTile.GetIlluminationAt(
					nearestShardTile.Value.X,
					nearestShardTile.Value.Y,
					out tileIllumAmt
				);
			}
			
			// If tile is lit up, report gauged amount directly
			if( tileIllumAmt > 0f ) {
				gauge = priorGaugedAmount;
				gauge += (1f - priorGaugedAmount) * Math.Min(tileIllumAmt, 1f);
			}
			// Otherwise, infrequently report gauged amount
			else {
				var config = FMCConfig.Instance;
				float detectChancePerTick = config.Get<float>( nameof(config.PKEDetectChancePerTick) );

				// Show detected amount only occasionally (default 1/25 chance per tick), and with heavy 'interference'
				if( detectChancePerTick > Main.rand.NextFloat() ) {
					gauge = FMCMod.ApplyInterferenceToManaShardGauge( priorGaugedAmount );
				}
			}

			return gauge;
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

			if( config.Get<bool>(nameof(config.PKEDetectInterference)) ) {
				float perc = Main.rand.NextFloat();
				perc *= perc;
				perc = 1f - perc;

				gaugedPercent *= perc;
			}

			return gaugedPercent;
		}
	}
}
