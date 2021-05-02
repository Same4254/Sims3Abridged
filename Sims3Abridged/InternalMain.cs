using System;
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

        private static InternalBridgeConnection connection;

        static InternalMain() {
            connection = new InternalBridgeConnection();
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e) {
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, HelloWorld, "Hello World Alarm", AlarmType.NeverPersisted, null);
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, new AlarmTimerCallback(onAlarm), "Cycle Alarm", AlarmType.NeverPersisted, null);
        }

        private static void HelloWorld() {
            connection.addWriteTask(new WriteStringTask("Words"));
            StyledNotification.Show(new StyledNotification.Format("Hello World!", StyledNotification.NotificationStyle.kSystemMessage));
        }

        private static void onAlarm() {
            connection.update();
            AlarmManager.Global.AddAlarm(2f, TimeUnit.Seconds, new AlarmTimerCallback(onAlarm), "Cycle Alarm", AlarmType.NeverPersisted, null);
        }
    }
}