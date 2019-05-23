namespace GitChat.Library {
	public sealed class Message {
		public readonly string Author;
		public readonly string Content;
		public readonly bool   IsCurrentUser;

		public Message(string author, string content, bool isCurrentUser) {
			Author        = author;
			Content       = content;
			IsCurrentUser = isCurrentUser;
		}
	}
}