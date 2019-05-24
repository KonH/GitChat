using System.Linq;

namespace GitChat.Library {
	public sealed class ChatService {
		readonly CacheStorage _storage;
		readonly GitRunner    _git;
		readonly string       _userName;
		
		public ChatService(string originUrl) {
			_storage  = new CacheStorage(".cache");
			_git      = new GitRunner(_storage, originUrl);
			_userName = _git.GetUserName().Trim();
		}

		public void SendMessage(string message) {
			_git.SendMessage(message);
		}

		public Message[] ReadMessages() {
			var rawMessages = _git.ReadMessages();
			return rawMessages
				.Select(ConvertStringToMessage)
				.Where(m => (m != null))
				.Reverse()
				.ToArray();
		}

		Message ConvertStringToMessage(string str) {
			var delimiterIndex = str.IndexOf(":");
			if ( delimiterIndex < 0 ) {
				return null;
			}
			var author = str.Substring(0, delimiterIndex);
			var message = str.Substring(delimiterIndex + 2);
			var isCurrentUser = (author == _userName);
			return new Message(author, message, isCurrentUser);
		}
	}
}