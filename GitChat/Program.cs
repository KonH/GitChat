namespace GitChat {
	class Program {
		static void Main(string[] args) {
			var ui = new Interface();
			while ( true ) {
				ui.Update();
			}
		}
	}
}