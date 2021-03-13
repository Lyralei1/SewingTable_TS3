using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Lyralei;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;

namespace Sims3.Gameplay.Lyralei
{
    /// <summary>
    /// Description of ObjectLoader.
    /// </summary>
    public class ObjectLoader
    {
        public static List<ResourceKey> EasySewablesList = new List<ResourceKey>();
        public static List<ResourceKey> MediumSewablesList = new List<ResourceKey>();
        public static List<ResourceKey> HardSewablesList = new List<ResourceKey>();
        public static List<ResourceKey> HarderSewablesList = new List<ResourceKey>();
        public static List<ResourceKey> MagicHarderSewablesList = new List<ResourceKey>();

        public static ulong[] AllObjectKeys = ScriptCore.Simulator.Simulator_GetAllSimulationObjectKeysImpl();

        public static List<ResourceKey> easySewables = new List<ResourceKey>()
        {
			//Rug traditional BG
			new ResourceKey(0x0000000000000E10, 0x319E4F1D, 0x00000000),
			//Rug traditional Square 4x4
			new ResourceKey(0x0000000000000F64, 0x319E4F1D, 0x00000000),
			//Rug traditional square 3x2
			new ResourceKey(0x0000000000000541, 0x319E4F1D, 0x00000000),
			//Rug Traditional Runner 4x1
			new ResourceKey(0x00000000000006FC, 0x319E4F1D, 0x00000000),
			//Rug Modern Square 2x1
			new ResourceKey(0x00000000000006FA, 0x319E4F1D, 0x00000000),
			//Rug ModernRound1x1
			new ResourceKey(0x0000000000000F63, 0x319E4F1D, 0x00000000),
			// RugMissionSquare3x2
			new ResourceKey(0x0000000000000E0D, 0x319E4F1D, 0x00000000),
			// RugCountryRound2x2
			new ResourceKey(0x0000000000000699, 0x319E4F1D, 0x00000000),
			//Rug CountryOval2x1
			new ResourceKey(0x000000000000058D, 0x319E4F1D, 0x00000000),
			//Rug ContemporaryOval 1x1
			new ResourceKey(0x0000000000000E0F, 0x319E4F1D, 0x00000000),
			//Rug ContemporaryHalfRound2x2
			new ResourceKey(0x0000000000000E0E, 0x319E4F1D, 0x00000000),
			//Rug ContemporaryHalfRound1x1
			new ResourceKey(0x00000000000006F9, 0x319E4F1D, 0x00000000),
			//RugUnivBrainiac2x2
			new ResourceKey(0x000000000098DD49, 0x319E4F1D, 0x88000000),
			//sculptureWallPennantsTileable
			new ResourceKey(0x000000000098DD0B, 0x319E4F1D, 0x88000000)
        };

        public static List<ResourceKey> mediumSewables = new List<ResourceKey>()
        {
			//Teddy bear BG
			new ResourceKey(0x0000000000000592, 0x319E4F1D, 0x00000000),
			//Teddy bear mummy
			new ResourceKey(0x0000000000989B60, 0x319E4F1D, 0x08000000),
			//teddy bear AntiqueImaginaryFriend
			new ResourceKey(0x000000000098D785, 0x319E4F1D, 0x70000000),
			//TeddyBearAntiqueFairy
			new ResourceKey(0x000000000098D91E, 0x319E4F1D, 0x70000000),
			//TeddyAntiqueMummy
			new ResourceKey(0x000000000098D788, 0x319E4F1D, 0x70000000),
			//TeddyBearAntiqueSimbot
			new ResourceKey(0x000000000098D789, 0x319E4F1D, 0x70000000),
			//TeddyBearAntiqueVampire
			new ResourceKey(0x000000000098D787, 0x319E4F1D, 0x70000000),
			//TeddyBearAntiqueWereWolf
			new ResourceKey(0x000000000098D8F9, 0x319E4F1D, 0x70000000),
			//TeddyBearAntiqueWizard
			new ResourceKey(0x000000000098D8FA, 0x319E4F1D, 0x70000000),
			//RugDarkLuxNaturalHide3x3
			new ResourceKey(0x000000000098D884, 0x319E4F1D, 0x70000000),
			//RugAntiqueFreezeBunny2x2
			new ResourceKey(0x000000000098DA13, 0x319E4F1D, 0x70000000),
			//Rug flower
			new ResourceKey(0x000000000098A59C, 0x319E4F1D, 0x38000000),
			//TablePlushDuck
			new ResourceKey(0x000000000098AF14, 0x319E4F1D, 0x48000000),
			//TeddybearAntiqueGenie
			new ResourceKey(0x000000000098D786, 0x319E4F1D, 0x70000000),
			//TeddybearSeasonsSnowman
			new ResourceKey(0x000000000098B1F3, 0x319E4F1D, 0x78000000),
			//RugAntique4x4
			new ResourceKey(0x000000000098DA14, 0x319E4F1D, 0x70000000),
			//RugSeasonButterflyKids3x3
			new ResourceKey(0x000000000098B1DD, 0x319E4F1D, 0x78000000),
			//RugUniSports2x3
			new ResourceKey(0x000000000098DCEC, 0x319E4F1D, 0x88000000),
			//RugUniSports2x2
			new ResourceKey(0x000000000098DD01, 0x319E4F1D, 0x88000000),
			//sculptureWallunivFlagLlama2x1
			new ResourceKey(0x000000000098DD10, 0x319E4F1D, 0x88000000),
        };

        public static List<ResourceKey> hardSewables = new List<ResourceKey>()
        {
			//FloorPillowGen2x2
			new ResourceKey(0x000000000098A684, 0x319E4F1D, 0x38000000),
			//chairLivingBeanBag
			new ResourceKey(0x000000000098A7BD, 0x319E4F1D, 0x38000000),
			//TableGenUnicorn
			new ResourceKey(0x000000000098A750, 0x319E4F1D, 0x38000000),
			//TableGenPlushDragon
			new ResourceKey(0x000000000098A65D, 0x319E4F1D, 0x38000000),
			//TablePlushUnicorn
			new ResourceKey(0x000000000098AF80, 0x319E4F1D, 0x48000000),
			//TablePlushGiraffe
			new ResourceKey(0x000000000098A7F3, 0x319E4F1D, 0x48000000),
			//PetbedLarge
			new ResourceKey(0x000000000098AA06, 0x319E4F1D, 0x48000000),
			//PetBedSmall
			new ResourceKey(0x000000000098AA35, 0x319E4F1D, 0x48000000),
        };

        public static List<ResourceKey> hardestSewables = new List<ResourceKey>()
        {
			//backpackCollege
			new ResourceKey(0x000000000098DC43, 0x319E4F1D, 0x88000000),
        };
        public static List<ResourceKey> magicHardestSewables = new List<ResourceKey>()
        {
			//magicGnomeTeddyLaundry
			new ResourceKey(0x0000000000989EDF, 0x319E4F1D, 0x18000000),
			//ImaginaryFriendDoll
			new ResourceKey(0x000000000098A4D7, 0x319E4F1D, 0x38000000),
        };

        /*
		 * REFERENCE GROUPIDS FOR 'customSewableGroup' LIST:
		 * Easy sewables GroupID:   		0x1001A575
		 * Medium Sewables GroupID: 		0x74031C5A
		 * Hard Sewables GroupID:   		0x86C8D040
		 * Hardest Sewables GroupID: 		0x74322790
		 * MagicHardestSewables GroupID: 	0x21BC2F49		
		*/
        public static List<uint> customSewableGroups = new List<uint>()
        {
            0x1001A575,
            0x74031C5A,
            0x86C8D040,
            0x74322790,
            0x21BC2F49,
        };

        public static void FindAndSortAllExistingSewables()
        {
            int num = AllObjectKeys.Length;
            ResourceKey[] array2 = new ResourceKey[num];
            ResourceKey sewableKey = new ResourceKey();

            //Filter out our defined easy sewable objects
            for (int i = 0; i < easySewables.Count; i++)
            {
                //Check for easy
                if (Sims3.SimIFace.Simulator.CountResources(easySewables[i]) > 0)
                {
                    //GlobalOptionsSewingTable.print(easySewables[i].ToString());
                    EasySewablesList.Add(easySewables[i]);
                }
            }
            //Filter out our defined medium sewable objects
            for (int i = 0; i < mediumSewables.Count; i++)
            {
                //Check for easy
                if (Sims3.SimIFace.Simulator.CountResources(mediumSewables[i]) > 0)
                {
                    //GlobalOptionsSewingTable.print(mediumSewables[i].ToString());
                    MediumSewablesList.Add(mediumSewables[i]);
                }
            }

            //Filter out our defined hard sewable objects
            for (int i = 0; i < hardSewables.Count; i++)
            {
                //Check for easy
                if (Sims3.SimIFace.Simulator.CountResources(hardSewables[i]) > 0)
                {
                    HardSewablesList.Add(hardSewables[i]);
                }
            }

            //Filter out our defined hardest sewable objects
            for (int i = 0; i < hardestSewables.Count; i++)
            {
                //Check for easy
                if (Sims3.SimIFace.Simulator.CountResources(hardestSewables[i]) > 0)
                {
                    HarderSewablesList.Add(hardestSewables[i]);
                }
            }

            //Filter out our defined hardest MAGIC sewable objects
            for (int i = 0; i < magicHardestSewables.Count; i++)
            {
                //Check for easy
                if (Sims3.SimIFace.Simulator.CountResources(magicHardestSewables[i]) > 0)
                {
                    MagicHarderSewablesList.Add(magicHardestSewables[i]);
                }
            }

            //Filter out the Custom objects made by CC creators.
            for (int i = 0; i < num; i++)
            {
                sewableKey = new ResourceKey(AllObjectKeys[i], 0x319E4F1D, 0x0000000);
                if (Sims3.SimIFace.Simulator.CountResources(sewableKey) == 0)
                {
                    /* CUSTOM GROUPS:
					 * Easy sewables GroupID:   		0x1001A575
					 * Medium Sewables GroupID: 		0x74031C5A
					 * Hard Sewables GroupID:   		0x86C8D040
					 * Hardest Sewables GroupID: 		0x74322790
					 * MagicHardestSewables GroupID: 	0x21BC2F49
					*/

                    for (int j = 0; j < customSewableGroups.Count; j++)
                    {
                        sewableKey.GroupId = customSewableGroups[j];
                        if (Sims3.SimIFace.Simulator.CountResources(sewableKey) > 0)
                        {
                            if (j == 0) EasySewablesList.Add(sewableKey);
                            if (j == 1) MediumSewablesList.Add(sewableKey);
                            if (j == 2) HardSewablesList.Add(sewableKey);
                            if (j == 3) HarderSewablesList.Add(sewableKey);
                            if (j == 4) MagicHarderSewablesList.Add(sewableKey);
                            break;
                        }
                    }
                }
            }
        }

        public class sewableSetting
        {
            public ResourceKey key;
            public List<SewingSkill.FabricType> typeFabric = new List<SewingSkill.FabricType>();
            public int amountRemoveFabric = 0;
            public bool isMagicProject = false;
            public bool isDiscoverableOnly = false;
            public bool isClothing = false;
        }
        public List<ResourceKey> SettingsXML = new List<ResourceKey>();

        public static void GetAllXMLSettingsForSewables()
        {
            // Get count/length of all objects in the game
            int num = AllObjectKeys.Length;
            ResourceKey[] array2 = new ResourceKey[num];

            foreach (ulong keys in AllObjectKeys)
            {
                ResourceKey XMLKey = new ResourceKey(keys, 0x0333406C, 0x7354C1FC);
                uint count = Sims3.SimIFace.Simulator.CountResources(XMLKey);
                if (count > 0 && XMLKey != ResourceKey.kInvalidResourceKey)
                {
                    ReadSettingData(XMLKey);
                }
            }

            // Read CASPs, since the top code only checks OBJD instances
            //KeySearch keySearch = new KeySearch(0x034AEECB);
            //foreach (ResourceKey item in keySearch)
            //{
                //ResourceKey XMLKey = new ResourceKey(item.InstanceId, 0x0333406C, 0x7354C1FC);
                ResourceKey XMLKey1 = new ResourceKey(0xDEF5042B349293A7, 0x0333406C, 0x7354C1FC);
                if(XMLKey1 != ResourceKey.kInvalidResourceKey)
                {
                    ReadSettingData(XMLKey1);
                }
                //list.Add(item);
            //}
           // GlobalOptionsSewingTable.print(st.ToString());
        }
        public static List<sewableSetting> sewableSettings = new List<sewableSetting>();

        public static Dictionary<ResourceKey, sewableSetting> dictSettings = new Dictionary<ResourceKey, sewableSetting>();

        public static void ReadSettingData(ResourceKey xmlKey)
        {
            ResourceKey empty = new ResourceKey(0, 0, 0);

            string data = Sims3.SimIFace.Simulator.LoadXMLString(xmlKey);
            if (data == null)
            {
                GlobalOptionsSewingTable.print("Creator Debugger: XML Sewables settings are empty/Don't exist! Try the following: \n 1. Use the Instance of your OBJD and apply that to your XML's instance. \n 2. Make sure that the XML actually exists. \n 3. That the XML file has the group 0x7354C1FC.");
            }
            string[] SettingElements = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int currentEntry = sewableSettings.Count - 1;
            int currentLine = 0;

            foreach (string element in SettingElements)
            {
                if (element == "----")
                {
                    currentEntry++;
                    currentLine = 0;
                    sewableSettings.Add(new sewableSetting());
                }
                if (currentLine == 1)
                {
                    sewableSettings[currentEntry].key = ResourceKey.FromString(element);
                    if (sewableSettings[currentEntry].key == empty || sewableSettings[currentEntry].key == ResourceKey.kInvalidResourceKey)
                    {
                        GlobalOptionsSewingTable.print("Creator Debugger: \n Oops, seems like the OBJD key is incorrect or doesn't exist inside your package/game. Therefore, the mod can't find it! :/ What you entered was: " + sewableSettings[currentEntry].key.ToString() + "\n Make sure of the following: \n 1. Your OBJD Key is correct. \n 2. That, inside S3PE, you highlighted the OBJD and right+clicked and then used the 'Copy resourceKey' Function. And then pasted this inside 'XML_Lyralei_Settings_Sewables'. \n \n \n If all that fails, make sure to contact me (Lyralei at MTS or Greenplumbboblover at tumblr)");
                    }
                }
                if (currentLine == 2)
                {
                    string[] fabrics = element.Split(',');
                    for (int i = 0; i < fabrics.Length; i++)
                    {
                        string type1 = fabrics[i].ToUpper().Trim();

                        // Knitted, Cotton, Satin, Leather, Denim, Synthetic
                        switch (type1)
                        {
                            case "KNITTED":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Knitted);
                                break;
                            case "COTTON":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Cotton);
                                break;
                            case "SATIN":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Satin);
                                break;
                            case "LEATHER":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Leather);
                                break;
                            case "DENIM":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Denim);
                                break;
                            case "SYNTHETIC":
                                sewableSettings[currentEntry].typeFabric.Add(SewingSkill.FabricType.Synthetic);
                                break;
                            // FOR CC CREATORS - An error	
                            default:
                                GlobalOptionsSewingTable.print("Creator Debugger: \n The fabric you used in " + data.ToString() + " isn't correct. What you entered was: " + type1.ToString() + "\n Make sure of the following: \n 1. You've formatted them Uppercase-like (i.e 'Knitted' or 'Cotton', etc) \n 2. It being an existing fabric type inside the mod. Either: (Knitted, Cotton, Satin, Leather, Denim, Synthetic). \n 3. Check for any typos! :) \n 4. If you have multiple, that you differentiate them with comma's ',' without any spaces!. \n \n If all that fails, make sure to contact me (Lyralei at MTS or Greenplumbboblover at tumblr)");
                                break;
                        }
                    }
                }
                if (currentLine == 3)
                {
                    if (element.Contains("isMagicProject="))
                    {
                        string strBool = element.Replace("isMagicProject=", "");
                        bool ToBoolean = Convert.ToBoolean(strBool);
                        sewableSettings[currentEntry].isMagicProject = ToBoolean;
                    }
                }
                if (currentLine == 4)
                {
                    if (element.Contains("isDiscoverableOnly="))
                    {
                        string strBool = element.Replace("isDiscoverableOnly=", "");
                        bool ToBoolean = Convert.ToBoolean(strBool);
                        sewableSettings[currentEntry].isDiscoverableOnly = ToBoolean;
                    }
                }
                if (currentLine == 5)
                {
                    if (element.Contains("amountOfFabricToRemove="))
                    {
                        string strInt = element.Replace("amountOfFabricToRemove=", "");
                        int ToInt = Convert.ToInt16(strInt);
                        sewableSettings[currentEntry].amountRemoveFabric = ToInt;
                        
                    }
                }
                if (currentLine == 6)
                {
                    if (element.Contains("isClothing="))
                    {
                        string strBool = element.Replace("isClothing=", "");
                        bool ToBoolean = Convert.ToBoolean(strBool);
                        sewableSettings[currentEntry].isClothing = ToBoolean;
                    }
                    else
                    {
                        sewableSettings[currentEntry].isClothing = false;
                    }
                }
                // Check for dupes
                if (!dictSettings.ContainsKey(sewableSettings[currentEntry].key))
                {
                    dictSettings.Add(sewableSettings[currentEntry].key, sewableSettings[currentEntry]);
                }
                currentLine++;
            }
        }
    }
}