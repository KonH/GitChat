using System.Diagnostics;
using System.IO;

namespace GitChat.Library {
	public sealed class GitRunner {
		public readonly string WorkingDirectory;
		public readonly string RepoName;
		
		readonly CacheStorage _storage;

		public GitRunner(CacheStorage storage, string originUrl = null, string repoName = null) {
			_storage   = storage;
			
			RepoName         = repoName ?? GetRepoNameFromOrigin(originUrl);
			WorkingDirectory = Path.Combine(_storage.RootPath, RepoName);
			
			TryClone(originUrl);
		}
		
		void TryClone(string originUrl) {
			if ( string.IsNullOrEmpty(originUrl) ) {
				return;
			}
			if ( Directory.Exists(WorkingDirectory) ) {
				return;
			}
			Git("clone " + originUrl, _storage.RootPath);
		}

		string GetRepoNameFromOrigin(string originUrl) {
			var lastSlashIndex = originUrl.LastIndexOf('/');
			var repoName       = originUrl.Substring(lastSlashIndex + 1);
			return repoName;
		}

		public void SendMessage(string message) {
			Pull();
			Commit(message);
			while ( Push().stderror.Contains("Updates were rejected because the remote contains work") ) {
				Pull();
			}
		}

		public string[] ReadMessages() {
			Pull();
			return Log();
		}

		public string GetUserName() {
			return Git("config user.name").stdout;
		}
		
		void Commit(string message) {
			Git($"commit -m \"{message}\" --allow-empty");
		}

		(string stdout, string stderror) Push() {
			return Git("push");
		}

		void Pull() {
			Git("pull");
		}

		string[] Log() {
			return Git("log -n 15 --format=\"%ar^%an:%s\" --no-merges").stdout.Split('\n');
		}

		(string stdout, string stderror) Git(string command, string workingDirectory = null) {
			if ( workingDirectory == null ) {
				workingDirectory = WorkingDirectory;
			}
			var startInfo = new ProcessStartInfo {
				FileName               = "git",
				Arguments              = command,
				WorkingDirectory       = workingDirectory,
				RedirectStandardOutput = true,
				RedirectStandardError  = true,
			};
			var proc = Process.Start(startInfo);
			if ( proc != null ) {
				proc.WaitForExit();
				var error = proc.StandardError.ReadToEnd();
				var output = proc.StandardOutput.ReadToEnd();
				return (output, error);
			}
			return (string.Empty, string.Empty);
		}
	}
}