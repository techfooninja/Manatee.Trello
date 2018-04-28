﻿using Manatee.Trello.Json;

namespace Manatee.Trello.UnitTests.Internal.Synchronization
{
	internal class SynchronizedData : IAcceptId
	{
		public string Test { get; set; } = "default";
		public string Dependency { get; set; } = "none";

		public bool ValidForMerge { get; set; } = true;
	}
}