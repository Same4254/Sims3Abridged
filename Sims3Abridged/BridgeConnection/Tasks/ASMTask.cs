using System;
using System.Runtime;
using System.Reflection;

#if EXTERNAL_SCRIPT
using System.IO;
#endif

namespace Sims3Abridged.BridgeConnection.Tasks {
	class ASMTask : Task, ISendToInternal {
		private string filePath;

		public ASMTask(String filePath) {
			this.filePath = filePath;
		}

		public ASMTask() {

		}

		byte[] ISendToInternal.generateData() {
		#if EXTERNAL_SCRIPT
			return File.ReadAllBytes(filePath);
		#else
			return new byte[] { };
		#endif
		}

		void ISendToInternal.onRead(byte[] data) {
			Assembly asm = null;
			try {
				asm = Assembly.Load(data);
			} catch(Exception e) {
				BridgeConnection.Instance.addWriteTask(new WriteStringTask("Message: " + e.Message));
				BridgeConnection.Instance.addWriteTask(new WriteStringTask("Stack Trace: " + e.StackTrace));
				return;
			}

			Type[] types = asm.GetTypes();

			foreach (Type t in types) {
				BridgeConnection.Instance.addWriteTask(new WriteStringTask("Name: " + t.FullName));
			}

			Type c = types[0];

			MethodInfo methodInfo = c.GetMethod("func");

			object obj = Activator.CreateInstance(c);
			methodInfo.Invoke(obj, new object[] { });
		}
	}
}
