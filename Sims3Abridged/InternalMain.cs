using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using Sims3.UI;

using Sims3Abridged.BridgeConnection;
using Sims3Abridged.BridgeConnection.Tasks;

namespace InternalScript {
    public class InternalMain {
        // Entry point for class
        [Tunable]
        public static bool kInstantiator = false;

        private static BridgeConnection connection;

        static InternalMain() {
            connection = BridgeConnection.Instance;
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e) {
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, HelloWorld, "Hello World Alarm", AlarmType.NeverPersisted, null);
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, new AlarmTimerCallback(onAlarm), "Cycle Alarm", AlarmType.NeverPersisted, null);

        }

        private static void HelloWorld() {
            //Make sure that the internal to external is working
            connection.addWriteTask(new WriteStringTask("Words"));
            StyledNotification.Show(new StyledNotification.Format("Hello World!", StyledNotification.NotificationStyle.kSystemMessage));
        }

        private static void onAlarm() {
            //Keep updating the connection and set up another alarm to fire this again later
            connection.update();
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, new AlarmTimerCallback(onAlarm), "Cycle Alarm", AlarmType.NeverPersisted, null);
        }
    }
}