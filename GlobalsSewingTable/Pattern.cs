﻿using System;
using System.Collections.Generic;
using Lyralei;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Lyralei;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.BuildBuy;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
    public class Pattern : GameObject, IExportableContent
    {
        public static List<ResourceKey> mStoredPatternsKeySettingsList = new List<ResourceKey>();

        public class PatternNameComponent : NameComponent
        {
            public PatternNameComponent()
            {
            }

            public PatternNameComponent(GameObject o)
                : base(o)
            {
            }

            public PatternNameComponent(GameObject o, bool isBuildBuyNameOnly)
                : base(o)
            {
            }

            public PatternNameComponent(GameObject o, string randomNameGenerator)
                : base(o)
            {
            }

            public override string GetDefaultName()
            {
                Pattern pattern = base.Owner as Pattern;
                if (pattern == null)
                {
                    return string.Empty;
                }
                return pattern.NameComponent.Name;
            }
        }

        public class PatternObjectInitParams : Simulator.ObjectInitParameters
        {
            public List<SewingSkill.FabricType> fabricsNeeded;

            public string Name;

            public bool IsMagic;

            public int amountOfFabricToRemove;

            public int mSkilllevel;

            public ResourceKey resKeyPattern;

            public bool isClothing;

            public CASPart mSimOutfit;

            public string mClothingName;

            public bool mWasPatternGifted;

            public PatternObjectInitParams()
            {
            }

            public PatternObjectInitParams(List<SewingSkill.FabricType> fabrics, bool magic, int removeAmount, int skilllevel, ResourceKey resKey, bool clothing, CASPart outfit, string clothingName, bool wasGifted)
            {
                // IF this is needed, make sure to add price values and such. (See: NectarBottleObjectInitParams)
                fabricsNeeded = fabrics;
                IsMagic = magic;
                mSkilllevel = skilllevel;
                amountOfFabricToRemove = removeAmount;
                resKeyPattern = resKey;
                isClothing = clothing;
                mSimOutfit = outfit;
                mClothingName = clothingName;
                mWasPatternGifted = wasGifted;
            }

            public PatternObjectInitParams(ulong createGroupId, Vector3 createAtPostion, List<SewingSkill.FabricType> fabrics, bool magic, int skilllevel, int level, Vector3 createFacing, HiddenFlags hiddenFlags, int removeAmount, string name, ResourceKey resKey, bool clothing, CASPart outfit, string clothingName, bool wasGifted)
            : base(createGroupId, createAtPostion, level, createFacing, hiddenFlags)
            {
                // IF this is needed, make sure to add price values and such. (See: NectarBottleObjectInitParams)
                fabricsNeeded = fabrics;
                IsMagic = magic;
                mSkilllevel = skilllevel;
                amountOfFabricToRemove = removeAmount;
                resKeyPattern = resKey;
                isClothing = clothing;
                mSimOutfit = outfit;
                mClothingName = clothingName;
                mWasPatternGifted = wasGifted;
            }
        }

        [Persistable]
        public struct PatternInfo
        {
            // Is magic, fabric types needed, name, made by level 10 sim
            public List<SewingSkill.FabricType> fabricsNeeded;

            public string Name;

            public bool IsMagic;

            public int amountOfFabricToRemove;

            public int mSkilllevel;

            public ResourceKey resKeyPattern;

            public bool isClothing;

            public CASPart mSimOutfit;

            public string mClothingName;

            public bool mWasPatternGifted;
        }

        public PatternInfo mPatternInfo;

        public string Name
        {
            get
            {
                return mPatternInfo.Name;
            }
        }

        public int SkillLevel
        {
            get
            {
                return mPatternInfo.mSkilllevel;
            }
        }

        public List<SewingSkill.FabricType> Fabrics
        {
            get
            {
                return mPatternInfo.fabricsNeeded;
            }
        }

        public ResourceKey ResourceKeyPattern
        {
            get
            {
                return mPatternInfo.resKeyPattern;
            }
        }

        public bool Magic
        {
            get
            {
                return mPatternInfo.IsMagic;
            }
        }

        public bool IsClothing
        {
            get
            {
                return mPatternInfo.isClothing;
            }
        }

        public CASPart simOutfitData
        {
            get
            {
                return mPatternInfo.mSimOutfit;
            }
        }

        public string ClothingName
        {
            get
            {
                return mPatternInfo.mClothingName;
            }
        }

        public bool WasGifted
        {
            get
            {
                return mPatternInfo.mWasPatternGifted;
            }
        }

        public ResourceKey myResKey = new ResourceKey(0, 0, 0);

        public override void OnStartup()
        {
            base.AddComponent<PatternNameComponent>(new object[0]);
            base.AddComponent<ItemComponent>(new object[1]
            {
                new List<Type>(new Type[3]
                {
                    typeof(Sim),
                    typeof(Mailbox),
                    typeof(GameObject),
				})
            });

            base.AddInteraction(PutInInventory.Singleton);
            base.OnStartup();
        }

        public override bool StacksWith(IGameObject other)
        {
            return false;
        }

        //public static Pattern DiscoverPatternStartUp()
        //{
        //    ResourceKey getPattern = new ResourceKey();
        //    PatternInfo mPatternInfoInit = new PatternInfo();
        //    List<ResourceKey> checkIfAdded = new List<ResourceKey>();

        //    do
        //    {
        //        getPattern = RandomUtil.GetRandomObjectFromList(ObjectLoader.EasySewablesList);
        //        if (!checkIfAdded.Contains(getPattern))
        //        {
        //            checkIfAdded.Add(getPattern);
        //            break;
        //        }
        //    }
        //    while (true);

        //    if (mStoredPatternsKeySettingsList.Contains(getPattern))
        //    {
        //        try
        //        {
        //            for (int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
        //            {
        //                mPatternInfoInit.resKeyPattern = getPattern;
        //                // Move on if the reskey is invalid. 
        //                if (mPatternInfoInit.resKeyPattern == ResourceKey.kInvalidResourceKey)
        //                {
        //                    continue;
        //                }

        //                mPatternInfoInit.fabricsNeeded = ObjectLoader.sewableSettings[i].typeFabric;
        //                mPatternInfoInit.IsMagic = ObjectLoader.sewableSettings[i].isMagicProject;
        //                mPatternInfoInit.amountOfFabricToRemove = ObjectLoader.sewableSettings[i].amountRemoveFabric;
        //                mPatternInfoInit.mSkilllevel = 0;
        //                mPatternInfoInit.isClothing = ObjectLoader.sewableSettings[i].isClothing;
        //                mPatternInfoInit.mClothingName = ObjectLoader.sewableSettings[i].clothingName;
        //                mPatternInfoInit.mWasPatternGifted = false;

        //                if (mPatternInfoInit.isClothing)
        //                {
        //                    mPatternInfoInit.mSimOutfit = new CASPart(mPatternInfoInit.resKeyPattern);
        //                }

        //                // Pattern OBJD key.
        //                ResourceKey reskey1 = new ResourceKey(0x19D4F5930F26B2D8, 0x319E4F1D, 0x00000000);
        //                Pattern.PatternObjectInitParams initData = new Pattern.PatternObjectInitParams(mPatternInfoInit.fabricsNeeded, mPatternInfoInit.IsMagic, mPatternInfoInit.amountOfFabricToRemove, mPatternInfoInit.mSkilllevel, mPatternInfoInit.resKeyPattern, mPatternInfoInit.isClothing, mPatternInfoInit.mSimOutfit, mPatternInfoInit.mClothingName, mPatternInfoInit.mWasPatternGifted);
        //                Pattern pattern = (Pattern)GlobalFunctions.CreateObjectOutOfWorld(reskey1, null, initData);

        //                if (pattern.GetType() == typeof(FailureObject))
        //                {
        //                    return null;
        //                }
        //                // We always pick a random EASY EA resource (and creator stuff) but clothing will always have a hardness level of medium. So we don't need the clothing check!!
        //                if (pattern != null)
        //                {
        //                    IGameObject getname = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(mPatternInfoInit.resKeyPattern, null, initData);
        //                    if (getname != null)
        //                    {
        //                        // Currently uses the pattern object's name. We need to concatinate the sewable's name here as well. Since EA never made a function to get the name direction from the resource key, we need to do this.
        //                        mPatternInfoInit.Name = pattern.GetLocalizedName() + ": " + getname.GetLocalizedName();
        //                        pattern.NameComponent.SetName(pattern.GetLocalizedName() + ": " + getname.GetLocalizedName());
        //                        // Now we finally got the name and can destroy the object.
        //                        getname.Destroy();
        //                    }
        //                    // Currently uses the pattern object's name. We need to concatinate the sewable's name here as well. Since EA never made a function to get the name direction from the resource key, we need to do this.
        //                    mPatternInfoInit.Name = pattern.GetLocalizedName() + ": " + getname.GetLocalizedName();
        //                    pattern.NameComponent.SetName(pattern.GetLocalizedName() + ": " + getname.GetLocalizedName());
        //                    return pattern;
        //                }
        //                else if (pattern != null && mPatternInfoInit.isClothing)
        //                {
        //                    mPatternInfoInit.Name = mPatternInfoInit.mClothingName;
        //                    pattern.NameComponent.SetName(mPatternInfoInit.Name);

        //                    pattern.mPatternInfo = mPatternInfoInit;
        //                    return pattern;
        //                }
        //                else
        //                {
        //                    GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n The pattern doesn't exist! Did you delete things from the sewing table .package? Else, contact Lyralei.");
        //                    return null;
        //                }
        //            }
        //        }
        //        catch (Exception ex2)
        //        {
        //            GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n REPORT THIS TO LYRALEI: " + ex2.ToString());
        //            return null;
        //        }
        //    }
        //    return null;
        //}

        public static ResourceKey GetUnregisteredpattern(Sim actor, bool NeedsToCache)
        {
            ResourceKey getPattern = new ResourceKey();
            SimDescription actorDesc = actor.SimDescription;

            // Checks whether the current actor already knows the pattern chosen.
            do
            {
                getPattern = GetRandomKeyWithSKills(actor);

                if (!SewingSkill.HasAlreadyDiscoveredThis(actorDesc.mSimDescriptionId, getPattern))
                {
                    return getPattern;
                }
            }
            while (true);
        }

        // Only use if we need to Discover patterns through the sewing table
        //public static Pattern DiscoverPatternWithOnlySewingTable(GameObject target, Sim actor)
        //{
        //    PatternInfo mPatternInfoInit = new PatternInfo();
        //    ResourceKey getPattern = GetUnregisteredpattern(actor, false);
        //    SimDescription actorDesc = actor.SimDescription;


        //    ResourceKey emptyRes = new ResourceKey(0uL, 0u, 0u);
        //    if (getPattern != emptyRes)
        //    {

        //        for (int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
        //        {

        //            if (ObjectLoader.sewableSettings[i].key == getPattern)
        //            {
        //                try
        //                {
        //                    mPatternInfoInit.resKeyPattern = getPattern;
        //                    mPatternInfoInit.fabricsNeeded = ObjectLoader.sewableSettings[i].typeFabric;
        //                    mPatternInfoInit.IsMagic = ObjectLoader.sewableSettings[i].isMagicProject;
        //                    mPatternInfoInit.amountOfFabricToRemove = ObjectLoader.sewableSettings[i].amountRemoveFabric;
        //                    mPatternInfoInit.isClothing = ObjectLoader.sewableSettings[i].isClothing;
        //                    mPatternInfoInit.mClothingName = ObjectLoader.sewableSettings[i].clothingName;
        //                    mPatternInfoInit.mWasPatternGifted = false;

        //                    if (mPatternInfoInit.isClothing)
        //                    {
        //                        mPatternInfoInit.mSimOutfit = new CASPart(mPatternInfoInit.resKeyPattern);
        //                    }

        //                    // Move on if the reskey is invalid. 
        //                    if (mPatternInfoInit.resKeyPattern == ResourceKey.kInvalidResourceKey)
        //                    {
        //                        continue;
        //                    }

        //                    // Pattern OBJD key.
        //                    ResourceKey reskey1 = new ResourceKey(0x19D4F5930F26B2D8, 0x319E4F1D, 0x00000000);
        //                    Pattern.PatternObjectInitParams initData = new Pattern.PatternObjectInitParams(mPatternInfoInit.fabricsNeeded, mPatternInfoInit.IsMagic, mPatternInfoInit.amountOfFabricToRemove, mPatternInfoInit.mSkilllevel, mPatternInfoInit.resKeyPattern, mPatternInfoInit.isClothing, mPatternInfoInit.mSimOutfit, mPatternInfoInit.mClothingName, mPatternInfoInit.mWasPatternGifted);
        //                    Pattern pattern = GlobalFunctions.CreateObject(reskey1, target.GetSlotPosition(Slot.ContainmentSlot_1), 0, target.GetForwardOfSlot(Slot.ContainmentSlot_1), null, initData) as Pattern;

        //                    SetPatternMaterial(pattern, mPatternInfoInit.mSkilllevel, actor);

        //                    if (pattern.GetType() == typeof(FailureObject))
        //                    {
        //                        continue;
        //                    }
        //                    if (pattern != null)
        //                    {
        //                        SimDescription desc = actor.SimDescription;
        //                        SewingSkill.AddItemsToDiscoveredList(desc, mPatternInfoInit.resKeyPattern);
        //                        IGameObject getname = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(mPatternInfoInit.resKeyPattern, null, initData);
        //                        // Currently uses the pattern object's name. We need to concatinate the sewable's name here as well. Since EA never made a function to get the name direction from the resource key, we need to do this.
        //                        mPatternInfoInit.Name = pattern.GetLocalizedName() + ": " + getname.GetLocalizedName();
        //                        pattern.NameComponent.SetName(pattern.GetLocalizedName() + ": " + getname.GetLocalizedName());
        //                        // Now we finally got the name and can destroy the object. 
        //                        getname.Destroy();
        //                        //Set global

        //                        //actor.ShowTNSIfSelectable("Ah yes! I just came up with a great project to sew and just drew the pattern for it! For the time being, I put it in the sewing table's inventory.", StyledNotification.NotificationStyle.kSimTalking);
        //                        return pattern;
        //                    }
        //                    else if (pattern != null && mPatternInfoInit.isClothing)
        //                    {
        //                        mPatternInfoInit.Name = mPatternInfoInit.mClothingName;
        //                        pattern.NameComponent.SetName(mPatternInfoInit.mClothingName);

        //                        pattern.mPatternInfo = mPatternInfoInit;
        //                        actor.Inventory.TryToAdd(pattern);
        //                        return pattern;
        //                    }
        //                    else
        //                    {
        //                        GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n The pattern doesn't exist! Did you delete things from the sewing table .package? Else, contact Lyralei.");
        //                        return null;
        //                    }
        //                }
        //                catch (Exception ex2)
        //                {
        //                    GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n REPORT THIS TO LYRALEI: " + ex2.ToString());
        //                    return null;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        public static ResourceKey GetRandomKeyWithSKills(Sim actor)
        {

            ResourceKey emptyRes = new ResourceKey(0uL, 0u, 0u);
            ResourceKey randomPatternKey = new ResourceKey();
            Pattern.PatternInfo mPatternInfoInit = new Pattern.PatternInfo();
            int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);

            if (skillLevel >= 0)
            {
                randomPatternKey = RandomUtil.GetRandomObjectFromList(ObjectLoader.EasySewablesList);
                mPatternInfoInit.mSkilllevel = 0;
                return randomPatternKey;
            }
            if (skillLevel >= 3)
            {

                randomPatternKey = RandomUtil.GetRandomObjectFromList(ObjectLoader.MediumSewablesList);
                mPatternInfoInit.mSkilllevel = 3;
                return randomPatternKey;
            }
            if (skillLevel >= 6)
            {
                randomPatternKey = RandomUtil.GetRandomObjectFromList(ObjectLoader.HardSewablesList);
                mPatternInfoInit.mSkilllevel = 6;
                return randomPatternKey;
            }
            if (skillLevel >= 9)
            {
                randomPatternKey = RandomUtil.GetRandomObjectFromList(ObjectLoader.HarderSewablesList);
                mPatternInfoInit.mSkilllevel = 9;
                if (actor.mSimDescription.IsWitch || (actor.mSimDescription.IsFairy) || (actor.mSimDescription.IsGenie) || (actor.mSimDescription.IsImaginaryFriend))
                {
                    randomPatternKey = RandomUtil.GetRandomObjectFromList(ObjectLoader.MagicHarderSewablesList);
                    mPatternInfoInit.mSkilllevel = 9;
                    return randomPatternKey;
                }
                return randomPatternKey;
            }
            return emptyRes;
        }

        public static Pattern GetRandomClothingPattern(Sim sim)
        {
            ResourceKey reskeyPattern;
            SewingSkill sewingSkill = sim.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;

            do
            {
                reskeyPattern = RandomUtil.GetRandomObjectFromList(ObjectLoader.mStoredClothingPattern);

                if (!SewingSkill.HasAlreadyDiscoveredThis(sim.mSimDescription.mSimDescriptionId, reskeyPattern))
                {
                    break;
                }
            }
            while (true);

            if(reskeyPattern != ResourceKey.kInvalidResourceKey)
            {
                PatternInfo mPatternInfoInit = new PatternInfo();
                mPatternInfoInit.resKeyPattern = reskeyPattern;
                ObjectLoader.sewableSetting sSetting = ObjectLoader.dictSettings[reskeyPattern];
                mPatternInfoInit.fabricsNeeded = sSetting.typeFabric;
                mPatternInfoInit.IsMagic = sSetting.isMagicProject;
                mPatternInfoInit.amountOfFabricToRemove = sSetting.amountRemoveFabric;
                mPatternInfoInit.isClothing = sSetting.isClothing;
                mPatternInfoInit.mClothingName = sSetting.clothingName;
                mPatternInfoInit.mWasPatternGifted = false;

                if (mPatternInfoInit.isClothing)
                {
                    mPatternInfoInit.mSimOutfit = new CASPart(mPatternInfoInit.resKeyPattern);
                }

                ResourceKey reskey1 = new ResourceKey(0x19D4F5930F26B2D8, 0x319E4F1D, 0x00000000);
                Pattern.PatternObjectInitParams initData = new Pattern.PatternObjectInitParams(mPatternInfoInit.fabricsNeeded, mPatternInfoInit.IsMagic, mPatternInfoInit.amountOfFabricToRemove, mPatternInfoInit.mSkilllevel, mPatternInfoInit.resKeyPattern, mPatternInfoInit.isClothing, mPatternInfoInit.mSimOutfit, mPatternInfoInit.mClothingName, mPatternInfoInit.mWasPatternGifted);
                Pattern pattern = (Pattern)GlobalFunctions.CreateObjectOutOfWorld(reskey1, null, initData);

                mPatternInfoInit.Name = mPatternInfoInit.mClothingName;
                pattern.NameComponent.SetName(mPatternInfoInit.mClothingName);

                pattern.mPatternInfo = mPatternInfoInit;
                sim.Inventory.TryToAdd(pattern);
                sewingSkill.AddPatternCount(1);

                return pattern;
            }
            return null;

        }

        public static Pattern DiscoverPattern(Sim actor)
        {
            //int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
            ResourceKey getPattern = GetUnregisteredpattern(actor, false);
            ResourceKey emptyRes = new ResourceKey(0uL, 0u, 0u);
            PatternInfo mPatternInfoInit = new PatternInfo();
            SewingSkill sewingSkill = actor.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;

            try
            {
                ObjectLoader.sewableSetting sSetting = ObjectLoader.dictSettings[getPattern];

                mPatternInfoInit.resKeyPattern = getPattern;
                mPatternInfoInit.fabricsNeeded = sSetting.typeFabric;
                mPatternInfoInit.IsMagic = sSetting.isMagicProject;
                mPatternInfoInit.amountOfFabricToRemove = sSetting.amountRemoveFabric;
                mPatternInfoInit.isClothing = sSetting.isClothing;
                mPatternInfoInit.mClothingName = sSetting.clothingName;
                mPatternInfoInit.mWasPatternGifted = false;

                //mPatternInfoInit.mSkilllevel 				= 0;

                if (mPatternInfoInit.isClothing)
                {
                    mPatternInfoInit.mSimOutfit = new CASPart(mPatternInfoInit.resKeyPattern);
                }

                // Pattern OBJD key.
                ResourceKey reskey1 = new ResourceKey(0x19D4F5930F26B2D8, 0x319E4F1D, 0x00000000);
                Pattern.PatternObjectInitParams initData = new Pattern.PatternObjectInitParams(mPatternInfoInit.fabricsNeeded, mPatternInfoInit.IsMagic, mPatternInfoInit.amountOfFabricToRemove, mPatternInfoInit.mSkilllevel, mPatternInfoInit.resKeyPattern, mPatternInfoInit.isClothing, mPatternInfoInit.mSimOutfit, mPatternInfoInit.mClothingName, mPatternInfoInit.mWasPatternGifted);
                Pattern pattern = (Pattern)GlobalFunctions.CreateObjectOutOfWorld(reskey1, null, initData);

                if (pattern.GetType() == typeof(FailureObject))
                {
                    return null;
                }

                if (pattern != null)
                {
                    IGameObject getname = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(mPatternInfoInit.resKeyPattern, null, initData);
                    if (getname != null)
                    {

                        // Currently uses the pattern object's name. We need to concatinate the sewable's name here as well. Since EA never made a function to get the name direction from the resource key, we need to do this.
                        mPatternInfoInit.Name = pattern.GetLocalizedName() + ": " + getname.GetLocalizedName();
                        pattern.NameComponent.SetName(pattern.GetLocalizedName() + ": " + getname.GetLocalizedName());

                        // Now we finally got the name and can destroy the object.
                        getname.Destroy();
                    }
                    SimDescription desc = actor.SimDescription;
                    pattern.mPatternInfo = mPatternInfoInit;
                    SewingSkill.AddItemsToDiscoveredList(desc.mSimDescriptionId, mPatternInfoInit.resKeyPattern);

                    actor.Inventory.TryToAdd(pattern);
                    sewingSkill.AddPatternCount(1);
                    return pattern;
                }
                else if (pattern != null && mPatternInfoInit.isClothing)
                {
                    mPatternInfoInit.Name = mPatternInfoInit.mClothingName;
                    pattern.NameComponent.SetName(mPatternInfoInit.mClothingName);

                    pattern.mPatternInfo = mPatternInfoInit;
                    actor.Inventory.TryToAdd(pattern);
                    sewingSkill.AddPatternCount(1);

                    return pattern;
                }
                else
                {
                    GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n The pattern doesn't exist! Did you delete things from the sewing table .package? Else, contact Lyralei.");
                    return null;
                }
            }
            catch (Exception ex2)
            {
                GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n REPORT THIS TO LYRALEI: " + ex2.ToString());
                return null;
            }
        }


        public static void DiscoverAllPatterns(Sim actor)
        {
            List<ResourceKey> allPatternsList = new List<ResourceKey>();
            allPatternsList.AddRange(ObjectLoader.EasySewablesList);
            allPatternsList.AddRange(ObjectLoader.MediumSewablesList);
            allPatternsList.AddRange(ObjectLoader.HardSewablesList);
            allPatternsList.AddRange(ObjectLoader.HarderSewablesList);
            allPatternsList.AddRange(ObjectLoader.MagicHarderSewablesList);

            Pattern.PatternInfo mPatternInfoInit = new Pattern.PatternInfo();
            int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);

            for (int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
            {
                //					if(ObjectLoader.sewableSettings[i].key == getPattern)
                //   				{
                try
                {
                    mPatternInfoInit.resKeyPattern = ObjectLoader.sewableSettings[i].key;
                    mPatternInfoInit.fabricsNeeded = ObjectLoader.sewableSettings[i].typeFabric;
                    mPatternInfoInit.IsMagic = ObjectLoader.sewableSettings[i].isMagicProject;
                    mPatternInfoInit.amountOfFabricToRemove = ObjectLoader.sewableSettings[i].amountRemoveFabric;
                    mPatternInfoInit.mSkilllevel = 0;
                    mPatternInfoInit.isClothing = ObjectLoader.sewableSettings[i].isClothing;
                    mPatternInfoInit.mClothingName = ObjectLoader.sewableSettings[i].clothingName;
                    mPatternInfoInit.mWasPatternGifted = false;

                    if (mPatternInfoInit.isClothing)
                    {
                        mPatternInfoInit.mSimOutfit = new CASPart(mPatternInfoInit.resKeyPattern);
                    }

                    // Move on if the reskey is invalid. 
                    if (mPatternInfoInit.resKeyPattern == ResourceKey.kInvalidResourceKey)
                    {
                        continue;
                    }

                    // Pattern OBJD key.
                    ResourceKey reskey1 = new ResourceKey(0x19D4F5930F26B2D8, 0x319E4F1D, 0x00000000);
                    Pattern.PatternObjectInitParams initData = new Pattern.PatternObjectInitParams(mPatternInfoInit.fabricsNeeded, mPatternInfoInit.IsMagic, mPatternInfoInit.amountOfFabricToRemove, mPatternInfoInit.mSkilllevel, mPatternInfoInit.resKeyPattern, mPatternInfoInit.isClothing, mPatternInfoInit.mSimOutfit, mPatternInfoInit.mClothingName, mPatternInfoInit.mWasPatternGifted);
                    Pattern pattern = (Pattern)GlobalFunctions.CreateObjectOutOfWorld(reskey1, null, initData);

                    //Move on if the pattern resulted into a failure Object
                    if (pattern.GetType() == typeof(FailureObject))
                    {
                        continue;
                    }

                    if (pattern != null && !mPatternInfoInit.isClothing)
                    {
                        IGameObject getname = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(mPatternInfoInit.resKeyPattern, null, initData);
                        if (getname != null)
                        {
                            // Currently uses the pattern object's name. We need to concatinate the sewable's name here as well. Since EA never made a function to get the name direction from the resource key, we need to do this.
                            mPatternInfoInit.Name = pattern.GetLocalizedName() + ":" + getname.GetLocalizedName();
                            pattern.NameComponent.SetName(pattern.GetLocalizedName() + ": " + getname.GetLocalizedName());
                            // Now we finally got the name and can destroy the object.
                            getname.Destroy();
                        }
                        pattern.mPatternInfo = mPatternInfoInit;
                        actor.Inventory.TryToAdd(pattern);
                    }
                    else if (pattern != null && mPatternInfoInit.isClothing)
                    {
                        mPatternInfoInit.Name = mPatternInfoInit.mClothingName;
                        pattern.NameComponent.SetName(mPatternInfoInit.mClothingName);

                        pattern.mPatternInfo = mPatternInfoInit;
                        actor.Inventory.TryToAdd(pattern);
                    }
                    else
                    {
                        GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n The pattern doesn't exist! Did you delete things from the sewing table .package? Else, contact Lyralei.");
                    }
                    SewingSkill.AddItemsToDiscoveredList(actor.mSimDescription.mSimDescriptionId, mPatternInfoInit.resKeyPattern);
                }
                catch (Exception ex2)
                {
                    GlobalOptionsSewingTable.print("Lyralei's Sewing table: \n \n REPORT THIS TO LYRALEI: " + ex2.ToString());
                }
                //}
            }
        }

        public static void SetPatternMaterial(GameObject obj, int objSkill, Sim ActorCurr)
        {
            List<string> EasyMat = new List<string>()
               {
                   "4010185124",
                   "Default"
               };

            List<string> MediumMat = new List<string>()
               {
                   "1513881282",
                   "1530658997",
                   "1480326140"
               };

            List<string> HardMat = new List<string>()
               {
                   "4010185124",
                   "2349299754",
                   "4154529313"
               };

            List<string> HardestMat = new List<string>()
               {
                   "4010185124",
                   "Default",
                   "1513881282",
                   "1530658997",
                   "1480326140",
                   "4010185124",
                   "2349299754",
                   "4154529313",
               };

            if (ActorCurr != null)
            {
                int skillLevel = ActorCurr.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
                if (obj != null)
                {
                    if (skillLevel >= 0)
                    {
                        //easy
                        string mChosenMaterial = RandomUtil.GetRandomObjectFromList(EasyMat);
                        obj.SetMaterial(mChosenMaterial);
                    }
                    if (skillLevel >= 3)
                    {
                        //medium
                        string mChosenMaterial = RandomUtil.GetRandomObjectFromList(MediumMat);
                        obj.SetMaterial(mChosenMaterial);
                    }
                    if (skillLevel >= 6)
                    {
                        //hard
                        string mChosenMaterial = RandomUtil.GetRandomObjectFromList(HardMat);
                        obj.SetMaterial(mChosenMaterial);
                    }
                    if (skillLevel >= 9)
                    {
                        //hardest
                        string mChosenMaterial = RandomUtil.GetRandomObjectFromList(HardestMat);
                        obj.SetMaterial(mChosenMaterial);
                    }
                    else
                    {
                        obj.SetMaterial("Default");
                    }
                }
            }
            if (obj != null)
            {
                if (objSkill >= 0)
                {
                    string mChosenMaterial = RandomUtil.GetRandomObjectFromList(EasyMat);
                    obj.SetMaterial(mChosenMaterial);
                }
                if (objSkill >= 3)
                {
                    string mChosenMaterial = RandomUtil.GetRandomObjectFromList(MediumMat);
                    obj.SetMaterial(mChosenMaterial);
                }
                if (objSkill >= 6)
                {
                    string mChosenMaterial = RandomUtil.GetRandomObjectFromList(HardMat);
                    obj.SetMaterial(mChosenMaterial);
                }
                if (objSkill >= 9)
                {
                    string mChosenMaterial = RandomUtil.GetRandomObjectFromList(HardMat);
                    obj.SetMaterial(mChosenMaterial);
                }
                else
                {
                    obj.SetMaterial("Default");
                }
            }
        }
        public override string ToTooltipString()
        {
            string name = base.NameComponent.Name;
            return name ?? base.ToTooltipString();
        }

        public static bool AddClothingToGiftable(Pattern pattern, Sim desc)
        {
            if (!GlobalOptionsSewingTable.retrieveData.mGiftableClothing.ContainsKey(desc))
            {
                // If sim isn't in the dictionary yet, add the sim and create a list.
                GlobalOptionsSewingTable.retrieveData.mGiftableClothing.Add(desc, new List<Pattern>());
            }
            //foreach (KeyValuePair<Sim, List<Pattern>> keyvalues in GlobalOptionsSewingTable.retrieveData.mGiftableClothing)
            //{
                // if we found the sim, and the pattern isn't in the list yet, add the pattern.
            if(!GlobalOptionsSewingTable.retrieveData.mGiftableClothing[desc].Contains(pattern))
            {
                GlobalOptionsSewingTable.retrieveData.mGiftableClothing[desc].Add(pattern);
                return true;
            }
            return false;
            //}
            //return false;
        }


        public override void OnCreation()
        {
            base.OnCreation();
            PatternObjectInitParams patternObjectInitParameters = Simulator.GetObjectInitParameters(base.ObjectId) as PatternObjectInitParams;
            if (patternObjectInitParameters != null)
            {
                mPatternInfo.fabricsNeeded = patternObjectInitParameters.fabricsNeeded;
                mPatternInfo.Name = patternObjectInitParameters.Name;
                mPatternInfo.IsMagic = patternObjectInitParameters.IsMagic;
                mPatternInfo.amountOfFabricToRemove = patternObjectInitParameters.amountOfFabricToRemove;
                mPatternInfo.resKeyPattern = patternObjectInitParameters.resKeyPattern;
                if(mPatternInfo.isClothing)
                {
                    mPatternInfo.isClothing = patternObjectInitParameters.isClothing;
                    mPatternInfo.mClothingName = patternObjectInitParameters.mClothingName;
                    mPatternInfo.mSimOutfit = patternObjectInitParameters.mSimOutfit;
                }
                else
                {
                    mPatternInfo.isClothing = false;
                    mPatternInfo.mClothingName = "";
                    mPatternInfo.mSimOutfit = new CASPart();
                }
            }
        }
    }
}