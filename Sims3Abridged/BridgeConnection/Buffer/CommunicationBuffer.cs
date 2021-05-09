using System;

using Sims3.UI;

namespace Sims3Abridged.BridgeConnection.Buffer {
	public class BufferReadSection {
		public int opcode { get; }
		public int messegeLength { get; }
		public byte[] packetData { get; }

		public BufferReadSection(int opcode, int messegeLength, byte[] packetData) {
			this.opcode = opcode;
			this.messegeLength = messegeLength;
			this.packetData = packetData;
		}
	}

	public abstract class CommunicationBuffer {
		public static readonly int MAGIC_NUMBER_BYTE_LENGTH = 20;

		public static readonly int HEADER_BYTE_LENGTH = 16;

		public static readonly int DATA_BYTE_LENGTH = 100;

		protected byte[] buffer;

		protected int readOffset, writeOffset;

		public CommunicationBuffer(int readOffset, int writeOffset) {
			this.readOffset = readOffset;
			this.writeOffset = writeOffset;

			this.buffer = new byte[MAGIC_NUMBER_BYTE_LENGTH + HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH + HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH];
		}

		public abstract Boolean canWrite();

		protected abstract void updateReadSection();
		protected abstract void setRead(bool read);

		public BufferReadSection read() {
			updateReadSection();

			unsafe {
				fixed (byte* packet_ptr = buffer) {
					byte* offset_ptr = packet_ptr + readOffset;
					int* packet_int_ptr = (int*)offset_ptr;

					if (packet_int_ptr[0] == 1)
						return null;

					byte[] data = new byte[packet_int_ptr[3]];
					Array.Copy(buffer, readOffset + HEADER_BYTE_LENGTH, data, 0, data.Length);

					setRead(true);

					return new BufferReadSection(packet_int_ptr[1], packet_int_ptr[2], data);
				}
			}
		}

		protected abstract void setWriteBufferSection(byte[] bytes);
		protected abstract void setWritten(bool written);

		public void write(int opcode, int messegeLength, int dataStartIndex, byte[] data) {
			int dataLength = Math.Min(DATA_BYTE_LENGTH, data.Length);

			byte[] toWrite = new byte[HEADER_BYTE_LENGTH - 4 + dataLength];
			unsafe {
				fixed (byte* packet_ptr = toWrite) {
					int* packet_int_ptr = (int*)packet_ptr;

					packet_int_ptr[0] = opcode;
					packet_int_ptr[1] = messegeLength;
					packet_int_ptr[2] = data.Length;
				}
			}

			Array.Copy(data, 0, toWrite, HEADER_BYTE_LENGTH - 4, dataLength);
			setWriteBufferSection(toWrite);
			setWritten(true);
		}
	}
}
