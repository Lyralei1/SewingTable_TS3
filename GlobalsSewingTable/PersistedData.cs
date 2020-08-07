using System;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.SimIFace;

namespace Lyralei
{
	[Persistable]
	public class PersistedData
	{
		[Persistable]
		public Dictionary<SimDescription, ResourceKey> mDiscoveredObjects;
		
		[Persistable]
		public Dictionary<SimDescription, bool> whoIsInPatternClub;
		
		
		public PersistedData()
		{
			if(mDiscoveredObjects == null)
			{
				mDiscoveredObjects = new Dictionary<SimDescription, ResourceKey>();
			}
			if(whoIsInPatternClub == null)
			{
				whoIsInPatternClub = new Dictionary<SimDescription, bool>();
			}
		}
		
		public void Cleanup()
		{
			mDiscoveredObjects = null;
			whoIsInPatternClub = null;
		}
	}
}
