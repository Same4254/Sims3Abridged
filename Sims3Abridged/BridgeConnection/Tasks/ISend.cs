using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public interface ISend {
		byte[] generateData();
		void onRead(byte[] data);
	}
}
