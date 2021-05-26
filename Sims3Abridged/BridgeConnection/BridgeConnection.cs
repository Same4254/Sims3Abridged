using System;
using System.Collections.Generic;

using Sims3Abridged.BridgeConnection.Tasks;
using Sims3Abridged.BridgeConnection.Buffer;

using Sims3.UI;

namespace Sims3Abridged.BridgeConnection {
	/**
	 * The BridgeConnection is the front-end to all of this.
	 * 
	 * This is responsible for managing the current messeges, queue up writing tasks, and telling the communication buffer what to transmit.
	 * The code here is meant to operate in both the external and internal side. 
	 */
	public class BridgeConnection {
		private CommunicationBuffer communicationBuffer;

		private Queue<Task> writeTasks;
		private Messege currentReadMessege, currentWritingMessege;

		public BridgeConnection() {
			writeTasks = new Queue<Task>();
			currentReadMessege = null;
			currentWritingMessege = null;

			//These "macros" are to pick and choose what code to compile depending on the compiler flags.
			//This prevents external .NET 4.0 specific code from getting into the internal mod
			#if EXTERNAL_SCRIPT
				communicationBuffer = new ExternalCommunicationBuffer();
			#else
				communicationBuffer = new InternalCommunicationBuffer();
			#endif
		}

		/**
		 * This performs 2 operations and MUST be called often.
		 * 
		 * READ:
		 *	- Read off the bytes in the read section and see if there is anything to be done. If there is, just DO IT
		 *	
		 * Write
		 *	- If there is something to be written to the buffer, and the write section is free, then write the packet 
		 */
		public void update() {
			BufferReadSection readSection = communicationBuffer.read();
			if(readSection != null) {
				if (currentReadMessege == null)
					currentReadMessege = new Messege(readSection.opcode, readSection.messegeLength);

				// Accumalate the bytes into the messege
				currentReadMessege.recv(readSection.packetData);

				// When the messsege is all recieved, hand it off to the task
				if(currentReadMessege.isMessegeComplete()) {
					Type taskType = Opcodes.getType(currentReadMessege.opcode);

					//Creat an instance of the task with the empty constructor
					Task t = (Task)Activator.CreateInstance(taskType);
					#if INTERNAL_SCRIPT
						StyledNotification.Show(new StyledNotification.Format("Object type: " + t.GetType().Name + ", opcode: " + currentReadMessege.opcode + ", messegeL: " + currentReadMessege.messegeLength + ", OP Name: " + taskType.Name, StyledNotification.NotificationStyle.kSystemMessage));
					#endif

					//Depending on what side this connection is on, cast the task to the corresponding interface to read the data
					#if INTERNAL_SCRIPT
						((ISendToInternal)t).onRead(currentReadMessege.data);
					#else
						((ISendToExternal)t).onRead(currentReadMessege.data);
					#endif

					currentReadMessege = null;
				}
			}

			if (currentWritingMessege == null && writeTasks.Count > 0) {
				Task t = writeTasks.Dequeue();

				byte[] data;

				//Cast the task to the correct interface and generate the data to be sent
				#if INTERNAL_SCRIPT
					data = ((ISendToExternal)t).generateData();
				#else
					data = ((ISendToInternal)t).generateData();
				#endif

				currentWritingMessege = new Messege(Opcodes.getOpcode(t.GetType()), data);
			}

			if(communicationBuffer.canWrite() && currentWritingMessege != null) {
				int startIndex = currentWritingMessege.getByteProgress();
				byte[] data = currentWritingMessege.nextPacketData();
				communicationBuffer.write(currentWritingMessege.opcode, currentWritingMessege.messegeLength, startIndex, data);

				if (currentWritingMessege.isMessegeComplete())
					currentWritingMessege = null;
			}
		}

		/**
		 * This will queue up the given task to be sent.
		 * NOTE: This task will call generateData when it is popped off the queue, not when it is added to the queue.
		 */
		public void addWriteTask(Task task) {
		#if INTERNAL_SCRIPT
			if (task is ISendToExternal)
		#else
			if (task is ISendToInternal)
		#endif
				writeTasks.Enqueue(task);
		}
	}
}
