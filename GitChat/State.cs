using System.Collections.Generic;
using GitChat.Library;

namespace GitChat {
	sealed class State {
		public readonly CacheStorage Storage = new CacheStorage(".cache");
		
		public Dictionary<string, ChatService> Services        = new Dictionary<string, ChatService>();
		public List<ChatService>               OrderedServices = new List<ChatService>();
		public int                             SelectedService = 0;

		public ChatService CurrentService {
			get {
				if ( SelectedService < 0 ) {
					return null;
				}
				if ( SelectedService >= OrderedServices.Count ) {
					return null;
				}
				return OrderedServices[SelectedService];
			}
		}
	}
}