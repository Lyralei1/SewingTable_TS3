using System;
using System.Collections.Generic;
using Lyralei;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.Hud;

namespace Sims3.Gameplay.Skills.Lyralei
{
	public class SewingSkill : Skill
	{
//		[TunableComment("Rate of gain for Artisan Skill default 1.0f")]
//		[Tunable]
		public static float kSewingSkillGainRate = 5f;

        public enum FabricType
		{
			Knitted   = 1,
			Satin	  = 2,
			Cotton 	  = 3,
			Synthetic = 4,
			Leather   = 5,
			Denim	  = 6,
		}
		
		public const uint kIDHashArtisanSkill = 0x605205D7u;
		
		public static SkillNames kSewingSkillGUID = (SkillNames)0x605205DB;

        [Persistable(false)]
        private List<ITrackedStat> mTrackedStats;

        [Persistable(false)]
        public List<ILifetimeOpportunity> mLifetimeOpportunities;

        public static int kMaxLevel = 10;
		
		public static bool InPatternClub = false;
		
		public override uint GetSkillHash()
		{
			return 0x108734EBu;
		}
		
		public SewingSkill(SkillNames guid)
			: base(guid)
		{
		}
	
		public SewingSkill()
		{
		}

        public class FinishedProjects : ITrackedStat
        {
            private SewingSkill mSkill;

            public string Description
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillStats:CountFinishedProjects", new object[] { mSkill.mProjectCount} );
                }
            }

            public FinishedProjects(SewingSkill skill)
            {
                mSkill = skill;
            }
        }
        public class BlogPostsWritten : ITrackedStat
        {
            private SewingSkill mSkill;

            public string Description
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillStats:BlogPostsWritten", new object[] { mSkill.mBlogPosts });
                }
            }

            public BlogPostsWritten(SewingSkill skill)
            {
                mSkill = skill;
            }
        }

        public class PatternsDiscovered : ITrackedStat
        {
            private SewingSkill mSkill;

            public string Description
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillStats:CountPatterns", new object[] { mSkill.mPatternCount });
                }
            }

            public PatternsDiscovered(SewingSkill skill)
            {
                mSkill = skill;
            }
        }

        public class PercentageOfPatternsDiscovered : ITrackedStat
        {
            private SewingSkill mSkill;

            public string Description
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillStats:PercentagePatterns", new object[] { Convert.ToInt32(mSkill.CalculatePatternPercentage()) });
                }
            }

            public PercentageOfPatternsDiscovered(SewingSkill skill)
            {
                mSkill = skill;
            }
        }
        public bool mMasterSewerIsNew = true;

        public class MasterSewer : ILifetimeOpportunity
        {
            public SewingSkill mSkill;

            public string Title
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:MasterSewerTitle");
                }
            }

            public string RewardDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:MasterSewerDescription", new object[3] { mSkill.SkillLevel, Convert.ToInt32(mSkill.CalculatePatternPercentage()), mSkill.mProjectCount });
                }
            }

            public string AchievedDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:MasterSewerAchievedDescription");
                }
            }

            public bool Completed
            {
                get
                {
                    return IsMasterSewer();
                }
            }

            public bool IsNew
            {
                get
                {
                    return mSkill.mMasterSewerIsNew;
                }
                set
                {
                    mSkill.mMasterSewerIsNew = value;
                }
            }

            public MasterSewer(SewingSkill skill)
            {
                mSkill = skill;
            }
            public bool IsMasterSewer()
            {
                Sim createdSim = mSkill.SkillOwner.CreatedSim;
                bool Level10Reached = mSkill.ReachedMaxLevel();

                if (createdSim.IsSelectable)
                {
                    if (Level10Reached && mSkill.mPatternCount == Pattern.mStoredPatternsKeySettingsList.Count && mSkill.mProjectCount == 100)
                    //if (Level10Reached && mSkill.mProjectCount == 100 && (mSkill.mPatternCount == listAmount))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        public class DoesNotTakeaVillage : ILifetimeOpportunity
        {
            public SewingSkill mSkill;

            public string Title
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DoesNotTakeaVillageTitle");
                }
            }

            public string RewardDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DoesNotTakeaVillageDescription", new object[1] { mSkill.mProjectCount });
                }
            }

            public string AchievedDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DoesNotTakeaVillageAchievedDescription");
                }
            }

            public bool Completed
            {
                get
                {
                    return mSkill.mProjectCount == 500;
                }
            }

            public bool IsNew
            {
                get
                {
                    return mSkill.mMasterSewerIsNew;
                }
                set
                {
                    mSkill.mMasterSewerIsNew = value;
                }
            }

            public DoesNotTakeaVillage(SewingSkill skill)
            {
                mSkill = skill;
            }
        }

        public class DIYBlogger : ILifetimeOpportunity
        {
            public SewingSkill mSkill;

            public string Title
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DIYBloggerTitle");
                }
            }

            public string RewardDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DIYBloggerDescription", new object[1] { mSkill.mBlogPosts });
                }
            }

            public string AchievedDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:DIYBloggerAchievedDescription");
                }
            }

            public bool Completed
            {
                get
                {
                    if(mSkill.mBlogPosts == 50)
                    {
                        return true;
                    }
                    return false;
                }
            }

            public bool IsNew
            {
                get
                {
                    return mSkill.mMasterSewerIsNew;
                }
                set
                {
                    mSkill.mMasterSewerIsNew = value;
                }
            }

            public DIYBlogger(SewingSkill skill)
            {
                mSkill = skill;
            }
        }

        public class PatternCollector : ILifetimeOpportunity
        {
            public SewingSkill mSkill;

            public string Title
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:PatternCollectorTitle");
                }
            }

            public string RewardDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:PatternCollectorDescription", new object[2] { mSkill.CalculatePatternPercentage(), mSkill.mPatternCount });
                }
            }

            public string AchievedDescription
            {
                get
                {
                    return Localization.LocalizeString("Lyralei/Localized/SkillOpportunity:PatternCollectorAchievedDescription");
                }
            }

            public bool Completed
            {
                get
                {
                    return mSkill.mPatternCount == Pattern.mStoredPatternsKeySettingsList.Count;
                }
            }

            public bool IsNew
            {
                get
                {
                    return mSkill.mMasterSewerIsNew;
                }
                set
                {
                    mSkill.mMasterSewerIsNew = value;
                }
            }

            public PatternCollector(SewingSkill skill)
            {
                mSkill = skill;
            }
        }

        public override List<ITrackedStat> TrackedStats
        {
            get
            {
                return mTrackedStats;
            }
        }

        public override List<ILifetimeOpportunity> LifetimeOpportunities
        {
            get
            {
                return mLifetimeOpportunities;
            }
        }

        public int mProjectCount = 0;
        public int mPatternCount = 0;
        public int mPatternPercentage = 0;
        public int mBlogPosts = 0;

        public void AddFinishedProjectsCount(int amountToAdd)
        {
            mProjectCount += amountToAdd;
        }
        public void AddPatternCount(int amountToAdd)
        {
            mPatternCount += amountToAdd;
        }

        public void AddBlogPostCount(int amountToAdd)
        {
            mBlogPosts += amountToAdd;
        }

        public int CalculatePatternPercentage()
        {
            mPatternPercentage = (int)((double)mPatternCount * 100 / Pattern.mStoredPatternsKeySettingsList.Count);
            return mPatternPercentage;
        }

        // Here we add it later for discovering stuff through a computer/Magazine
        public override void SkillLeveledUp()
		{
			Sim createdSim = base.SkillOwner.CreatedSim;
			bool flag = base.ReachedMaxLevel();
			
			if (createdSim.IsSelectable)
			{
				base.PlayLevelUpSting(flag);
				if(createdSim.CurrentInteraction is Sims3.Gameplay.Objects.ReadBook)
				{
					if(RandomUtil.RandomChance(100f))
					{
						Pattern randomPattern = Pattern.DiscoverPattern(createdSim);
						if(randomPattern != null)
						{
							createdSim.Inventory.TryToAdd(randomPattern);
							createdSim.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/BrowseWebForPatternsSuccess:InteractionName", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
						}
					}
				}
			}
			base.SkillLeveledUp();
		}
		
		public static bool isInPatternClub(SimDescription desc)
		{
			if(GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.ContainsKey(desc.mSimDescriptionId) && GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.ContainsValue(true))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public static bool HasAlreadyDiscoveredThis(ulong sim, ResourceKey reskey)
        {
            foreach(KeyValuePair<ulong, List<ResourceKey>> keyvalues in GlobalOptionsSewingTable.retrieveData.mDiscoveredObjectsNEWEST)
            {
                ulong simmie = keyvalues.Key;
                List<ResourceKey> storedKeys = keyvalues.Value;

                if(sim == simmie)
                {
                    foreach(ResourceKey keyStored in storedKeys)
                    {
                        if (keyStored == reskey)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

		
		public static void AddItemsToDiscoveredList(ulong sim, ResourceKey reskey)
		{
            foreach (KeyValuePair<ulong, List<ResourceKey>> keyvalues in GlobalOptionsSewingTable.retrieveData.mDiscoveredObjectsNEWEST)
            {
                ulong simmie = keyvalues.Key;
                List<ResourceKey> storedKeys = keyvalues.Value;

                if (sim == simmie)
                {
                    try
                    {
                        storedKeys.Add(reskey);
                        return;
                    }
                    catch(Exception exc)
                    {
                        GlobalOptionsSewingTable.print("Woops! Something went wrong when adding the pattern to the sim's list of 'known patterns'. if this happens frequently, please show this to Lyralei: /n /n" + exc.ToString());
                        return;
                    }
                }
                else
                {
                    storedKeys.Add(reskey);
                    GlobalOptionsSewingTable.retrieveData.mDiscoveredObjectsNEWEST.Add(sim, storedKeys);
                    return;
                }
            }
		}

        public override void MergeTravelData(Skill skill)
        {
            base.MergeTravelData(skill);
            SewingSkill sewingSkill = skill as SewingSkill;
            print(sewingSkill.mPatternCount.ToString());
            if (sewingSkill != null)
            {
                mProjectCount = sewingSkill.mProjectCount;
                mPatternCount = sewingSkill.mPatternCount;
                mPatternPercentage = sewingSkill.mPatternPercentage;
                mBlogPosts = sewingSkill.mBlogPosts;
            }
        }
        public static void print(string text)
        {
            SimpleMessageDialog.Show("Lyralei's Sewing Table:", text);
        }
        public override void CreateSkillJournalInfo()
        {
            mTrackedStats = new List<ITrackedStat>();
            mTrackedStats.Add(new FinishedProjects(this));
            mTrackedStats.Add(new PatternsDiscovered(this));
            mTrackedStats.Add(new PercentageOfPatternsDiscovered(this));
            mTrackedStats.Add(new BlogPostsWritten(this));
            mLifetimeOpportunities = new List<ILifetimeOpportunity>();
            mLifetimeOpportunities.Add(new PatternCollector(this));
            mLifetimeOpportunities.Add(new DIYBlogger(this));
            mLifetimeOpportunities.Add(new DoesNotTakeaVillage(this));
            mLifetimeOpportunities.Add(new MasterSewer(this));
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
		{
			base.ExportContent(writer);
			writer.WriteBool(0x66B7AD5F, InPatternClub);
			return true;
		}
	
		public override bool ImportContent(IPropertyStreamReader reader)
		{
			base.ImportContent(reader);
			reader.ReadBool(0x66B7AD5F, out InPatternClub, false);
			return true;
		}
	}
}
