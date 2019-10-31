using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;


namespace FindableManaCrystals {
	class FindableManaCrystalsProjectile : GlobalProjectile {
		internal static void InitializeSingleton() {
			var projSingleton = ModContent.GetInstance<FindableManaCrystalsProjectile>();
			projSingleton.MagicProjectiles = new HashSet<int>();
		}



		////////////////

		private bool IsAccountedFor = false;
		private ISet<int> MagicProjectiles = null;


		////////////////

		public override bool CloneNewInstances => false;
		public override bool InstancePerEntity => true;



		////////////////

		private void Initialize( Projectile projectile ) {
			if( projectile.magic ) {
				var projSingleton = ModContent.GetInstance<FindableManaCrystalsProjectile>();
				projSingleton.MagicProjectiles.Add( projectile.whoAmI );
			}
		}


		////////////////

		public IEnumerable<int> GetAllMagicProjectiles() {
			var projWhos = new List<int>( this.MagicProjectiles.Count );

			foreach( int i in this.MagicProjectiles.ToArray() ) {
				Projectile proj = Main.projectile[i];
				if( proj == null || !proj.active || !proj.magic ) {
					this.MagicProjectiles.Remove( i );
					continue;
				}

				projWhos.Add( i );
			}

			return projWhos;
		}


		////////////////

		public override bool PreAI( Projectile projectile ) {
			if( this.IsAccountedFor || Main.netMode == 2 ) {
				return true;
			}
			this.IsAccountedFor = true;

			this.Initialize( projectile );

			return base.PreAI( projectile );
		}
	}
}
