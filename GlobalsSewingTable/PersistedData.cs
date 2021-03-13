using Sims3.Gameplay.CAS;
using Sims3.SimIFace;
using System.Collections.Generic;

[Persistable]
public class PersistedData
{
    [Persistable]
    public Dictionary<SimDescription, List<ResourceKey>> mDiscoveredObjects;

    [Persistable]
    public Dictionary<SimDescription, bool> whoIsInPatternClub;

    public PersistedData()
    {
        if (mDiscoveredObjects == null)
        {
            mDiscoveredObjects = new Dictionary<SimDescription, List<ResourceKey>>();
        }
        if (whoIsInPatternClub == null)
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
