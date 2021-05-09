using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sims3Abridged.BridgeConnection.Buffer;

namespace Sims3Abridged.BridgeConnection {
	public class Messege {
		public int byteCount;

		public byte[] data { get; }
		public int opcode { get; }

		public int messegeLength { get { return data.Length; } }

		private Messege() {
			this.byteCount = 0;
		}

		public Messege(int opcode, int messegeLength) : this() {
			this.opcode = opcode;
			this.data = new byte[messegeLength];
		}

		public Messege(int opcode, byte[] data) : this() {
			this.opcode = opcode;
			this.data = data;
		}

		public void recv(byte[] bytes) {
			Array.Copy(bytes, 0, data, byteCount, bytes.Length);
			byteCount += bytes.Length;
		}

		public byte[] nextPacketData() {
			byte[] toRet = new byte[Math.Min(data.Length - byteCount, CommunicationBuffer.DATA_BYTE_LENGTH)];

			Array.Copy(data, byteCount, toRet, 0, toRet.Length);
			byteCount += toRet.Length;

			return toRet;
		}

		public bool isMessegeComplete() {
			return byteCount == data.Length;
		}

		public int getByteProgress() { return byteCount; }
	}
}
