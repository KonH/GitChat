namespace GitChat.Tests {
	static class Env {
		public const string RepoUrl = "https://github.com/KonH/GitChatTestRepo";

		public static string GetTempDir(object context) {
			return "." + context.GetHashCode().ToString();
		}
	}
}