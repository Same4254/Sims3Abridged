using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sims3Abridged.BridgeConnection.Buffer;

namespace Sims3Abridged.BridgeConnection {
	/**
	 * The Messege is responsible for either accumalating or splitting an amount of bytes into chunks that the CommunicationBuffer can transmit.
	 * This is what allows for trasmitting data larger than the buffer. The bytes will accumalate in this class before being used.
	 */
	public class Messege {
		//How many bytes have been sent or recieved
		private int byteCount;

		//Total data 
		public byte[] data { get; }

		//Opcode of this messege
		public int opcode { get; }

		//Amount of bytes in the TOTAL messege. This is not the amount of bytes in the packets.
		public int messegeLength { get { return data.Length; } }

		private Messege() {
			this.byteCount = 0;
		}

		/**
		 * This is the recieving constructor.
		 * Given the messege length, this will setup a buffer to hold that many bytes and accumalate the messege for that many bytes
		 * 
		 * Use "recv" with this constructor to recieve packets to accumalate.
		 */
		public Messege(int opcode, int messegeLength) : this() {
			this.opcode = opcode;
			this.data = new byte[messegeLength];
		}

		/**
		 * This is the transmit constructor.
		 * Given the data that is meant to be sent over, this will store how many bytes have been sent and send the next chunk.
		 * 
		 * Use "nextPacketData" with this constructor to get the next packet to be sent.
		 */
		public Messege(int opcode, byte[] data) : this() {
			this.opcode = opcode;
			this.data = data;
		}

		/**
		 * Given the data from a packet, this will store the data at the end of what has been accumalated so far.
		 * After calling this function, use "isMessegeComplete" to check if the entire messege has been recieved
		 */
		public void recv(byte[] bytes) {
			Array.Copy(bytes, 0, data, byteCount, bytes.Length);
			byteCount += bytes.Length;
		}

		/**
		 * This will return an array of bytes with a maximum possible length equivalent to the size of the data parameter in the communication buffer
		 * Calling this function more than once will deliever the next chunk of bytes to be sent. These bytes come from the data given in the constructor
		 */
		public byte[] nextPacketData() {
			byte[] toRet = new byte[Math.Min(data.Length - byteCount, CommunicationBuffer.DATA_BYTE_LENGTH)];

			Array.Copy(data, byteCount, toRet, 0, toRet.Length);
			byteCount += toRet.Length;

			return toRet;
		}

		/**
		 * This returns true if all bytes have been sent or recieved.
		 * Returns false if there are more bytes to be sent or recieved.
		 */
		public bool isMessegeComplete() {
			return byteCount == data.Length;
		}

		public int getByteProgress() { return byteCount; }
	}
}
