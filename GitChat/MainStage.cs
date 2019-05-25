using System;
using System.Collections.Generic;
using GitChat.Library;

namespace GitChat {
	class MainStage : Stage {
		public MainStage(State state) : base(state) {
			KeyMapping[ConsoleKey.H] = PrevRepo;
			KeyMapping[ConsoleKey.L] = NextRepo;
			KeyMapping[ConsoleKey.O] = ChangeToOpen;
			KeyMapping[ConsoleKey.Y] = RemoveCurrent;
			KeyMapping[ConsoleKey.I] = ReplyCurrent;
		}

		public override void Update() {
			var repos = State.Storage.FindRepositories();
			PrepareRepositories(repos);
			EnsureCurrentRepo();
		}

		public override void Render() {
			base.Render();
			RenderHeader();
			RenderCurrentMessages();
			RenderFooter();
		}
		
		void PrepareRepositories(string[] repos) {
			foreach ( var repo in repos ) {
				if ( !State.Services.ContainsKey(repo) ) {
					State.Services[repo] = new ChatService(State.Storage, repoName: repo);
				}
			}
			State.OrderedServices = new List<ChatService>(State.Services.Values);
		}
		
		void EnsureCurrentRepo() {
			if ( State.SelectedService >= State.OrderedServices.Count ) {
				State.SelectedService = 0;
			}
		}
		
		void RenderHeader() {
			var services = State.OrderedServices;
			for ( var i = 0; i < services.Count; i++ ) {
				var repo      = services[i];
				var isCurrent = (State.SelectedService == i);
				WriteWithColor(repo.RepoName, isCurrent ? ConsoleColor.Green : ConsoleColor.White);
				if ( i < (services.Count - 1) ) {
					Console.Write(" | ");
				}
			}
			if ( services.Count > 0 ) {
				Console.WriteLine();
			}
		}

		void RenderCurrentMessages() {
			if ( State.OrderedServices.Count == 0 ) {
				Console.WriteLine("No repositories in cache");
				return;
			}
			var service = State.CurrentService;
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

		void ChangeToOpen() {
			NewStage = new OpenStage(State);
		}

		void RemoveCurrent() {
			var service = State.CurrentService;
			if ( service == null ) {
				return;
			}
			State.Storage.Clear(service.RepoName);
			State.Services.Remove(service.RepoName);
		}
		
		void ReplyCurrent() {
			if ( State.CurrentService == null ) {
				return;
			}
			NewStage = new ReplyStage(State);
		}

		void PrevRepo() {
			var newIndex = State.SelectedService - 1;
			if ( newIndex < 0 ) {
				return;
			}
			State.SelectedService = newIndex;
		}
		
		void NextRepo() {
			var newIndex = State.SelectedService + 1;
			if ( newIndex >= State.OrderedServices.Count ) {
				return;
			}
			State.SelectedService = newIndex;
		}
	}
}