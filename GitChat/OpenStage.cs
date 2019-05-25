using System;
using GitChat.Library;

namespace GitChat {
	class OpenStage : Stage {
		public OpenStage(State state) : base(state) {}
		
		public override void Render() {
			base.Render();
			Console.WriteLine("Your repository URL:");
		}

		public override void Input() {
			var repoUrl = Console.ReadLine();
			var service = new ChatService(State.Storage, originUrl: repoUrl);
			State.Services[service.WorkingDirectory] = service;
			NewStage = new MainStage(State);
		}
	}
}