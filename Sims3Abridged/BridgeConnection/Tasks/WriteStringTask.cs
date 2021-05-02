using System;
using System.Text;

using Sims3.UI;

namespace Sims3Abridged.BridgeConnection.Tasks {
	class WriteStringTask : Task, ISendToInternal, ISendToExternal {
		private String s;

		public WriteStringTask() {
		}

		public WriteStringTask(String s) {
			this.s = s;
		}

		byte[] ISendToInternal.generateData() {
			return Encoding.ASCII.GetBytes(s);
		}

		byte[] ISendToExternal.generateData() {
			return Encoding.ASCII.GetBytes(s);
		}

		void ISendToInternal.onRead(byte[] data) {
			String temp = Encoding.ASCII.GetString(data);
			StyledNotification.Show(new StyledNotification.Format("Text: (" + data.Length + "): ", StyledNotification.NotificationStyle.kSystemMessage));
		}

		void ISendToExternal.onRead(byte[] data) {
			#if EXTERNAL_SCRIPT
				String temp = Encoding.ASCII.GetString(data);
				Console.WriteLine("Text from internal: ( " + temp + " ): " + temp);
			#endif
		}
	}
}
