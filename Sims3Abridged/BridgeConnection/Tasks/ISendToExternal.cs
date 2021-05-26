using System;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public interface ISendToExternal {
		byte[] generateData();
		void onRead(byte[] data);
	}
}
