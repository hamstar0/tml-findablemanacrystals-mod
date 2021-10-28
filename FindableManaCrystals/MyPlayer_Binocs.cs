using System;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;


namespace FindableManaCrystals {
	partial class FindableManaCrystalsPlayer : ModPlayer {
		public const float MaxBinocZoom = 2.1f;



		////////////////

		private void UpdateForBinocs( bool isHoldingBinocs ) {
			this.UpdateForBinocs_Focus( isHoldingBinocs );

			if( isHoldingBinocs ) {
				if( this.ScanTickElapsed++ == 10 ) {
					this.ScanTickElapsed = 0;

					this.AnimateManaCrystalShardHintFxIf();
				}
			}
		}
		
		private void UpdateForBinocs_Focus( bool isHoldingBinocs ) {
			if( Main.mouseRight && isHoldingBinocs ) {
				if( !this.PreBinocZoomPercent.HasValue ) {
					this.PreBinocZoomPercent = Main.GameZoomTarget;
				}

				if( this.BinocZoomPercent < FindableManaCrystalsPlayer.MaxBinocZoom ) {
					this.BinocZoomPercent += 1f / 15f;
					if( this.BinocZoomPercent > FindableManaCrystalsPlayer.MaxBinocZoom ) {
						this.BinocZoomPercent = FindableManaCrystalsPlayer.MaxBinocZoom;
					}
				}
			} else {
				if( this.PreBinocZoomPercent.HasValue ) {
					if( this.BinocZoomPercent > this.PreBinocZoomPercent.Value ) {
						this.BinocZoomPercent -= 1f / 15f;
					} else {
						this.ResetBinocZoomIf();
					}
				}
			}
		}


		////////////////

		private void ApplyBinocZoomIf() {
			if( this.PreBinocZoomPercent.HasValue ) {
				Main.GameZoomTarget = this.BinocZoomPercent;
			}
		}


		////////////////

		internal void ResetBinocZoomIf() {
			if( !this.PreBinocZoomPercent.HasValue ) {
				return;
			}

			this.BinocZoomPercent = this.PreBinocZoomPercent.Value;

			this.PreBinocZoomPercent = null;

			//

			Main.GameZoomTarget = this.BinocZoomPercent;
		}
	}
}
