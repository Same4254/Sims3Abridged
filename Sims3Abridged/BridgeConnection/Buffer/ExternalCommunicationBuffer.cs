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
			//The buffer tends to be after this address, so use this for quicker searching. Not guaranteed. To be replaced with actual pointer mapping later on

			//List<IntPtr> addresses = memoryModule.PatternScan(new IntPtr(0x20000000), BUFFER_MAGIC_NUMBERS).ToList();

			//if (addresses.Count != 1) {
			//	//Error handle???
			//}

			//this.bufferAddress = addresses.ElementAt(0);
			//Console.WriteLine("Buffer Address: {0:x}", bufferAddress.ToInt32());
			
			//From Cheat Engine investigation
			int baseAddress = 0x00400000;
			int baseOffset = 0x00E1D89C;

			//Using the path from cheat engine, navigate the pointers to our buffer (hopefully)
			List<int> offsets = new List<int>{ 0x18, 0x24, 0x1e0, 0x1c, 0x1c, 0x1c, 0x18, 0x1c, 0x10 };
			int address = baseAddress + baseOffset;
			foreach(int offset in offsets) {
				address = BitConverter.ToInt32(memoryModule.ReadVirtualMemory(new IntPtr(address), 4), 0) + offset;
			}
			Console.WriteLine("Buffer Address: {0:x}", address);
			bufferAddress = new IntPtr(address);

			//Make sure that the pointer has actually given us the buffer
			byte[] toCompare = memoryModule.ReadVirtualMemory(bufferAddress, BUFFER_MAGIC_NUMBERS.Length);
			bool equal = Enumerable.SequenceEqual(BUFFER_MAGIC_NUMBERS, toCompare);
			if(!equal) {
				Console.WriteLine("Big boi error time! The bytes at the found address are not correct!");
				System.Environment.Exit(-1);
			}
		}

		public override bool canWrite() {
			byte[] bytes = memoryModule.ReadVirtualMemory(bufferAddress + writeOffset, 4);
			unsafe {
				fixed(byte *ptr = bytes) {
					int* int_ptr = (int*)ptr;
					return int_ptr[0] == 1;
				}
			}
		}

		protected override void updateReadSection() {
			byte[] packet = memoryModule.ReadVirtualMemory(bufferAddress + readOffset, HEADER_BYTE_LENGTH + DATA_BYTE_LENGTH);
			Array.Copy(packet, 0, buffer, readOffset, packet.Length);
		}

		protected override void setRead(bool read) {
			memoryModule.WriteVirtualMemory(bufferAddress + readOffset, new byte[] { (byte)(read ? 1 : 0) });
		}

		protected override void setWriteBufferSection(byte[] bytes) {
			memoryModule.WriteVirtualMemory(bufferAddress + writeOffset + 4, bytes);
		}

		protected override void setWritten(bool written) {
			memoryModule.WriteVirtualMemory(bufferAddress + writeOffset, BitConverter.GetBytes(written ? 0 : 1));//new byte[] { (byte)(written ? 0 : 1) });
		}
	}
}
#endif