using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.UI;
using FindableManaCrystals.Tiles;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		public const float MaxBinocZoom = 2.1f;
		public const float BinocZoomRate = 1f / 15f;



		////////////////

		private void UpdateForShardViewing( bool isHoldingBinocs ) {
			bool isFocusingBinocs = this.UpdateForBinocs_ZoomStateForFocus( isHoldingBinocs );

			if( isFocusingBinocs ) {
				this.UpdateForBinocs_FocusIllum();
			}

			if( isHoldingBinocs || this.IsNearSurveyStation ) {
				if( this.IsBinocFocus || this.IsNearSurveyStation ) {
					this.UpdateForShardViewing_Focus_LightIf();
				}

				if( this.ScanTickElapsed++ == 10 ) {
					this.ScanTickElapsed = 0;

					this.AnimateManaCrystalShardHintFxIf();
				}
			}
		}
		
		private bool UpdateForBinocs_ZoomStateForFocus( bool isHoldingBinocs ) {
			bool isFocusing = Main.mouseLeft && isHoldingBinocs;

			if( isFocusing ) {
				if( !this.PreBinocZoomPercent.HasValue ) {
					this.PreBinocZoomPercent = Main.GameZoomTarget;
				}

				if( this.BinocZoomPercent < FMCPlayer.MaxBinocZoom ) {
					this.BinocZoomPercent += FMCPlayer.BinocZoomRate;
					if( this.BinocZoomPercent > FMCPlayer.MaxBinocZoom ) {
						this.BinocZoomPercent = FMCPlayer.MaxBinocZoom;
					}
				}
			} else {
				if( this.PreBinocZoomPercent.HasValue ) {
					if( this.BinocZoomPercent > this.PreBinocZoomPercent.Value ) {
						this.BinocZoomPercent -= FMCPlayer.BinocZoomRate;
					} else {
						this.ResetBinocZoomIf();
					}
				}
			}

			return isFocusing;
		}

		private void UpdateForBinocs_FocusIllum() {
			Vector2 wldScrPos = Main.screenPosition;
			wldScrPos.X += Main.screenWidth / 2;
			wldScrPos.Y += Main.screenHeight / 2;

			int tileX = (int)wldScrPos.X / 16;
			int tileY = (int)wldScrPos.Y / 16;

			//

			ManaCrystalShardTile.GetIlluminationAt( tileX, tileY, out float illum );

			if( illum <= 0f ) {
				ManaCrystalShardTile.SetIlluminationAt( tileX, tileY, 0.9f );
			} else if( illum <= 0.3f ) {
				ManaCrystalShardTile.SetIlluminationAt( tileX, tileY, 0.3f );
			}
		}

		////
		
		private void UpdateForShardViewing_Focus_LightIf() {
			float illum = 0.065f;
			Rectangle scr = UIZoomLibraries.GetWorldFrameOfScreen( null, true );
			int tileX = scr.Center.X / 16;  //(scr.X + Main.rand.Next(scr.Width)) / 16;
			int tileY = scr.Center.Y / 16; //(scr.Y + Main.rand.Next(scr.Height)) / 16;
			int rad = 4;
			int minX = tileX - 4;
			int maxX = tileX + 4;
			int minY = tileY - 4;
			int maxY = tileY + 4;
			
			for( int i=minX; i<maxX; i++ ) {
				int diffX = i - tileX;

				for( int j=minY; j<maxY; j++ ) {
					int diffY = j - tileY;
					int distSqr = ( diffX * diffX ) + ( diffY * diffY );
					if( distSqr > rad * rad ) {
						continue;
					}

					Lighting.AddLight( i, j, illum, illum, illum );
				}
			}
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
