using System;
using System.IO;
using GitChat.Library;
using Xunit;

namespace GitChat.Tests {
	public sealed class GitRunnerTests : IDisposable {
		readonly CacheStorage _storage;

		public GitRunnerTests() {
			_storage = new CacheStorage(Env.GetTempDir(this));
		}
		
		public void Dispose() {
			_storage.Clear();
		}
		
		[Fact]
		public void IsWorkingDirectorySet() {
			// https://.../RepoName => RepoName
			var repoUrl         = Env.RepoUrl;
			var expectedDirName = repoUrl.Substring(repoUrl.LastIndexOf('/') + 1);
			var expectedDirPath = Path.Combine(_storage.RootPath, expectedDirName);
			
			var runner = new GitRunner(_storage, repoUrl);
			
			Assert.Equal(expectedDirPath, runner.WorkingDirectory);
		}
		
		[Fact]
		public void IsRepoDirCreated() {
			var runner = new GitRunner(_storage, Env.RepoUrl);
			
			Assert.True(Directory.Exists(runner.WorkingDirectory));
		}

		[Fact]
		public void IsMessageCommited() {
			var message = Guid.NewGuid().ToString();
			
			var runner = new GitRunner(_storage, Env.RepoUrl);
			runner.SendMessage(message);
			
			Assert.Contains(runner.ReadMessages(), s => s.Contains(message));
		}
	}
}