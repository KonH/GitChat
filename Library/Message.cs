namespace GitChat.Library {
	public sealed class Message {
		public readonly string Time;
		public readonly string Author;
		public readonly string Content;
		public readonly bool   IsCurrentUser;

		public Message(string time, string author, string content, bool isCurrentUser) {
			Time          = time;
			Author        = author;
			Content       = content;
			IsCurrentUser = isCurrentUser;
		}
	}
}