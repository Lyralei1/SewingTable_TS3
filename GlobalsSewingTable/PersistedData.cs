using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

public class PersistedData
{

    [Persistable]
    public Dictionary<ulong, List<ResourceKey>> mDiscoveredObjectsNEWEST;

    [Persistable]
    public Dictionary<Sim, List<Pattern>> mGiftableClothing;

    [Persistable]
    public Dictionary<ulong, bool> whoIsInPatternClub;

    public PersistedData()
    {
        if (mDiscoveredObjectsNEWEST == null)
        {
            mDiscoveredObjectsNEWEST = new Dictionary<ulong, List<ResourceKey>>();
        }
        if (whoIsInPatternClub == null)
        {
            whoIsInPatternClub = new Dictionary<ulong, bool>();
        }
        if (mGiftableClothing == null)
        {
            mGiftableClothing = new Dictionary<Sim, List<Pattern>>();
        }
        
    }

    public static void print(string text)
    {
        SimpleMessageDialog.Show("Lyralei's Sewing Table:", text);
    }

    public void Cleanup()
    {
        mDiscoveredObjectsNEWEST = null;
        whoIsInPatternClub = null;
        mGiftableClothing = null;
    }
}
