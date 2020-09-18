using System;
using System.Collections.Generic;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Lyralei;
using Sims3.SimIFace;
using Sims3.UI.Hud;

namespace Lyralei
{
	[Persistable]
	public class PersistedData
	{
		[Persistable]
		public Dictionary<SimDescription, List<ResourceKey>> mDiscoveredObjects;
		
		[Persistable]
		public Dictionary<SimDescription, bool> whoIsInPatternClub;

        //[Persistable(false)]
        //public List<ITrackedStat> mTrackedStats;


        public PersistedData()
		{
			if(mDiscoveredObjects == null)
			{
				mDiscoveredObjects = new Dictionary<SimDescription, List<ResourceKey>>();
			}
			if(whoIsInPatternClub == null)
			{
				whoIsInPatternClub = new Dictionary<SimDescription, bool>();
			}
          //  if(mTrackedStats == null)
          //  {
           //     mTrackedStats = new List<ITrackedStat>();
           // }
		}
		
		public void Cleanup()
		{
			mDiscoveredObjects = null;
			whoIsInPatternClub = null;
           // mTrackedStats = null;

        }
	}
}
