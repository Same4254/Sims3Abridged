using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

using Sims3Abridged.BridgeConnection;
using Sims3Abridged.BridgeConnection.Tasks;

#if EXTERNAL_SCRIPT
using System.Threading;

namespace Sims3Abridged {
	class ExternalMain {
		private static String inputString;

		public static void asyncConsoleRead() {
			while(true) {
				//Blocking code sits here and waits for user input
				String temp = Console.ReadLine();

				//Lock the inputString variable to set its value
				//but don't block inside the lock
				lock (inputString) {
					inputString = temp;
				}
			}
		}

        static void Main(string[] args) {
			BridgeConnection.BridgeConnection connection = BridgeConnection.BridgeConnection.Instance;
			inputString = "";

			Thread inputThread = new Thread(ExternalMain.asyncConsoleRead);
			inputThread.IsBackground = true;
			inputThread.Start();

			while (true) {
				lock (inputString) {
					if (inputString != "") {
						if (inputString == "exit")
							break;
						else if (inputString == "burn")
							connection.addWriteTask(new BurnSimsTask());
						else if (inputString == "asm")
							connection.addWriteTask(new ASMTask("Sims3TestDll.dll"));
						else
							connection.addWriteTask(new WriteStringTask(inputString));

						inputString = "";
					}
				}

				connection.update();

				Thread.Sleep(100);
			}
		}
	}
}
#endif