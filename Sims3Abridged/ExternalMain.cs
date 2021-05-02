using System;

using Sims3Abridged.BridgeConnection;
using Sims3Abridged.BridgeConnection.Tasks;

#if EXTERNAL_SCRIPT
namespace Sims3Abridged {
	class ExternalMain {
		static void Main(string[] args) {
			//WriteStringTask t = new WriteStringTask("Hello");
			//ISendToExternal s = t as ISendToExternal;
			//Console.WriteLine(s.generateData().Length);

			//Console.ReadLine();

			ExternalBridgeConnection connection = new ExternalBridgeConnection();

			String s = "";
			while (s != "exit") {
				//Read a string from the console
				s = Console.ReadLine();
				if (s == "exit")
					continue;

				connection.addWriteTask(new WriteStringTask(s));

				connection.update();
			}
		}
	}
}
#endif