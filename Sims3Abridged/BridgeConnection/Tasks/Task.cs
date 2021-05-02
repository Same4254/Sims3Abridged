using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims3Abridged.BridgeConnection.Tasks {
	public class Task : ISend {
		byte[] ISend.generateData() {
			throw new NotImplementedException();
		}

		void ISend.onRead(byte[] data) {
			throw new NotImplementedException();
		}
	}
}
