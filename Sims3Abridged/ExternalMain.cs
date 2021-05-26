using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


using Sims3Abridged.BridgeConnection;
using Sims3Abridged.BridgeConnection.Tasks;

#if EXTERNAL_SCRIPT
namespace Sims3Abridged {
	class ExternalMain {

        static void Main(string[] args) {
			BridgeConnection.BridgeConnection connection = new BridgeConnection.BridgeConnection();

			String s = "";
			while (s != "exit") {
				//Read a string from the console
				s = Console.ReadLine();
				if (s == "exit")
					break;
				else if (s == "burn")
					connection.addWriteTask(new BurnSimsTask());
				else
					connection.addWriteTask(new WriteStringTask(s));

				connection.update();
			}

			Console.ReadLine();
		}
	}
}
#endif