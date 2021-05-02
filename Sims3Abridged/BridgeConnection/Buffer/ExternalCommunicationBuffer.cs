using System;
using System.Collections.Generic;

#if EXTERNAL_SCRIPT
using System.Linq;
using Jupiter;

namespace Sims3Abridged.BridgeConnection.Buffer {
	class ExternalCommunicationBuffer : CommunicationBuffer {
		//****** Scan The Sims process for our buffer. These are the bytes of the numbers at the start of the buffer
		public static readonly byte[] BUFFER_MAGIC_NUMBERS = { 0x45, 0x00, 0x00, 0x00, 0xA4, 0x01, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0xC1, 0x06, 0x00, 0x00, 0x52, 0x0E, 0x00, 0x00 };

		private MemoryModule memoryModule;
		private IntPtr bufferAddress;

		public ExternalCommunicationBuffer() : base(MAGIC_NUMBER_BYTE_LENGTH + HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH, MAGIC_NUMBER_BYTE_LENGTH) {
			this.memoryModule = new MemoryModule("TS3");
			List<IntPtr> addresses = memoryModule.PatternScan(BUFFER_MAGIC_NUMBERS).ToList();

			if (addresses.Count != 1) {
				//Error handle???
			}

			this.bufferAddress = addresses.ElementAt(0);
			//this.bufferAddress = new IntPtr(0x25c0c280);
			Console.WriteLine("Buffer Address: {0:x}", bufferAddress.ToInt32());
		}

		public override bool canWrite() {
			byte[] bytes = memoryModule.ReadVirtualMemory(bufferAddress + writeOffset, 1);

			return bytes[0] == 1;
		}

		protected override void updateReadSection() {
			byte[] packet = memoryModule.ReadVirtualMemory(bufferAddress + readOffset, HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH);
			Array.Copy(packet, 0, buffer, readOffset, packet.Length);
		}

		protected override void setRead(bool read) {
			memoryModule.WriteVirtualMemory(bufferAddress + readOffset, new byte[] { (byte)(read ? 1 : 0) });
		}

		protected override void setWriteBufferSection(byte[] bytes) {
			memoryModule.WriteVirtualMemory(bufferAddress + writeOffset + 1, bytes);
		}

		protected override void setWritten(bool written) {
			memoryModule.WriteVirtualMemory(bufferAddress + writeOffset, new byte[] { (byte)(written ? 0 : 1) });
		}

		//public override BufferReadSection read() {
		//	byte[] packet = memoryModule.ReadVirtualMemory(bufferAddress + HEADER_BYTE_LENGTH + READ_BYTE_LENGTH, HEADER_BYTE_LENGTH + WRITE_BYTE_LENGTH);
		//	if (packet[HEADER_CONTROL_BOOLEAN_BYTE_INDEX] == 1)
		//		return null;

		//	unsafe {
		//		fixed(byte* packet_ptr = packet) {
		//			int* packet_int_ptr = (int*)(packet_ptr + 1);

		//			byte[] data = new byte[packet_int_ptr[2]];
		//			Array.Copy(packet, HEADER_BYTE_LENGTH, data, 0, data.Length);

		//			memoryModule.WriteVirtualMemory(bufferAddress, new byte[] { 1 });

		//			return new BufferReadSection(packet_int_ptr[0], packet_int_ptr[1], data);
		//		}
		//	}
		//}

		//public override void write(int opcode, int messegeLength, int dataStartIndex, byte[] data) {
		//	int dataLength = Math.Min(WRITE_BYTE_LENGTH, data.Length);

		//	byte[] toWrite = new byte[HEADER_BYTE_LENGTH - 1 + dataLength];
		//	unsafe {
		//		fixed (byte* packet_ptr = toWrite) {
		//			int* packet_int_ptr = (int*)packet_ptr;

		//			packet_int_ptr[0] = opcode;
		//			packet_int_ptr[1] = messegeLength;
		//			packet_int_ptr[2] = data.Length;
		//		}
		//	}

		//	Array.Copy(data, 0, toWrite, HEADER_BYTE_LENGTH - 1, dataLength);
		//	memoryModule.WriteVirtualMemory(bufferAddress + 1, toWrite);
		//	memoryModule.WriteVirtualMemory(bufferAddress, new byte[] { 1 });
		//}
	}
}
#endif