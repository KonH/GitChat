namespace GitChat {
	class Program {
		static void Main(string[] args) {
			var state = new State();
			while ( true ) {
				state.Update();
			}
		}
	}
}