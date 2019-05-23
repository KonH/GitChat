using System;
using GitChat.Library;

namespace GitChat {
	class Program {
		static void Main(string[] args) {
			var storagePath = ".cache";
			Console.WriteLine("Your repository URL:");
			var originUrl = Console.ReadLine();
			var service = new ChatService(originUrl);
			while ( true ) {
				Console.Clear();
				Console.WriteLine(originUrl);
				var messages = service.ReadMessages();
				foreach ( var msg in messages ) {
					WriteWithColor(msg.Author, msg.IsCurrentUser ? ConsoleColor.Green : ConsoleColor.Yellow);
					Console.Write(": ");
					Console.Write(msg.Content);
					Console.WriteLine();
				}
				Console.WriteLine("Your reply (empty for exit):");
				var message = Console.ReadLine();
				if ( string.IsNullOrEmpty(message) ) {
					return;
				}
				service.SendMessage(message);
			}
		}

		static void WriteWithColor(string message, ConsoleColor color) {
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = oldColor;
		}
	}
}