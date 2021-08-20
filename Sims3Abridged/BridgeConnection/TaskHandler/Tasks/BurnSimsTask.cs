using System.Collections.Generic;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Abstracts;
using Sims3.UI;

namespace Sims3Abridged.BridgeConnection.Tasks {
	class BurnSimsTask : Task, ISendToInternal {
		public BurnSimsTask() {
		}

		byte[] ISendToInternal.generateData() {
			return new byte[] { };
		}


		void ISendToInternal.onRead(byte[] data) {
			List<Sim> sims = Household.ActiveHousehold.Sims;
			foreach (Sim poorSoul in sims) {
				FireManager.AddFire(poorSoul);
			}

			GameObject[] objects = Household.ActiveHousehold.LotHome.GetObjects<GameObject>();
			foreach (GameObject go in objects) {
				FireManager.AddFire(go);
			}
		}
	}
}
