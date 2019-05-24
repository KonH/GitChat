using System;
using GitChat.Library;

namespace GitChat {
	class Program {
		static void Main(string[] args) {
			var storagePath = ".cache";
			var storage = new CacheStorage(storagePath);
			ChatService service;
			var repos = storage.FindRepositories();
			if ( repos.Length > 0 ) {
				service = new ChatService(storage, repoName: repos[0]);
			} else {
				Console.WriteLine("Your repository URL:");
				var originUrl = Console.ReadLine();
				service = new ChatService(storage, originUrl: originUrl);
			}
			while ( true ) {
				Console.Clear();
				var messages = service.ReadMessages();
				foreach ( var msg in messages ) {
					Console.Write(msg.Time);
					Console.Write(" ");
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