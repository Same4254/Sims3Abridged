using System;
using System.Collections.Generic;
using System.Reflection;

using Sims3Abridged.BridgeConnection.Tasks;

namespace Sims3Abridged.BridgeConnection {
	/**
	 *	This class assigns the opcodes for the instructions.
	 *	The actual number for the opcode is not important, just that it is unique from other opcodes.
	 *	Thus, this class will use Reflection to find all of the Tasks and simply assing numbers by incrementing for each class starting at 0.
	 */
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

			//Get the types in the same order no matter what order the iterator above uses. 
			//This is running two different versions of .NET and compiled for different architectures, so do this to make sure the opcodes are assigned in the same order
			taskTypes.Sort(delegate (Type x, Type y) {
				return x.Name.CompareTo(y.Name);
			});

			for(int i = 0; i < taskTypes.Count; i++) {
				opcodeToType.Add(i, taskTypes[i]);
				typeToOpcode.Add(taskTypes[i], i);
			}
		}

		/**
		 * Given a type, return the integer opcode that was assigned to it.
		 * If the type (which should be a Task) has not been assigned, this returns -1
		 */
		public static int getOpcode(Type type) {
			int opcode;
			if(!typeToOpcode.TryGetValue(type, out opcode))
				return -1;
			return opcode;
		}


		/**
		 * Given the integer opcode, this will return the Task type that is assigned to that opcode.
		 * If there is no type for the given opcode, then it will return null.
		 * 
		 * The use for this type would be to create a new isntance of the Task with the empty constructor
		 */
		public static Type getType(int opcode) {
			Type type;
			if (!opcodeToType.TryGetValue(opcode, out type))
				return null;
			return type;
		}
	}
}
