using System;
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
			Git("clone " + _originUrl, _storage.RootPath);
		}

		public void SendMessage(string message) {
			Pull();
			Commit(message);
			Push();
		}

		public string[] ReadMessages() {
			Pull();
			return Log();
		}

		public string GetUserName() {
			return Git("config user.name");
		}
		
		void Commit(string message) {
			Git($"commit -m \"{message}\" --allow-empty");
		}

		void Push() {
			Git("push");
		}

		void Pull() {
			Git("pull");
		}

		string[] Log() {
			return Git("log --format=\"%an: %s\"").Split('\n');
		}

		string Git(string command, string workingDirectory = null) {
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
				if ( !string.IsNullOrWhiteSpace(error) ) {
					Console.WriteLine(error);
				}
				return proc.StandardOutput.ReadToEnd();
			}
			return string.Empty;
		}
	}
}