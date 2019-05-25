namespace GitChat {
	public sealed class Interface {
		Stage _stage;

		public Interface() {
			_stage = new MainStage(new State());
		}
		
		public void Update() {
			_stage.Update();
			_stage.Render();
			_stage.Input();
			_stage = _stage.NewStage ?? _stage;
		}
	}
}