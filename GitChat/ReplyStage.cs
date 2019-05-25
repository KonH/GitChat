using System;
using GitChat.Library;

namespace GitChat {
	class ReplyStage : Stage {
		public ReplyStage(State state) : base(state) {}
		
		public override void Render() {
			base.Render();
			Console.WriteLine("Your reply:");
		}

		public override void Input() {
			var message = Console.ReadLine();
			State.CurrentService.SendMessage(message);
			NewStage = new MainStage(State);
		}
	}
}