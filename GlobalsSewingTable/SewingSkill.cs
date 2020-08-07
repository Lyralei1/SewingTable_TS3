using System;
using System.Collections.Generic;
using GlobalDLLImporterTest;
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
		
		public static int kMaxLevel = 10;
		
//		[Persistable]
//		public static Dictionary<SimDescription, ResourceKey> mDiscoveredObjects = new Dictionary<SimDescription, ResourceKey>();
		
		public static bool InPatternClub = false;
		
//		[Persistable]
//		public static Dictionary<SimDescription, bool> whoIsInPatternClub = new Dictionary<SimDescription, bool>();
		
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
						Pattern randomPattern = Pattern.DiscoverPatternForGlobalObjects(createdSim);
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
			if(GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.ContainsKey(desc) && GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.ContainsValue(true))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static void AddItemsToDiscoveredList(SimDescription desc, ResourceKey reskey)
		{
			if(!GlobalOptionsSewingTable.retrieveData.mDiscoveredObjects.ContainsKey(desc) && !GlobalOptionsSewingTable.retrieveData.mDiscoveredObjects.ContainsValue(reskey))
			{
				GlobalOptionsSewingTable.retrieveData.mDiscoveredObjects.Add(desc, reskey);
				return;
			}
		}
		
//		public bool ImportDiscoveredObjects(IPropertyStreamReader reader)
//		{
//			SewingTablePropertyReader logicPropertyStreamReader = new SewingTablePropertyReader();
//			reader.AdoptChild(0xA825612D, logicPropertyStreamReader);
//			return logicPropertyStreamReader.Import(out mDiscoveredObjects);
//		}
//		
//		public bool ExportDiscoveredObjects(IPropertyStreamWriter writer)
//		{
//			if (mDiscoveredObjects == null)
//			{
//				return true;
//			}
//			SewingTablePropertyWriter logicPropertyStreamWriter = new SewingTablePropertyWriter();
//			writer.AdoptChild(0xA825612D, logicPropertyStreamWriter);
//			bool flag = logicPropertyStreamWriter.Export(mDiscoveredObjects);
//			if (flag)
//			{
//				writer.CommitChild();
//			}
//			else
//			{
//				writer.CancelChild(0xA825612D);
//			}
//			return flag;
//		}
		
		public override bool ExportContent(IPropertyStreamWriter writer)
		{
			base.ExportContent(writer);
			writer.WriteBool(0x66B7AD5F, InPatternClub);
			//ExportDiscoveredObjects(writer);
			return true;
		}
	
		public override bool ImportContent(IPropertyStreamReader reader)
		{
			base.ImportContent(reader);
			reader.ReadBool(0x66B7AD5F, out InPatternClub, false);
			//ImportDiscoveredObjects(reader);
			return true;
		}
	}
}
