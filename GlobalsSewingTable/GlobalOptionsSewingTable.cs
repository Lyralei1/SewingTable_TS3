using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Lyralei;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Lighting;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;

namespace Lyralei
{
    public class GlobalOptionsSewingTable
    {
        static bool HasBeenLoaded = false;

        [Tunable]
        protected static bool kInstantiator = false;

        [Tunable]
        public static bool mShouldSimChangeAfterGift = true;

        public static AlarmHandle mPatternClubAlarm = AlarmHandle.kInvalidHandle;

        public static AlarmHandle mWearClothing = AlarmHandle.kInvalidHandle;

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

            try
            {
                if (!alreadyParsed)
                {
                    LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(OnPreload);
                    LoadSaveManager.ObjectGroupsPreLoad += new ObjectGroupsPreLoadHandler(ParseBooks);
                }
                alreadyParsed = true;
                World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
                World.OnWorldQuitEventHandler += new EventHandler(OnWorldQuit);
            }
            catch(Exception ex)
            {
                print(ex.Message + ex.Source.ToString());
            }
        }

        static void OnPreload()
        {
            try
            {
                ObjectLoader.GetAllXMLSettingsForSewables();
                XmlDbData data = XmlDbData.ReadData(new ResourceKey(ResourceUtils.HashString64("tutorialSewingTable"), 0x0333406C, 0x0), false);
                ParseLessons(data);
            }
            catch (Exception ex2)
            {
                print("There was a problem, couldn't get the sewables settings \n \n" + ex2.Message.ToString());
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
                if (xmlDbTable == null)
                {
                    return;
                }
                foreach (XmlDbRow current in xmlDbTable.Rows)
                {
                    bool flag = false;
                    string @string = current.GetString("Hex");
                    SkillNames guid = (SkillNames)SkillManager.GetGuid(ref @string, bStore);
                    if (guid == SkillNames.None)
                    {
                        flag = true;
                    }
                    ProductVersion productVersion = ProductVersion.BaseGame;
                    double skillVersion = 0.0;
                    if (!flag)
                    {
                        Skill skill = null;
                        string string2 = current.GetString("CustomClassName");
                        bool flag2 = string2.Length > 0;
                        if (flag2)
                        {
                            Type type = null;
                            if (bStore)
                            {
                                string[] array = string2.Split(new char[] {
                                    ','
                                });
                                if (array.Length < 2)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    type = Type.GetType(array[0] + ",Sims3StoreObjects");
                                }
                            }
                            if (type == null)
                            {
                                type = Type.GetType(string2);
                            }
                            if (type == null)
                            {
                                flag = true;
                            }
                            else
                            {
                                object[] array2 = new object[] {
                                    guid
                                };
                                ConstructorInfo constructor = type.GetConstructor(Type.GetTypeArray(array2));
                                if (constructor == null)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    skill = (constructor.Invoke(array2) as Skill);
                                    if (skill == null)
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            skill = new Skill(guid);
                        }
                        if (!flag)
                        {
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
                            if (commodity != CommodityKind.None)
                            {
                                SkillManager.SkillCommodityMap[commodity] = guid;
                            }
                            if (bStore)
                            {
                                skillVersion = Convert.ToDouble(current.GetString("Version"));
                            }
                            nonPersistableSkillData.SkillVersion = skillVersion;
                            if (current.GetBool("Physical"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Physical);
                            }
                            if (current.GetBool("Mental"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Mental);
                            }
                            if (current.GetBool("Musical"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Musical);
                            }
                            if (current.GetBool("Creative"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Creative);
                            }
                            if (current.GetBool("Artistic"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Artistic);
                            }
                            if (current.GetBool("Hidden"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Hidden);
                            }
                            if (current.GetBool("Certificate"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.Certificate);
                            }
                            if (current.Exists("HiddenWithSkillProgress") && current.GetBool("HiddenWithSkillProgress") && skill.HasCategory(SkillCategory.Hidden))
                            {
                                skill.AddCategoryToSkill(SkillCategory.HiddenWithSkillProgress);
                            }
                            if (current.GetBool("CanDecay"))
                            {
                                skill.AddCategoryToSkill(SkillCategory.CanDecay);
                            }
                            int[] array3 = new int[skill.MaxSkillLevel];
                            int num2 = 0;
                            for (int i = 1; i <= skill.MaxSkillLevel; i++)
                            {
                                string column = "Level_" + i.ToString();
                                num2 += current.GetInt(column, 0);
                                array3[i - 1] = num2;
                            }
                            nonPersistableSkillData.PointsForNextLevel = array3;
                            nonPersistableSkillData.AlwaysDisplayLevelUpTns = current.GetBool("AlwaysDisplayTNS");
                            string[] array4 = new string[skill.MaxSkillLevel + 1];
                            for (int j = 2; j <= skill.MaxSkillLevel; j++)
                            {
                                string column2 = "Level_" + j.ToString() + "_Text";
                                array4[j - 1] = current.GetString(column2);
                                if (array4[j - 1] != string.Empty)
                                {
                                    array4[j - 1] = "Gameplay/Excel/Skills/SkillList:" + array4[j - 1];
                                }
                            }
                            array4[skill.MaxSkillLevel] = current.GetString("Level_10_Text_Alternate");
                            if (array4[skill.MaxSkillLevel] != string.Empty)
                            {
                                array4[skill.MaxSkillLevel] = "Gameplay/Excel/Skills/SkillList:" + array4[skill.MaxSkillLevel];
                            }
                            nonPersistableSkillData.LevelUpStrings = array4;
                            if (flag2)
                            {
                                XmlDbTable xmlDbTable2 = null;
                                string string6 = current.GetString("CustomDataSheet");
                                data.Tables.TryGetValue(string6, out xmlDbTable2);
                                if (xmlDbTable2 == null && string6.Length > 0)
                                {
                                    flag = true;
                                    skill = null;
                                }
                                else
                                {
                                    if (!skill.ParseSkillData(xmlDbTable2))
                                    {
                                        flag = true;
                                        skill = null;
                                    }
                                }
                            }
                            nonPersistableSkillData.AvailableAgeSpecies = ParserFunctions.ParseAllowableAgeSpecies(current, "AvailableAgeSpecies");
                            nonPersistableSkillData.DreamsAndPromisesIcon = current.GetString("DreamsAndPromisesIcon");
                            nonPersistableSkillData.DreamsAndPromisesIconKey = ResourceKey.CreatePNGKey(nonPersistableSkillData.DreamsAndPromisesIcon, num);
                            nonPersistableSkillData.LogicSkillBoost = current.GetBool("LogicSkillBoost");
                            if (!flag)
                            {
                                if (GenericManager<SkillNames, Skill, Skill>.sDictionary.ContainsKey((ulong)guid))
                                {
                                    if (GenericManager<SkillNames, Skill, Skill>.sDictionary[(ulong)guid].SkillVersion < skill.SkillVersion)
                                    {
                                        GenericManager<SkillNames, Skill, Skill>.sDictionary[(ulong)guid] = skill;
                                    }
                                }
                                else
                                {
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
                print("In Preload - caught issue" + ex.ToString());
                return;
            }
        }

        public static void ParseLessons(XmlDbData data)
        {
            bool flag = DeviceConfig.IsMac();
            XmlDbTable xmlDbTable = null;
            data.Tables.TryGetValue("Tutorialettes", out xmlDbTable);
            //Tutorialette.Tutorialettes = new List<TutorialetteDialog.TutorialetteData>();
            //Tutorialette.sIgnoreGlobalCooldown = new Dictionary<Lessons, bool>();
            List<TutorialetteDialog.TutorialettePage> list = null;
            //Tutorialette.sLessonTnsKeys = new Dictionary<Lessons, LessonTNSData>();
            foreach (XmlDbRow row in xmlDbTable.Rows)
            {
                ProductVersion productVersion;
                row.TryGetEnum("EPValid", out productVersion, ProductVersion.BaseGame);
                if (GameUtils.IsInstalled(productVersion))
                {
                    if (!string.IsNullOrEmpty(row["LessonKey"]))
                    {
                        //print("Lesson defaulted into: " + lessons.ToString());
                        string lessonTnsKey = "Gameplay/Excel/Tutorial/Tutorialettes:" + row["TnsKey"];
                        LessonTNSData value = new LessonTNSData(lessonTnsKey, productVersion);
                        Tutorialette.sLessonTnsKeys.Add((Lessons)207, value);
                        list = new List<TutorialetteDialog.TutorialettePage>();
                        Tutorialette.Tutorialettes.Add(new TutorialetteDialog.TutorialetteData("Gameplay/Excel/Tutorial/Tutorialettes:" + row["LessonName"], list, (int)207, (ulong)productVersion));
                        Tutorialette.sIgnoreGlobalCooldown.Add((Lessons)207, ParserFunctions.ParseBool(row["IgnoreGlobalCooldown"]));
                    }
                    if (list != null)
                    {
                        string text;
                        if (flag)
                        {
                            text = row["PageTextMac"];
                            if (string.IsNullOrEmpty(text))
                            {
                                text = row["PageText"];
                            }
                        }
                        else
                        {
                            text = row["PageText"];
                        }
                        list.Add(new TutorialetteDialog.TutorialettePage("Gameplay/Excel/Tutorial/Tutorialettes:" + text, row["PageImage"]));
                    }
                }
            }
        }


        public static Dictionary<string, BookSkillData> BookSkillDataListLyralei = new Dictionary<string, BookSkillData>();

        public static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            ObjectLoader.FindAndSortAllExistingSewables();

            for (int i = 0; i < Sims3.Gameplay.Queries.GetObjects<PhoneSmart>().Length; i++)
            {
                if (Sims3.Gameplay.Queries.GetObjects<PhoneSmart>()[i] != null)
                {
                    AddInteractionsPhone(Sims3.Gameplay.Queries.GetObjects<PhoneSmart>()[i]);
                }
            }

            foreach (Computer computer in Sims3.Gameplay.Queries.GetObjects<Computer>())
            {
                if (computer != null)
                {

                    AddInteractionsComputer(computer);
                }
            }

            // Save/cache our loaded key to make discovery quicker. Rather than always looping through it. 
            for (int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
            {
                Pattern.mStoredPatternsKeySettingsList.Add(ObjectLoader.sewableSettings[i].key);
            }
            mPatternClubAlarm = AlarmManager.Global.AddAlarmDay(1f, DaysOfTheWeek.Thursday, GlobalOptionsSewingTable.SendPatterns, "Mailbox:  Pattern club", AlarmType.NeverPersisted, null);

            //mWearClothing = AlarmManager.Global.AddAlarmRepeating(24f, TimeUnit.Hours, WearGiftedClothing, 1f, TimeUnit.Days, "Wear gifted clothing", AlarmType.AlwaysPersisted, null);

            EventTracker.AddListener(EventTypeId.kBoughtObject, new ProcessEventDelegate(OnObjectChanged));
            EventTracker.AddListener(EventTypeId.kInventoryObjectAdded, new ProcessEventDelegate(OnObjectChanged));
            EventTracker.AddListener(EventTypeId.kObjectStateChanged, new ProcessEventDelegate(OnObjectChanged));
        }

        //public static void SetUpAlarmForGiftedItems()
        //{
        //    print("Setting up alarm");
        //    foreach (KeyValuePair<Sim, List<Pattern>> keyvalues in retrieveData.mGiftableClothing)
        //    {
        //        alarmPatterns


        //        // If sim exists and was saved properly, and there are more than one patterns...
        //        if (keyvalues.Key != null && keyvalues.Value.Count >= 1)
        //        {
        //            print("Sim was found!");
        //            SimDescription simdesc = keyvalues.Key.mSimDescription;
        //            Sim sim = keyvalues.Key;

        //            // get a random pattern from the list. Especially good if they have a few dozen patterns they can wear :p
        //            Pattern randomPattern = RandomUtil.GetRandomObjectFromList(keyvalues.Value);
        //            print(randomPattern.GetResourceKey().ToString());
        //            // Check if the patterns age and gender meet with the stored sim..
        //            if(randomPattern.mPatternInfo.mSimOutfit.Age == simdesc.Age && randomPattern.mPatternInfo.mSimOutfit.Gender == simdesc.Gender)
        //            {
        //                print("Found pattern for age and gender");
        //                ApplyGiftedSewingsToOutfits(randomPattern.mPatternInfo.mSimOutfit, sim);
        //                OutfitCategories currentOutfitCategory = sim.CurrentOutfitCategory;
        //                sim.RefreshCurrentOutfit(true);
        //                sim.SwitchToOutfitWithoutSpin(sim.CurrentOutfitCategory);
        //                print("1 Changing time!");
        //                //Sim.SwitchOutfitHelper mSwitchOutfitHelper = new Sim.SwitchOutfitHelper(sim, currentOutfitCategory, 0);
        //                //if (mSwitchOutfitHelper != null)
        //                //{
        //                //    mSwitchOutfitHelper.Start();
        //                //    mSwitchOutfitHelper.ChangeOutfit();
        //                //    print("Changing time!");
        //                //}
        //            }
        //        }
        //        else
        //        {
        //            continue;
        //        }
        //    }
        //}

        //public static uint[] ClothingCategory =
        //{
        //    2,
        //    4,
        //    8,
        //    16,
        //    32,
        //    256,
        //};

        ////indly stolen from face paint :p
        //public static void ApplyGiftedSewingsToOutfits(CASPart casPart, Sim desc)
        //{
        //    OutfitCategoryMap outfits = desc.mSimDescription.Outfits;
        //    OutfitCategories[] listOfCategories = desc.mSimDescription.ListOfCategories;

        //    List<uint> possibleOutfits = new List<uint>();

        //    foreach(uint cat in ClothingCategory)
        //    {
        //        if((casPart.CategoryFlags & cat) == cat)
        //        {
        //            print("Match made! CAT: " + cat.ToString());
        //            possibleOutfits.Add(cat);
        //        }
        //    }
        //    if (possibleOutfits.Count > 0)
        //    {
        //        uint randomed = RandomUtil.GetRandomObjectFromList(possibleOutfits);
        //        print(randomed.ToString());
        //        //ArrayList arrayList = ((Hashtable)outfits)[randomed] as ArrayList;
        //        //if (arrayList != null)
        //        //{
        //        //    print(arrayList.Count.ToString());
        //        //    SimBuilder simBuilder = new SimBuilder();
        //        //    simBuilder.UseCompression = true;
        //        //    simBuilder.Age = desc.mSimDescription.Age;
        //        //    SimOutfit outfit = arrayList[0] as SimOutfit;
        //        //    //SimOutfit outfit = ((Hashtable)outfits)[randomed] as SimOutfit;
        //        //    print(simBuilder.ToString());
        //        //    OutfitUtils.SetOutfit(simBuilder, outfit, desc.mSimDescription);
        //        //    OutfitUtils.SetAutomaticModifiers(simBuilder);
        //        //    simBuilder.RemoveParts(casPart.BodyType);
        //        //    print(simBuilder.ToString());
        //        //    //OutfitUtils.AddPartAndPreset(simBuilder, casPart, false);
        //        //    //casPart.CategoryFlags = casPart.CategoryFlags - 0x01000000;
        //        //    OutfitUtils.AddPartAndPreset(simBuilder, casPart, false);
        //        //    print(simBuilder.ToString());
        //        //    ResourceKey key = simBuilder.CacheOutfit(string.Format("MakeCategoryOutfitForSewableGiftShared_{0}_{1}_{2}", simBuilder.Age, desc.mSimDescription.FirstName, (OutfitCategories)randomed));
        //        //    print(key.ToString());
        //        //    SimOutfit outfit2;
        //        //    if (OutfitUtils.TryGenerateSimOutfit(key, out outfit2))
        //        //    {
        //        //        desc.mSimDescription.RemoveOutfit((OutfitCategories)randomed, 0, true);
        //        //        desc.mSimDescription.AddOutfit(outfit2, (OutfitCategories)randomed, 0);
        //        //        print("MAde outfit");
        //        //    }
        //        //    print("Should now have added the item!");
        //        //    simBuilder.Dispose();
        //        //}

        //        OutfitCategories cat = OutfitCategories.None;
        //        switch (randomed)
        //        {
        //            case 2:
        //                {
        //                    cat = OutfitCategories.Everyday;
        //                    break;
        //                }
        //            case 4:
        //                {
        //                    cat = OutfitCategories.Formalwear;
        //                    break;
        //                }
        //            case 8:
        //                {
        //                    cat = OutfitCategories.Sleepwear;
        //                    break;
        //                }
        //            case 16:
        //                {
        //                    cat = OutfitCategories.Swimwear;
        //                    break;
        //                }
        //            case 32:
        //                {
        //                    cat = OutfitCategories.Athletic;
        //                    break;
        //                }
        //            case 256:
        //                {
        //                    cat = OutfitCategories.Makeover;
        //                    break;
        //                }
        //            default:
        //                {
        //                    cat = OutfitCategories.Everyday;
        //                    break;
        //                }
        //        }

        //        SimBuilder simBuilder = new SimBuilder();
        //        simBuilder.UseCompression = true;
        //        simBuilder.Age = desc.mSimDescription.Age;
        //        SimOutfit outfit = desc.mSimDescription.GetOutfit(cat, 0);
        //        OutfitUtils.SetOutfit(simBuilder, outfit, desc.mSimDescription);
        //        OutfitUtils.SetAutomaticModifiers(simBuilder);
        //        simBuilder.RemoveParts(casPart.BodyType);
        //        OutfitUtils.AddPartAndPreset(simBuilder, casPart, false);
        //        ResourceKey key = simBuilder.CacheOutfit(string.Format("MakeCategoryOutfitForSewableGiftShared_{0}_{1}_{2}", simBuilder.Age, desc.FirstName, cat));
        //        SimOutfit outfit2;
        //        if (OutfitUtils.TryGenerateSimOutfit(key, out outfit2))
        //        {
        //            desc.mSimDescription.RemoveOutfit(cat, 0, true);
        //            desc.mSimDescription.AddOutfit(outfit2, cat, 0);
        //        }
        //        print("Should now have added the item!");
        //        simBuilder.Dispose();
        //    }
        //}

        public static void OnWorldQuit(object sender, EventArgs e)
        {
            AlarmManager.Global.RemoveAlarm(mPatternClubAlarm);
            mPatternClubAlarm = AlarmHandle.kInvalidHandle;

            AlarmManager.Global.RemoveAlarm(mWearClothing);
            mWearClothing = AlarmHandle.kInvalidHandle;
        }

        public static void TriggerLesson(Lessons lesson, Sim sim)
        {
            if (!IntroTutorial.IsRunning && !Sims3.SimIFace.Environment.HasEditInGameModeSwitch && Tutorialette.AreTutorialTipsEnabled())
            {
                InWorldSubState inWorldSubState = GameStates.GetInWorldSubState();
                if (inWorldSubState != null)
                {
                    string stateName = inWorldSubState.StateName;
                    if (stateName != "Play Flow" && Tutorialette.IsValidLesson(lesson, sim))
                    {
                        TutorialetteNotification.Show(Tutorialette.sLessonTnsKeys[lesson].LessonTnsKey, (int)lesson);
                    }
                }
            }
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
                if (loaded)
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
                            if (SewingSkill.isInPatternClub(desc))
                            {
                                Pattern randomPattern = Pattern.DiscoverPatternForGlobalObjects(sim);
                                if (randomPattern != null)
                                {
                                    mailboxOnLot.AddMail(randomPattern, false);
                                }
                                else
                                {
                                    randomPattern.Destroy();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void AddInteractionsComputer(Computer computer)
        {
            if (computer != null)
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
                computer.AddInteraction(BlogAboutSewing.Singleton);
            }
        }

        public static void AddInteractionsPhone(PhoneSmart phone)
        {
            if (phone != null)
            {
                foreach (InteractionObjectPair interaction in phone.Interactions)
                {
                    if (interaction.InteractionDefinition.GetType() == GottaBlogAboutSewing.Singleton.GetType())
                    {
                        return;
                    }
                }
                if (phone.ItemComp.InteractionsInventory != null)
                {
                    foreach (InteractionObjectPair iop in phone.ItemComp.InteractionsInventory)
                    {
                        if (iop.InteractionDefinition.GetType() == GottaBlogAboutSewing.Singleton.GetType())
                        {
                            return;
                        }
                    }
                }
                phone.AddInteraction(GottaBlogAboutSewing.Singleton);
                phone.AddInventoryInteraction(GottaBlogAboutSewing.Singleton);
            }
        }

        private static ListenerAction OnObjectChanged(Event e)
        {
            Computer computer = e.TargetObject as Computer;
            if (computer != null)
            {
                AddInteractionsComputer(computer);
            }
            PhoneSmart phone = e.TargetObject as PhoneSmart;
            if (phone != null)
            {
                AddInteractionsPhone(phone);
            }
            PhoneFuture phone5 = e.TargetObject as PhoneFuture;
            if (phone != null)
            {
                AddInteractionsPhone(phone5);
            }
            return ListenerAction.Keep;
        }

        public static void print(string text)
        {
            SimpleMessageDialog.Show("Lyralei's Sewing Table:", text);
        }
    }
}