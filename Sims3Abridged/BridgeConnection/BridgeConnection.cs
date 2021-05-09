using System;
using System.Collections.Generic;

using Sims3Abridged.BridgeConnection.Tasks;
using Sims3Abridged.BridgeConnection.Buffer;

using Sims3.UI;

namespace Sims3Abridged.BridgeConnection {
	public abstract class BridgeConnection<ReadType, WriteType> where ReadType : class, ISend where WriteType : class, ISend {
		private CommunicationBuffer communicationBuffer;

		private Queue<Task> writeTasks;
		private Messege currentReadMessege, currentWritingMessege;

		public BridgeConnection() {
			writeTasks = new Queue<Task>();
			currentReadMessege = null;
			currentWritingMessege = null;

			#if EXTERNAL_SCRIPT
				communicationBuffer = new ExternalCommunicationBuffer();
			#else
				communicationBuffer = new InternalCommunicationBuffer();
			#endif
		}

		public void update() {
			BufferReadSection readSection = communicationBuffer.read();
			if(readSection != null) {
				if (currentReadMessege == null)
					currentReadMessege = new Messege(readSection.opcode, readSection.messegeLength);

				currentReadMessege.recv(readSection.packetData);
				if(currentReadMessege.isMessegeComplete()) {
					Type taskType = Opcodes.getType(currentReadMessege.opcode);

					Task t = (Task)Activator.CreateInstance(taskType);
					#if INTERNAL_SCRIPT
						StyledNotification.Show(new StyledNotification.Format("Object type: " + t.GetType().Name + ", opcode: " + currentReadMessege.opcode + ", messegeL: " + currentReadMessege.messegeLength + ", OP Name: " + taskType.Name, StyledNotification.NotificationStyle.kSystemMessage));
					#endif

					if (typeof(ReadType) == typeof(ISendToExternal))
						((ISendToExternal)t).onRead(currentReadMessege.data);
					else
						((ISendToInternal)t).onRead(currentReadMessege.data);

					currentReadMessege = null;
				}
			}

			if (currentWritingMessege == null && writeTasks.Count > 0) {
				Task t = writeTasks.Dequeue();

				byte[] data;
				if(typeof(WriteType) == typeof(ISendToExternal)) {
					data = ((ISendToExternal)t).generateData();
				} else {
					data = ((ISendToInternal)t).generateData();
				}

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

		public void addWriteTask(Task task) {
			//foreach(Type type in task.GetType().GetInterfaces())
			//	if(type == typeof(WriteType))
			//		writeTasks.Enqueue(task);
			if(task is WriteType) {
				writeTasks.Enqueue(task);
			}
		}
	}
}
