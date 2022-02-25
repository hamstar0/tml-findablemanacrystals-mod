﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ModLibsCore.Libraries.Debug;
using ModLibsGeneral.Libraries.UI;


namespace FindableManaCrystals {
	partial class FMCPlayer : ModPlayer {
		public const float MaxBinocZoom = 2.1f;
		public const float BinocZoomRate = 1f / 15f;



		////////////////

		private void UpdateForShardViewing( bool isHoldingBinocs, bool isNearSurveyStation ) {
			this.UpdateForBinocs_ZoomStateForFocus( isHoldingBinocs );

			if( isHoldingBinocs || isNearSurveyStation ) {
				if( this.IsBinocFocus || isNearSurveyStation ) {
					this.UpdateForShardViewing_Focus_LightIf();
				}

				if( this.ScanTickElapsed++ == 10 ) {
					this.ScanTickElapsed = 0;

					this.AnimateManaCrystalShardHintFxIf( isNearSurveyStation );
				}
			}
		}
		
		private void UpdateForBinocs_ZoomStateForFocus( bool isHoldingBinocs ) {
			if( Main.mouseLeft && isHoldingBinocs ) {
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
