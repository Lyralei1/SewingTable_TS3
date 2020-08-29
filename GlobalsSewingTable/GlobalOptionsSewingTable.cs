using System;
using System.Collections.Generic;
using System.Reflection;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Lyralei;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;

namespace Lyralei
{
 	public class GlobalOptionsSewingTable
    {
        static bool HasBeenLoaded = false;

        [Tunable]
        protected static bool kInstantiator = false;
        
        public static AlarmHandle mPatternClubAlarm = AlarmHandle.kInvalidHandle;
        
        public static SuperSetEnum<SkillNames> sSkillEnumValues = new SuperSetEnum<SkillNames>();
        
        private static List<XmlDbData> sDelayedSkillBooks = new List<XmlDbData>();
        
    	[PersistableStatic]
		protected static PersistedData sSettings;
		
		public static PersistedData retrieveData
		{
			get 
			{
				if (GlobalOptionsSewingTable.sSettings == null) 
				{
					GlobalOptionsSewingTable.sSettings = new PersistedData();
				}
				return GlobalOptionsSewingTable.sSettings;
			}
		}
        
        public static bool alreadyParsed = false;

        static GlobalOptionsSewingTable()
        {
            // gets the OnPreload method to run before the whole savegame is loaded so your sim doesn't find
            // the skill missing if they need to access its data

            LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(GlobalOptionsSewingTable.OnPreload);

			if (!GlobalOptionsSewingTable.alreadyParsed)
			{
				LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(GlobalOptionsSewingTable.ParseBooks);
			}
			// LoadSaveManager.ObjectGroupsPostLoad += new ObjectGroupsPostLoadHandler(Pattern.OnPostWorldLoad);
			
			alreadyParsed = true;
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
            World.OnWorldQuitEventHandler += new EventHandler(OnWorldQuit);
        }

        static void OnPreload()
        {
        	try 
        	{
        		ObjectLoader.GetAllXMLSettingsForSewables();
        	}
        	catch(Exception ex2)
        	{
             	print("There was a problem, couldn't get the sewables settings \n \n" + ex2.ToString());
                return;       		
        	}
			try
            {	
				bool bStore = true;
                if (HasBeenLoaded) return; // you only want to run it once per gameplay session
                HasBeenLoaded = true;

                // fill this in with the resourcekey of your SKIL xml
                XmlDbData data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("SewingSkill"), 0xA8D58BE5, 0x0), false);
                if (data == null)
                {
                    return;
                }
				if (data == null || data.Tables == null) 
				{
					return;
				}
				XmlDbTable xmlDbTable = null;
				data.Tables.TryGetValue("SkillList", out xmlDbTable);
					if (xmlDbTable == null) {
						return;
					}
					foreach (XmlDbRow current in xmlDbTable.Rows) {
					bool flag = false;
					string @string = current.GetString("Hex");
					SkillNames guid = (SkillNames)SkillManager.GetGuid(ref @string, bStore);
					if (guid == SkillNames.None) {
						flag = true;
					}
					ProductVersion productVersion = ProductVersion.BaseGame;
					double skillVersion = 0.0;
					if (!flag) {
						Skill skill = null;
						string string2 = current.GetString("CustomClassName");
						bool flag2 = string2.Length > 0;
						if (flag2) {
							Type type = null;
							if (bStore) {
								string[] array = string2.Split(new char[] {
									','
								});
								if (array.Length < 2) {
									flag = true;
								}
								else {
									type = Type.GetType(array[0] + ",Sims3StoreObjects");
								}
							}
							if (type == null) {
								type = Type.GetType(string2);
							}
							if (type == null) {
								flag = true;
							}
							else {
								object[] array2 = new object[] {
									guid
								};
								ConstructorInfo constructor = type.GetConstructor(Type.GetTypeArray(array2));
								if (constructor == null) {
									flag = true;
								}
								else {
									skill = (constructor.Invoke(array2) as Skill);
									if (skill == null) {
										flag = true;
									}
								}
							}
						}
						else {
							skill = new Skill(guid);
						}
						if (!flag) {
							Skill.NonPersistableSkillData nonPersistableSkillData = new Skill.NonPersistableSkillData();
							skill.NonPersistableData = nonPersistableSkillData;
							uint num = ResourceUtils.ProductVersionToGroupId(productVersion);
							nonPersistableSkillData.SkillProductVersion = productVersion;
							nonPersistableSkillData.Name = "Gameplay/Excel/Skills/SkillList:" + current.GetString("SkillName");
							string string3 = current.GetString("SkillDescription");
							nonPersistableSkillData.Description = (string.IsNullOrEmpty(string3) ? "" : ("Gameplay/Excel/Skills/SkillList:" + string3));
							nonPersistableSkillData.MaxSkillLevel = current.GetInt("MaxSkillLevel", 0);
							skill.Guid = guid;
							nonPersistableSkillData.ThoughtBalloonTopicString = current.GetString("ThoughtBalloonTopic");
							string string4 = current.GetString("IconKey");
							nonPersistableSkillData.IconKey = ResourceKey.CreatePNGKey(string4, num);
							nonPersistableSkillData.SkillUIIconKey = ResourceKey.CreatePNGKey(current.GetString("SkillUIIcon"), num);
							string string5 = current.GetString("Commodity");
							CommodityKind commodity = (CommodityKind)SkillManager.GetCommodity(string5, bStore);
							
							nonPersistableSkillData.Commodity = commodity;
							if (commodity != CommodityKind.None) {
								SkillManager.SkillCommodityMap[commodity] = guid;
							}
							if (bStore) {
								skillVersion = Convert.ToDouble(current.GetString("Version"));
							}
							nonPersistableSkillData.SkillVersion = skillVersion;
							if (current.GetBool("Physical")) {
								skill.AddCategoryToSkill(SkillCategory.Physical);
							}
							if (current.GetBool("Mental")) {
								skill.AddCategoryToSkill(SkillCategory.Mental);
							}
							if (current.GetBool("Musical")) {
								skill.AddCategoryToSkill(SkillCategory.Musical);
							}
							if (current.GetBool("Creative")) {
								skill.AddCategoryToSkill(SkillCategory.Creative);
							}
							if (current.GetBool("Artistic")) {
								skill.AddCategoryToSkill(SkillCategory.Artistic);
							}
							if (current.GetBool("Hidden")) {
								skill.AddCategoryToSkill(SkillCategory.Hidden);
							}
							if (current.GetBool("Certificate")) {
								skill.AddCategoryToSkill(SkillCategory.Certificate);
							}
							if (current.Exists("HiddenWithSkillProgress") && current.GetBool("HiddenWithSkillProgress") && skill.HasCategory(SkillCategory.Hidden)) {
								skill.AddCategoryToSkill(SkillCategory.HiddenWithSkillProgress);
							}
							if (current.GetBool("CanDecay")) {
								skill.AddCategoryToSkill(SkillCategory.CanDecay);
							}
							int[] array3 = new int[skill.MaxSkillLevel];
							int num2 = 0;
							for (int i = 1; i <= skill.MaxSkillLevel; i++) {
								string column = "Level_" + i.ToString();
								num2 += current.GetInt(column, 0);
								array3[i - 1] = num2;
							}
							nonPersistableSkillData.PointsForNextLevel = array3;
							nonPersistableSkillData.AlwaysDisplayLevelUpTns = current.GetBool("AlwaysDisplayTNS");
							string[] array4 = new string[skill.MaxSkillLevel + 1];
							for (int j = 2; j <= skill.MaxSkillLevel; j++) {
								string column2 = "Level_" + j.ToString() + "_Text";
								array4[j - 1] = current.GetString(column2);
								if (array4[j - 1] != string.Empty) {
									array4[j - 1] = "Gameplay/Excel/Skills/SkillList:" + array4[j - 1];
								}
							}
							array4[skill.MaxSkillLevel] = current.GetString("Level_10_Text_Alternate");
							if (array4[skill.MaxSkillLevel] != string.Empty) {
								array4[skill.MaxSkillLevel] = "Gameplay/Excel/Skills/SkillList:" + array4[skill.MaxSkillLevel];
							}
							nonPersistableSkillData.LevelUpStrings = array4;
							if (flag2) {
								XmlDbTable xmlDbTable2 = null;
								string string6 = current.GetString("CustomDataSheet");
								data.Tables.TryGetValue(string6, out xmlDbTable2);
								if (xmlDbTable2 == null && string6.Length > 0) {
									flag = true;
									skill = null;
								}
								else {
									if (!skill.ParseSkillData(xmlDbTable2)) {
										flag = true;
										skill = null;
									}
								}
							}
							nonPersistableSkillData.AvailableAgeSpecies = ParserFunctions.ParseAllowableAgeSpecies(current, "AvailableAgeSpecies");
							nonPersistableSkillData.DreamsAndPromisesIcon = current.GetString("DreamsAndPromisesIcon");
							nonPersistableSkillData.DreamsAndPromisesIconKey = ResourceKey.CreatePNGKey(nonPersistableSkillData.DreamsAndPromisesIcon, num);
							nonPersistableSkillData.LogicSkillBoost = current.GetBool("LogicSkillBoost");
							if (!flag) {
								if (GenericManager<SkillNames, Skill, Skill>.sDictionary.ContainsKey((ulong)guid)) {
									if (GenericManager<SkillNames, Skill, Skill>.sDictionary[(ulong)guid].SkillVersion < skill.SkillVersion) {
										GenericManager<SkillNames, Skill, Skill>.sDictionary[(ulong)guid] = skill;
									}
								}
								else {
									GenericManager<SkillNames, Skill, Skill>.sDictionary.Add((ulong)guid, skill);
									SkillManager.sSkillEnumValues.AddNewEnumValue(@string, guid);
								}
							}
						}
					}
				}
            }
            catch (Exception ex)
            {
            	print("In Preload - caught issue");
                return;
            }
	    }
        public static Dictionary<string, BookSkillData> BookSkillDataListLyralei = new Dictionary<string, BookSkillData>();
        
        public static void OnWorldLoadFinished(object sender, EventArgs e)
	    {
    		ObjectLoader.GetAllSimulationObjectKeysForDialogue();
        	foreach (Computer computer in Sims3.Gameplay.Queries.GetObjects<Computer>())
			{
				AddInteractions(computer);
			}
        	
        	// Save our loaded key to make discovery quicker. Rather than always looping through it. 
        	for(int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
   			{
        		Pattern.mStoredPatternsKeySettingsList.Add(ObjectLoader.sewableSettings[i].key);
        	}
        	
        	
		   	mPatternClubAlarm = AlarmManager.Global.AddAlarmDay(1f, DaysOfTheWeek.Thursday, GlobalOptionsSewingTable.SendPatterns, "Mailbox:  Pattern club", AlarmType.NeverPersisted, null);
        	EventTracker.AddListener(EventTypeId.kBoughtObject, new
			ProcessEventDelegate(OnObjectChanged));
			EventTracker.AddListener(EventTypeId.kInventoryObjectAdded, new
			ProcessEventDelegate(OnObjectChanged));
			EventTracker.AddListener(EventTypeId.kObjectStateChanged, new
			ProcessEventDelegate(OnObjectChanged));
        }
        
        public static void OnWorldQuit(object sender, EventArgs e)
        {
        	AlarmManager.Global.RemoveAlarm(mPatternClubAlarm);
			mPatternClubAlarm = AlarmHandle.kInvalidHandle;
        }
        
        public static void ParseBooks()
		{
			string text = "BooksSewable";
			XmlDbData xmlDbData = XmlDbData.ReadData(text);
			if (xmlDbData != null && xmlDbData.Tables != null)
			{
					sDelayedSkillBooks.Add(xmlDbData);
					BookData.LoadBookData(xmlDbData, "BookSkill", BookData.BookType.Skill);
					bool loaded = true;
					if(loaded)
					{
						Bookstore.mItemDictionary.Clear();
						Bookstore.LoadData();
					}
			}
			else
			{
				SimpleMessageDialog.Show("[DEBUG] Arsil's CustomBookLoader", "Resource " + text + " of type _XML not found or with no data defined!");
			}
		}
        
        public static void SendPatterns()
		{
			if (!Sims3.SimIFace.Environment.HasEditInGameModeSwitch && !GameUtils.IsOnVacation())
			{
				Household activeHousehold = Household.ActiveHousehold;
				if (activeHousehold != null)
				{
					Mailbox mailboxOnLot = Mailbox.GetMailboxOnLot(activeHousehold.LotHome);
					if (mailboxOnLot != null)
					{
						foreach (Sim sim in activeHousehold.Sims)
						{
							SimDescription desc = sim.SimDescription;
							if(SewingSkill.isInPatternClub(desc))
							{
								Pattern randomPattern = Pattern.DiscoverPatternForGlobalObjects(sim);
								if(randomPattern != null)
								{
									mailboxOnLot.AddMail(randomPattern, false);
								}
								else {
									randomPattern.Destroy();
								}
							}
						}
					}
				}
			}
		}
        
        
		private static void AddInteractions(Computer computer)
		{
			foreach (InteractionObjectPair interaction in computer.Interactions)
			{
				if (interaction.InteractionDefinition.GetType() == BrowseSewingDIY.Singleton.GetType() || (interaction.InteractionDefinition.GetType() == JoinPatternClub.Singleton.GetType()) || (interaction.InteractionDefinition.GetType() == LeavePatternClub.Singleton.GetType()))
				{
					return;
				}
			}
			computer.AddInteraction(BrowseSewingDIY.Singleton);
			computer.AddInteraction(JoinPatternClub.Singleton);
			computer.AddInteraction(LeavePatternClub.Singleton);
		}
		
		private static ListenerAction OnObjectChanged(Event e)
		{
			try
			{
				Computer computer = e.TargetObject as Computer;
				if (computer != null)
				{
					AddInteractions(computer);
				}
			}
			catch (Exception)
			{
			}
			return ListenerAction.Keep;
		}
	
		public static void print(string text)
		{
			SimpleMessageDialog.Show("Lyralei's Sewing Table:", text);
		}
    }
}
