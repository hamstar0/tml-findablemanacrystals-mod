using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.UI;


namespace FindableManaCrystals {
	partial class FindableManaCrystalsPlayer : ModPlayer {
		public const float MaxBinocZoom = 2.1f;
		public const float BinocZoomRate = 1f / 15f;



		////////////////

		private void UpdateForBinocs( bool isHoldingBinocs ) {
			this.UpdateForBinocs_ZoomStateForFocus( isHoldingBinocs );

			if( isHoldingBinocs ) {
				if( this.IsBinocFocus ) {
					this.UpdateForBinocs_Focus_LightIf();
				}

				if( this.ScanTickElapsed++ == 10 ) {
					this.ScanTickElapsed = 0;

					this.AnimateManaCrystalShardHintFxIf();
				}
			}
		}
		
		private void UpdateForBinocs_ZoomStateForFocus( bool isHoldingBinocs ) {
			if( Main.mouseRight && isHoldingBinocs ) {
				if( !this.PreBinocZoomPercent.HasValue ) {
					this.PreBinocZoomPercent = Main.GameZoomTarget;
				}

				if( this.BinocZoomPercent < FindableManaCrystalsPlayer.MaxBinocZoom ) {
					this.BinocZoomPercent += FindableManaCrystalsPlayer.BinocZoomRate;
					if( this.BinocZoomPercent > FindableManaCrystalsPlayer.MaxBinocZoom ) {
						this.BinocZoomPercent = FindableManaCrystalsPlayer.MaxBinocZoom;
					}
				}
			} else {
				if( this.PreBinocZoomPercent.HasValue ) {
					if( this.BinocZoomPercent > this.PreBinocZoomPercent.Value ) {
						this.BinocZoomPercent -= FindableManaCrystalsPlayer.BinocZoomRate;
					} else {
						this.ResetBinocZoomIf();
					}
				}
			}
		}
		
		private void UpdateForBinocs_Focus_LightIf() {
			var config = FMCConfig.Instance;

			float chance = config.Get<float>( nameof(config.BinocularsFocusModeLightChancePerTick) );
			if( Main.rand.NextFloat() > chance ) {
				return;
			}

			float illum = config.Get<float>( nameof(config.BinocularsFocusModeLightIntensity) );

			Rectangle scr = UIZoomLibraries.GetWorldFrameOfScreen( null, true );
			int tileX = (scr.X + Main.rand.Next(scr.Width)) / 16;
			int tileY = (scr.Y + Main.rand.Next(scr.Height)) / 16;

			Lighting.AddLight(
				i: tileX,
				j: tileY,
				R: illum,
				G: illum,
				B: illum
			);
		}


		////////////////

		private void ApplyBinocZoomIf() {
			if( this.IsBinocFocus ) {
				Main.GameZoomTarget = this.BinocZoomPercent;
			}
		}


		////////////////

		internal void ResetBinocZoomIf() {
			if( !this.IsBinocFocus ) {
				return;
			}

			this.BinocZoomPercent = this.PreBinocZoomPercent.Value;

			this.PreBinocZoomPercent = null;

			//

			Main.GameZoomTarget = this.BinocZoomPercent;
		}
	}
}
