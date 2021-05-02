using System;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public interface ISendToExternal : ISend {
		byte[] generateData();
		void onRead(byte[] data);
	}
}
