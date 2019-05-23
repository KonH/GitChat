using System.Diagnostics;
using System.IO;

namespace GitChat.Library {
	public sealed class GitRunner {
		public readonly string WorkingDirectory;
		
		readonly CacheStorage _storage;
		readonly string       _originUrl;

		public GitRunner(CacheStorage storage, string originUrl) {
			_storage   = storage;
			_originUrl = originUrl;

			var lastSlashIndex = _originUrl.LastIndexOf('/');
			var repoName = _originUrl.Substring(lastSlashIndex + 1);
			WorkingDirectory = Path.Combine(_storage.RootPath, repoName);

			TryClone();
		}
		
		void TryClone() {
			if ( Directory.Exists(WorkingDirectory) ) {
				return;
			}
			var startInfo = new ProcessStartInfo {
				FileName = "git",
				Arguments = "clone " + _originUrl,
				WorkingDirectory = _storage.RootPath
			};
			Process.Start(startInfo).WaitForExit();
		}

		public void SendMessage(string message) {
			Commit(message);
			Push();
		}

		public string[] ReadMessages() {
			Pull();
			return Log();
		}

		public string GetUserName() {
			var startInfo = new ProcessStartInfo {
				FileName               = "git",
				Arguments              = $"config user.name",
				WorkingDirectory       = WorkingDirectory,
				RedirectStandardOutput = true
			};
			var proc = Process.Start(startInfo);
			proc.WaitForExit();
			var output = proc.StandardOutput.ReadToEnd();
			return output;
		}
		
		void Commit(string message) {
			var startInfo = new ProcessStartInfo {
				FileName         = "git",
				Arguments        = $"commit -m \"{message}\" --allow-empty",
				WorkingDirectory = WorkingDirectory
			};
			Process.Start(startInfo).WaitForExit();
		}

		void Push() {
			var startInfo = new ProcessStartInfo {
				FileName         = "git",
				Arguments        = $"push",
				WorkingDirectory = WorkingDirectory
			};
			Process.Start(startInfo).WaitForExit();
		}

		void Pull() {
			var startInfo = new ProcessStartInfo {
				FileName         = "git",
				Arguments        = $"pull",
				WorkingDirectory = WorkingDirectory
			};
			Process.Start(startInfo).WaitForExit();
		}

		string[] Log() {
			var startInfo = new ProcessStartInfo {
				FileName               = "git",
				Arguments              = $"log --format=\"%an: %s\"",
				WorkingDirectory       = WorkingDirectory,
				RedirectStandardOutput = true
			};
			var proc = Process.Start(startInfo);
			proc.WaitForExit();
			var output = proc.StandardOutput.ReadToEnd();
			return output.Split('\n');
		}
	}
}