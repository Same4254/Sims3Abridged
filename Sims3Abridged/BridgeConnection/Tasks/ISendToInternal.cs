using System;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public interface ISendToInternal : ISend {
		byte[] generateData();
		void onRead(byte[] data);
	}
}
