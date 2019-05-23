using System;
using System.IO;
using GitChat.Library;
using Xunit;

namespace GitChat.Tests {
	public sealed class CacheStorageTests : IDisposable {
		readonly string _tempDir;
		
		public CacheStorageTests() {
			_tempDir = "." + GetHashCode().ToString();
		}
		
		public void Dispose() {
			if ( Directory.Exists(_tempDir) ) {
				Directory.Delete(_tempDir);
			}
		}
		
		[Fact]
		public void IsRootDirCreatedOnInit() {
			var unused = new CacheStorage(_tempDir);
			
			Assert.True(Directory.Exists(_tempDir));
		}

		[Fact]
		public void IsRootDirRemovedOnClear() {
			var storage = new CacheStorage(_tempDir);
			storage.Clear();
			
			Assert.False(Directory.Exists(_tempDir));
		}
	}
}