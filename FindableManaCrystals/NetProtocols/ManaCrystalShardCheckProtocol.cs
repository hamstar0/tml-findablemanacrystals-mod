using System;
using Terraria.ModLoader;
using ModLibsCore.Services.Network.SimplePacket;


namespace FindableManaCrystals.NetProtocols {
	[Serializable]
	class ManaCrystalShardCheckProtocol : SimplePacketPayload {
		public static void QuickRequest( int tileX, int tileY, float brightness ) {
			var packet = new ManaCrystalShardCheckProtocol( tileX, tileY, brightness );
			SimplePacket.SendToServer( packet );
		}



		////////////////

		public int TileX;
		public int TileY;
		public float Brightness;



		////////////////

		private ManaCrystalShardCheckProtocol() { }

		private ManaCrystalShardCheckProtocol( int tileX, int tileY, float brightness ) {
			this.TileX = tileX;
			this.TileY = tileY;
			this.Brightness = brightness;
		}

		////////////////

		public override void ReceiveOnServer( int fromWho ) {
			var myworld = ModContent.GetInstance<FMCWorld>();
			myworld.QueueManaCrystalShardCheck( this.TileX, this.TileY, this.Brightness );
		}

		public override void ReceiveOnClient() {
			throw new NotImplementedException();
		}
	}
}
