using System;
using System.Collections.Generic;
using System.Reflection;

using Sims3Abridged.BridgeConnection.Tasks;

namespace Sims3Abridged.BridgeConnection {
	class Opcodes {
		private static readonly Dictionary<int, Type> opcodeToType;
		private static readonly Dictionary<Type, int> typeToOpcode;

		static Opcodes() {
			opcodeToType = new Dictionary<int, Type>();
			typeToOpcode = new Dictionary<Type, int>();

			List<Type> taskTypes = new List<Type>();
			foreach (Type type in Assembly.GetAssembly(typeof(Task)).GetTypes()) {
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Task))) {
					taskTypes.Add(type);
				}
			}

			//Get the types in the same order no matter what order the iterator above uses
			taskTypes.Sort(delegate (Type x, Type y) {
				return x.Name.CompareTo(y.Name);
			});

			for(int i = 0; i < taskTypes.Count; i++) {
				opcodeToType.Add(i, taskTypes[i]);
				typeToOpcode.Add(taskTypes[i], i);
			}
		}

		public static int getOpcode(Type type) {
			int opcode;
			if(!typeToOpcode.TryGetValue(type, out opcode))
				return -1;
			return opcode;
		}

		public static Type getType(int opcode) {
			Type type;
			if (!opcodeToType.TryGetValue(opcode, out type))
				return null;
			return type;
		}
	}
}
