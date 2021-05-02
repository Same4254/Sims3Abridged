using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims3Abridged.BridgeConnection.Buffer {
	class InternalCommunicationBuffer : CommunicationBuffer {
		public InternalCommunicationBuffer() : base(MAGIC_NUMBER_BYTE_LENGTH, MAGIC_NUMBER_BYTE_LENGTH + HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH) {
			unsafe {
				fixed (byte* p = buffer) {
					int* p2 = (int*)p;
					p2[0] = 69;
					p2[1] = 420;
					p2[2] = 21;
					p2[3] = 1729;
					p2[4] = 3666;
				}
			}

			buffer[readOffset] = 1;
			buffer[writeOffset] = 1;
		}

		public override bool canWrite() {
			return buffer[writeOffset] == 1;
		}

		protected override void setRead(bool read) {
			buffer[readOffset] = (byte)(read ? 1 : 0);
		}

		protected override void setWriteBufferSection(byte[] bytes) {
			Array.Copy(bytes, 0, buffer, writeOffset, bytes.Length);
		}

		protected override void setWritten(bool written) {
			buffer[writeOffset] = (byte)(written ? 0 : 1);
		}

		protected override void updateReadSection() {

		}
	}
}
