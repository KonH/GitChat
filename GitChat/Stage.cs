using System;
using System.Collections.Generic;

namespace GitChat {
	abstract class Stage {
		public Stage NewStage = null;

		protected readonly State State;
		
		protected Dictionary<ConsoleKey, Action> KeyMapping = new Dictionary<ConsoleKey, Action>();

		public Stage(State state) {
			State = state;
		}
		
		public virtual void Update() {}

		public virtual void Render() {
			Console.Clear();
		}

		public virtual void Input() {
			if ( KeyMapping.Count > 0 ) {
				var key = Console.ReadKey().Key;
				if ( KeyMapping.TryGetValue(key, out var handler) ) {
					handler();
				}
			}
		}
		
		protected void WriteWithColor(string message, ConsoleColor color) {
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = oldColor;
		}
	}
}