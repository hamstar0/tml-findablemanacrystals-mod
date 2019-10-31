using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using System;
using Terraria.ModLoader;


namespace FindableManaCrystals.NetProtocols {
	class ManaCrystalShardCheckProtocol : PacketProtocolSendToServer {
		public static void QuickRequest( int tileX, int tileY, float brightness ) {
			var protocol = new ManaCrystalShardCheckProtocol( tileX, tileY, brightness );
			protocol.SendToServer( false );
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

		////

		protected override void InitializeClientSendData() {
		}

		////////////////

		protected override void Receive( int fromWho ) {
			var myworld = ModContent.GetInstance<FindableManaCrystalsWorld>();
			myworld.QueueManaCrystalShardCheck( this.TileX, this.TileY, this.Brightness );
		}
	}
}
