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

			setRead(true);
			setWritten(false);
		}

		public override bool canWrite() {
			return buffer[writeOffset] == 1;
		}

		protected override void setRead(bool read) {
			Array.Copy(BitConverter.GetBytes(read ? 1 : 0), 0, buffer, readOffset, 4);
		}

		protected override void setWriteBufferSection(byte[] bytes) {
			Array.Copy(bytes, 0, buffer, writeOffset + 4, bytes.Length);
		}

		protected override void setWritten(bool written) {
			unsafe {
				fixed(byte* ptr = buffer) {
					int* int_ptr = (int*)(ptr + writeOffset);
					int_ptr[0] = written ? 0 : 1;
				}
			}
		}

		protected override void updateReadSection() { }
	}
}
