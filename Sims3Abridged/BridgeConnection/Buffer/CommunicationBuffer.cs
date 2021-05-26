using System;

using Sims3.UI;

namespace Sims3Abridged.BridgeConnection.Buffer {
	/**
	 * This class is a small struct of information from the read section of the buffer
	 * This is usefull so we can expose each part as a name with a data type. 
	 */
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

	/**
	 * The Communication buffer the the meat and potatoes of this program. This is where we can transmit.
	 * 
	 * This abstract class is meant to operate in both the internal and external sides.
	 * Where possible code is generalized and when specific behavior is needed depending on whther it is
	 *		internal or external, that is left for the child class to implement.
	 */
	public abstract class CommunicationBuffer {
		public static readonly int MAGIC_NUMBER_BYTE_LENGTH = 20;
		public static readonly int HEADER_BYTE_LENGTH = 16;
		public static readonly int DATA_BYTE_LENGTH = 100;

		protected byte[] buffer;

		//The number of bytes to offset by to get to the read and write section of the buffer
		//The values depend on whether this is external or internal. Hence the OOP.
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
			//Allow the external communication buffer to make the system call to read the memory of the Sims 3 process
			updateReadSection();

			unsafe {
				fixed (byte* packet_ptr = buffer) {
					byte* offset_ptr = packet_ptr + readOffset;
					int* packet_int_ptr = (int*)offset_ptr;

					// The first integer is the lock. If it is a 1 then the messege has already been read from
					if (packet_int_ptr[0] == 1)
						return null;

					// Read the packet data into an independent array
					byte[] data = new byte[packet_int_ptr[3]];
					Array.Copy(buffer, readOffset + HEADER_BYTE_LENGTH, data, 0, data.Length);

					// Flip the lock (child-implemented)
					setRead(true);

					return new BufferReadSection(packet_int_ptr[1], packet_int_ptr[2], data);
				}
			}
		}

		/**
		 * Given all of the bytes for the write section, excluding the lock, change the write section to these bytes.
		 * Child implemented.
		 */
		protected abstract void setWriteBufferSection(byte[] bytes);

		/**
		 * Flips the control lock on the write section to read as it has or has not been written (but not yet read)
		 */
		protected abstract void setWritten(bool written);

		public void write(int opcode, int messegeLength, int dataStartIndex, byte[] data) {
			int dataLength = Math.Min(DATA_BYTE_LENGTH, data.Length);

			//Create an array of bytes to set as the write section (excluding the 4 bytes of the lock)
			byte[] toWrite = new byte[HEADER_BYTE_LENGTH - 4 + dataLength];
			unsafe {
				fixed (byte* packet_ptr = toWrite) {
					int* packet_int_ptr = (int*)packet_ptr;

					packet_int_ptr[0] = opcode;
					packet_int_ptr[1] = messegeLength;
					packet_int_ptr[2] = data.Length;
				}
			}

			//Copy the data into the new write section
			Array.Copy(data, 0, toWrite, HEADER_BYTE_LENGTH - 4, dataLength);

			//Set this as the new write section
			setWriteBufferSection(toWrite);

			//Flip the lock
			setWritten(true);
		}
	}
}
