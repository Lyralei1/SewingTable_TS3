using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.SimIFace;
using System.Collections.Generic;

[Persistable]
public class PersistedData
{
    [Persistable]
    public Dictionary<SimDescription, List<ResourceKey>> mDiscoveredObjects;

    [Persistable]
    public Dictionary<Sim, List<Pattern>> mGiftableClothing;

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
        if (mGiftableClothing == null)
        {
            mGiftableClothing = new Dictionary<Sim, List<Pattern>>();
        }
    }

    public void Cleanup()
    {
        mDiscoveredObjects = null;
        whoIsInPatternClub = null;
        mGiftableClothing = null;
    }
}
