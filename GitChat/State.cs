using System;
using System.Collections.Generic;
using GitChat.Library;

namespace GitChat {
	public sealed class State {
		enum Stage {
			Main,
			Open,
			Reply
		}
		
		readonly CacheStorage                   _storage;
		readonly Dictionary<Stage, Action>      _handlers;
		readonly Dictionary<ConsoleKey, Action> _keyMapping;

		Stage                           _stage           = Stage.Main;
		Dictionary<string, ChatService> _services        = new Dictionary<string, ChatService>();
		List<ChatService>               _orderedServices = new List<ChatService>();
		int                             _selectedService = 0;

		public State() {
			_storage = new CacheStorage(".cache");
			_handlers = new Dictionary<Stage, Action> {
				{ Stage.Main, Main },
				{ Stage.Open, Open },
			};
			_keyMapping = new Dictionary<ConsoleKey, Action> {
				{ ConsoleKey.O, ChangeToOpen },
				{ ConsoleKey.Y, RemoveCurrent },
				{ ConsoleKey.H, PrevRepo },
				{ ConsoleKey.L, NextRepo },
				{ ConsoleKey.U, () => {} },
			};
		}
		
		public void Update() {
			Console.Clear();
			_handlers[_stage]();
		}

		void Main() {
			var repos = _storage.FindRepositories();
			PrepareRepositories(repos);
			RenderHeader(repos);
			if ( repos.Length > 0 ) {
				EnsureCurrentRepo();
				RenderCurrentMessages();
			} else {
				Console.WriteLine("No repositories in cache");
			}
			RenderFooter();
			ReadMainInput();
		}

		void PrepareRepositories(string[] repos) {
			foreach ( var repo in repos ) {
				if ( !_services.ContainsKey(repo) ) {
					_services[repo] = new ChatService(_storage, repoName: repo);
				}
			}
			_orderedServices = new List<ChatService>(_services.Values);
		}

		void RenderHeader(string[] repos) {
			for ( var i = 0; i < repos.Length; i++ ) {
				var repo = repos[i];
				var isCurrent = (_orderedServices[_selectedService] == _services[repo]);
				WriteWithColor(repo, isCurrent ? ConsoleColor.Green : ConsoleColor.White);
				if ( i < (repos.Length - 1) ) {
					Console.Write(" | ");
				}
			}
			if ( repos.Length > 0 ) {
				Console.WriteLine();
			}
		}

		void EnsureCurrentRepo() {
			if ( _selectedService >= _orderedServices.Count ) {
				_selectedService = 0;
			}
		}
		
		void RenderCurrentMessages() {
			var service = _orderedServices[_selectedService];
			var messages = service.ReadMessages();
			foreach ( var msg in messages ) {
				Console.Write(msg.Time);
				Console.Write(" ");
				WriteWithColor(msg.Author, msg.IsCurrentUser ? ConsoleColor.Green : ConsoleColor.Yellow);
				Console.Write(": ");
				Console.Write(msg.Content);
				Console.WriteLine();
			}
		}

		void RenderFooter() {
			WriteWithColor("Move: [K] Move Up [J] Move Down [H] Prev Chat [L] Next Chat", ConsoleColor.Gray);
			Console.WriteLine();
			WriteWithColor("Chat: [O] Open    [Y] Remove    [U] Update    [I] Reply", ConsoleColor.Gray);
			Console.WriteLine();
		}

		void ReadMainInput() {
			var key = Console.ReadKey().Key;
			if ( _keyMapping.TryGetValue(key, out var handler) ) {
				handler();
			}
		}

		void ChangeToOpen() {
			_stage = Stage.Open;
		}

		void RemoveCurrent() {
			var service = _orderedServices[_selectedService];
			_storage.Clear(service.WorkingDirectory);
			_services.Remove(service.WorkingDirectory);
		}

		void PrevRepo() {
			var newIndex = _selectedService - 1;
			if ( newIndex < 0 ) {
				return;
			}
			_selectedService = newIndex;
		}
		
		void NextRepo() {
			var newIndex = _selectedService + 1;
			if ( newIndex >= _orderedServices.Count ) {
				return;
			}
			_selectedService = newIndex;
		}

		void Open() {
			Console.WriteLine("Your repository URL:");
			var repoUrl = Console.ReadLine();
			var service = new ChatService(_storage, originUrl: repoUrl);
			_services[service.WorkingDirectory] = service;
			_stage = Stage.Main;
		}

		static void WriteWithColor(string message, ConsoleColor color) {
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = oldColor;
		}
	}
}