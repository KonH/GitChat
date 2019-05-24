using System.IO;
using System.Linq;

namespace GitChat.Library {
	public sealed class CacheStorage {
		public readonly string RootPath;
		
		public CacheStorage(string rootPath) {
			RootPath = rootPath;
			EnsureRootAccess();
		}

		void EnsureRootAccess() {
			if ( Directory.Exists(RootPath) ) {
				return;
			}
			Directory.CreateDirectory(RootPath);
		}

		public void Clear() {
			if ( Directory.Exists(RootPath) ) {
				Directory.Delete(RootPath, true);
			}
		}

		public string[] FindRepositories() {
			return Directory.EnumerateDirectories(RootPath)
				.Where(dir => Directory.Exists(Path.Combine(dir, ".git")))
				.Select(dir => Path.GetRelativePath(RootPath, dir))
				.ToArray();
		}
	}
}