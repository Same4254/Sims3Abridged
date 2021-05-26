using System;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public interface ISendToInternal {
		byte[] generateData();
		void onRead(byte[] data);
	}
}
