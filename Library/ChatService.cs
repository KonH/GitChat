using System;
using System.Linq;

namespace GitChat.Library {
	public sealed class ChatService {
		readonly GitRunner _git;
		readonly string    _userName;

		public string WorkingDirectory => _git.WorkingDirectory;
		public string RepoName         => _git.RepoName;

		public ChatService(CacheStorage storage, string originUrl = null, string repoName = null) {
			_git      = new GitRunner(storage, originUrl, repoName);
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
			var timeDelimiter = str.IndexOf("^", StringComparison.Ordinal);
			var nameDelimiter = str.IndexOf(":", StringComparison.Ordinal);
			if ( (timeDelimiter < 0) || (nameDelimiter < 0) ) {
				return null;
			}
			var time = str.Substring(0, timeDelimiter);
			var author = str.Substring(timeDelimiter + 1, nameDelimiter - timeDelimiter - 1);
			var message = str.Substring(nameDelimiter + 1);
			var isCurrentUser = (author == _userName);
			return new Message(time, author, message, isCurrentUser);
		}
	}
}