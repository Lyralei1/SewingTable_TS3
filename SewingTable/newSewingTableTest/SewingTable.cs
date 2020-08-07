using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using ScriptCore;

namespace Sims3.Gameplay.Objects.Lyralei
{
	public class SewingTable : GameObject, IChairOwningSurface
	{
		
		/* Pre-startup
		*  - Skill level (Do later) 
		*  - Progress (Do later) - CHECK-ISH, Bug currently on skilling up
		*/
		
		
		/* SETUP AS OF NOW:
		 * 1. Fabric needed in order to make a project
		 */
		
		
		/*
		 * Find all objects that have the custom group type.
		 * Check if there's an XML file with the correct header.
		 * Find 'need fabric' and 'Need wool' check
		 * Which level it will be unlocked
		 */
		
		private const SkillNames sewingSkillGuid = (SkillNames)0x605205DB;
        
		public bool mHasInitialFabric = false;
			
		public static List<ResourceKey> CustomGameObjectsList = new List<ResourceKey>();
		
		public static List<ResourceKey> EasySewablesList = new List<ResourceKey>();
		public static List<ResourceKey> MediumSewablesList = new List<ResourceKey>();
		public static List<ResourceKey> HardSewablesList = new List<ResourceKey>();
		public static List<ResourceKey> HarderSewablesList = new List<ResourceKey>();
		public static List<ResourceKey> MagicHarderSewablesList = new List<ResourceKey>();
		
        public static float Progress;
        
       	public static SewingTable target2;
       	
       	public static SimDescription mSimDescription;
       	
       	public static Sim ActorCurr;
       	
       	public static List<ResourceKey> allGameSewables =  new List<ResourceKey>();
       
       	public static List<ResourceKey> MergedObjects =  new List<ResourceKey>();
       	
       	public static List<ResourceKey> getAllCachedDialogueObjects
       	{
       		get 
       		{
       			return MergedObjects;
       		}
       	}
       	
       	public static SewingTable_ClothProp mFabric;
       	
       	[TunableComment("Min time it will take to make the min level sewable")]
		public static int kBaseMinTimeMakeSewable = 100;

		[TunableComment("Max time it will take to make the max level sewable")]
		[Tunable]
		public static int kBaseMaxTimeMakeSewable = 200;
		
		[Tunable]
		[TunableComment("Each restock sub pie menu option for restocking N scraps. You can have as many as you like")]
		public static int[] kRestockNumFabric = new int[3]
		{
			5,
			10,
			100
		};
       	
		
		[TunableComment("How many Simoleons a single unit of scrap is worth")]
		[Tunable]
		public static int kFabricUnitValue = 10;
		
//		public static Dictionary<ulong, uint> easySewables = new Dictionary<ulong, uint>()
		public static List<ResourceKey> easySewables = new List<ResourceKey>()
		{
			new ResourceKey(0x0000000000000592, 0x319E4F1D, 0x00000000),
			new ResourceKey(0x000000000098A59D, 0x319E4F1D, 0x38000000),
		};

		/*
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
		
       	//Sewables BASE GAME
       	public static List<string> defaultObjectBG = new List<string>() 
       	{
       		"0x319E4F1D-0x00000000-0x0000000000000592",
       		"0x319E4F1D-0x00000000-0x0000000000000E10",
       		"0x319E4F1D-0x00000000-0x0000000000000F64",
       		"0x319E4F1D-0x00000000-0x0000000000000541",
       		"0x319E4F1D-0x00000000-0x00000000000006FC",
       		"0x319E4F1D-0x00000000-0x00000000000006FA",
       		"0x319E4F1D-0x00000000-0x0000000000000F63",
       		"0x319E4F1D-0x00000000-0x0000000000000E0D",
       		"0x319E4F1D-0x00000000-0x0000000000000699",
       		"0x319E4F1D-0x00000000-0x000000000000058D",
       		"0x319E4F1D-0x00000000-0x0000000000000E0F",
       		"0x319E4F1D-0x00000000-0x0000000000000E0E",
       		"0x319E4F1D-0x00000000-0x00000000000006F9",
       	};
       	
       	//Sewables WorldAdventures
       	public static List<string> defaultObjectWA = new List<string>() 
       	{
       		"0x319E4F1D-0x08000000-0x0000000000989B60",
       	};
       	
       	//Sewables Ambitions
       	public static List<string> defaultObjectAMB = new List<string>() 
       	{
       		"0x319E4F1D-0x00000000-0x0000000000000592",
       	};
       	
        //Sewables Generations
       	public static List<string> defaultObjectGEN = new List<string>() 
       	{
       		"",
       		"0x319E4F1D-0x38000000-0x000000000098A59C",
       		"0x319E4F1D-0x38000000-0x000000000098A684",
       		"0x319E4F1D-0x38000000-0x000000000098A684",
       		"0x319E4F1D-0x38000000-0x000000000098A7BD",
       		"0x319E4F1D-0x38000000-0x000000000098A750",
       		"0x319E4F1D-0x38000000-0x000000000098A65D",
       		"0x319E4F1D-0x38000000-0x000000000098A65A"
       	};
       	
        //Sewables Pets
       	public static List<string> defaultObjectPETS = new List<string>() 
       	{
       		"0x319E4F1D-0x48000000-0x000000000098AF80",
       		"0x319E4F1D-0x48000000-0x000000000098A7F3",
       		"0x319E4F1D-0x48000000-0x000000000098AF14",
       		"0x319E4F1D-0x48000000-0x000000000098AA35",
       		"0x319E4F1D-0x48000000-0x000000000098AA06",
       	};
       	
       	//Sewables Supernatural
       	public static List<string> defaultObjectSN = new List<string>() 
       	{
       		"0x319E4F1D-0x70000000-0x000000000098D785",
       		"0x319E4F1D-0x70000000-0x000000000098D91E",
       		"0x319E4F1D-0x70000000-0x000000000098D786",
       		"0x319E4F1D-0x70000000-0x000000000098D788",
       		"0x319E4F1D-0x70000000-0x000000000098D789",
       		"0x319E4F1D-0x70000000-0x000000000098D787",
       		"0x319E4F1D-0x70000000-0x000000000098D8F9",
       		"0x319E4F1D-0x70000000-0x000000000098D8FA",
       		"0x319E4F1D-0x70000000-0x000000000098D884",
       		"0x319E4F1D-0x70000000-0x000000000098DA13",
       		"0x319E4F1D-0x70000000-0x000000000098DA14",
       	};
       	
       	//Sewables Seasons
       	public static List<string> defaultObjectSEA = new List<string>() 
       	{
       		"0x319E4F1D-0x78000000-0x000000000098B1F3",
       		"0x319E4F1D-0x78000000-0x000000000098B1DD",
       	};
       	
       	//Sewables University
       	public static List<string> defaultObjectUNI = new List<string>() 
       	{
       		"0x319E4F1D-0x88000000-0x000000000098DCEC",
       		"0x319E4F1D-0x88000000-0x000000000098DD01",
       		"0x319E4F1D-0x88000000-0x000000000098DD49",
       		"0x319E4F1D-0x88000000-0x000000000098DC43",
       		"0x319E4F1D-0x88000000-0x000000000098DD10",
       		"0x319E4F1D-0x88000000-0x000000000098DD0B",
       	};
       	
       	
        public static bool IsComplete
		{
			get
			{
				return Progress >= 1f;
			}
		}
        
        public static float mTotalTime;


		public class SewingObjects : Interaction<Sim, SewingTable>
        {
            public class Definition :  InteractionDefinition<Sim, SewingTable, SewingObjects>
            {
                public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair interaction)
                {
                    return "Sew an Object";
                }
                
                public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                	// The code below will check if there's a chair attached to any containment slot. 
                	if (target.GetTotalNumChairsAtTable() < 1)
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Has No Chair");
						return false;
					}
                	if (target.GetNumAvailableChairSlots(actor) == 0 && target.IsActorSittingAtTable(actor))
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Is in Use!");
						return false;
					}
	            	return true;
                }
            }
            
            public static InteractionDefinition Singleton = new Definition();
            
            public SkillNames mSkillName;
            
            public ScriptCore.GameUtils gUtils;
            
            
            //ADD PROP FOR BRACELET WITH PINS
            public override bool Run()
            {
            	if (!base.Target.ScootInActor(base.Actor))
				{
					base.Actor.PlayRouteFailThoughtBalloon(base.Target.GetThumbnailKey());
					base.Actor.AddExitReason(ExitReason.FailedToStart);
					return false;
				}
//            	SewingSkill sewingSkill = base.Actor.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
//            	if (sewingSkill == null)
//				{
//            		print("Failed to load skill");
//					return false;
//				}
				
				target2 		= base.Target;
				mSimDescription = base.Actor.mSimDescription;
				ActorCurr		= base.Actor;
				
//				if(!ShowSewableItemsDialog())
//				{
//					base.Actor.AddExitReason(ExitReason.FailedToStart);
//					return false;
//				}
				//sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
				
            	base.StandardEntry();
				
				mFabric = null;
				Vector3 slotPosition = base.Target.GetSlotPosition(Slot.ContainmentSlot_1);
				Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(Slot.ContainmentSlot_1);
				mFabric = (SewingTable_ClothProp)GlobalFunctions.CreateObject(ResourceKey.FromString("0x319E4F1D:0x00000000:0x0361BDCA336A8546"), slotPosition, 0, forwardOfSlot, null, null);
				
				if(mFabric != null)
				{
					mFabric.ParentToSlot(base.Target, Slot.ContainmentSlot_1);
					base.SetActor("fabric", mFabric);
					print("fabric found");
				}
//				else {
//					base.Actor.PlayRouteFailThoughtBalloon(base.Target.GetThumbnailKey());
//					base.Actor.AddExitReason(ExitReason.FailedToStart);
//					print("Did not found fabric actor");
//					return false;
//				}
				base.SetActor("sewingtable_table", base.Target);
					print("Sewing table actor found");
				
				base.SetActor("chairDining", base.Actor.Posture.Container);
					print("Chair actor found");
				base.EnterState("x", "Enter");
				
				// This defines how long a stage should play for
				SimpleStage simpleStage = new SimpleStage(GetInteractionName(), GetTimeToCompletion(), DraftProgressTest, true, true, true);
				base.Stages = new List<Stage>(new Stage[1]
				{
					simpleStage
				});
				
				base.BeginCommodityUpdates();
				base.AnimateSim("Loop");
				
            	bool GainSkillOnLoop = DoLoop(ExitReason.Default, Loop, null);
            	if(Actor.HasExitReason(ExitReason.UserCanceled) || (Actor.HasExitReason(ExitReason.MoodFailure)))
            	{
            		base.AnimateSim("Exit");
            	}
            	base.EndCommodityUpdates(GainSkillOnLoop);
            	//sewingSkill.StopSkillGain();
            	base.StandardExit();
            	if (mFabric != null)
				{
           			mFabric.UnParent();
					mFabric.Destroy();
					mFabric = null;
				}
            	return GainSkillOnLoop;
            }
            
            public float DraftProgressTest(InteractionInstance instance)
			{
				return Progress;
			}
            
            public void Loop(StateMachineClient smc, LoopData loopData)
            {
            	Progress += loopData.mDeltaTime / kBaseMinTimeMakeSewable;
            	
            	if(IsComplete)
            	{
					EventTracker.SendEvent(EventTypeId.kFinishedPainting, base.Actor, mFabric);
					PaintingSkill skill = base.Actor.SkillManager.GetSkill<PaintingSkill>(SkillNames.Painting);
					mObjectChosenGeneral.SetOpacity(1f, 0.3f);
					
					if(!Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(mObjectChosenGeneral))
					{
						mObjectChosenGeneral.Destroy();
						mObjectChosenGeneral = null;
					}
					Actor.ShowTNSIfSelectable(mObjectChosenGeneral.GetLocalizedName().ToString() + "  Has been moved to my family's inventory!", StyledNotification.NotificationStyle.kSimTalking);
					base.Actor.AddExitReason(ExitReason.Finished);
					base.AnimateSim("Exit");
            	}
            	EventTracker.SendEvent(EventTypeId.kPainted, base.Actor, mFabric);
            }
            
           public override void Cleanup()
           {
	           	if (base.StandardEntryCalled && mFabric != null)
	           	{
	           		if (mFabric != null)
					{
	           			mFabric.UnParent();
						mFabric.Destroy();
						mFabric = null;
					}
	           	}
	           	base.Cleanup();
           }
        }
		
		public class Practise : Interaction<Sim, SewingTable>
		{
			public class Definition :  InteractionDefinition<Sim, SewingTable, Practise>
			{
				public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
                	// The code below will check if there's a chair attached to any containment slot. 
                	if (target.GetTotalNumChairsAtTable() < 1)
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Has No Chair");
						return false;
					}
                	if (target.GetNumAvailableChairSlots(actor) == 0 && target.IsActorSittingAtTable(actor))
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Is in Use!");
						return false;
					}
	            	return true;
				}
	
				public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair iop)
				{
					return "Practise sewing";
				}
			}
	
			public static InteractionDefinition Singleton = new Definition();
	
			public override bool Run()
			{
				if (!base.Target.ScootInActor(base.Actor))
				{
					base.Actor.PlayRouteFailThoughtBalloon(base.Target.GetThumbnailKey());
					base.Actor.AddExitReason(ExitReason.FailedToStart);
					return false;
				}
            	SewingSkill sewingSkill = base.Actor.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
            	if (sewingSkill == null)
				{
            		print("Failed to load skill");
					return false;
				}
				sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
            	
				print("Did find skill" + SewingSkill.kSewingSkillGainRate.ToString());
				
				//SimpleMessageDialog.Show("Debugger Sewing", "Did find skill" + SewingSkill.kSewingSkillGainRate.ToString());
				
				base.StandardEntry();
				
				mFabric = null;
				Vector3 slotPosition = base.Target.GetSlotPosition(Slot.ContainmentSlot_1);
				Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(Slot.ContainmentSlot_1);
				mFabric = (SewingTable_ClothProp)GlobalFunctions.CreateObject(ResourceKey.FromString("0x319E4F1D:0x00000000:0x0361BDCA336A8546"), slotPosition, 0, forwardOfSlot, null, null);
				
				base.EnterStateMachine("SewingTable", "Enter", "x");
				
				if(mFabric != null)
				{
					mFabric.ParentToSlot(base.Target, Slot.ContainmentSlot_1);
					base.SetActor("fabric", mFabric);
				}
				
				base.SetActor("sewingtable_table", base.Target);
				base.SetActor("chairDining", base.Actor.Posture.Container);
				
				base.EnterState("x", "Enter");
				base.AnimateSim("Loop");
				
				bool GainSkillOnLoop = DoLoop(ExitReason.Default, LoopInfinite, null);
				
				base.AnimateSim("Exit");
				sewingSkill.StopSkillGain();
				base.StandardExit();
				return GainSkillOnLoop;
			}
			
			public void LoopInfinite(StateMachineClient smc, LoopData loopData)
            {
				if(Actor.HasExitReason(ExitReason.UserCanceled) || (Actor.HasExitReason(ExitReason.MoodFailure)))
				{
				   	base.Actor.AddExitReason(ExitReason.UserCanceled);
				   	base.Actor.AddExitReason(ExitReason.MoodFailure);
				}
            }
		}	
			
		public class Workbench_OpenInventory : ImmediateInteraction<Sim, SewingTable>
		{
			[DoesntRequireTuning]
			public class Definition : ActorlessInteractionDefinition<Sim, SewingTable, Workbench_OpenInventory>
			{
				public override bool Test(Sim a, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
					if (hudModel.SecondaryInventory != null && hudModel.SecondaryInventory == target.InventoryComp.InventoryUIModel)
					{
						return false;
					}
					return true;
				}
	
				public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair iop)
				{
					return "Open workbench";
				}
			}
	
			public static InteractionDefinition Singleton = new Definition();
	
			public override bool Run()
			{
				HudModel.OpenObjectInventoryForOwner(base.Target);
				return true;
			}
		}
		
		public class Restock : ImmediateInteraction<Sim, SewingTable>
		{
			[DoesntRequireTuning]
			public class Definition : ImmediateInteractionDefinition<Sim, SewingTable, Restock>
			{
				public int NumFabric = 1;

				public Definition()
				{
				}

				public Definition(int numFabric)
				{
					NumFabric = numFabric;
				}

				public override string[] GetPath(bool isFemale)
				{
					return new string[1]
					{
						"Restock fabric"
					};
				}

				public override void AddInteractions(InteractionObjectPair iop, Sim actor, SewingTable target, List<InteractionObjectPair> results)
				{
					for (int i = 0; i < kRestockNumFabric.Length; i++)
					{
						results.Add(new InteractionObjectPair(new Definition(kRestockNumFabric[i]), iop.Target));
					}
				}

				public override string GetInteractionName(Sim a, SewingTable target, InteractionObjectPair interaction)
				{
					return "Restock: " + NumFabric.ToString() + " " + GetFabricCost(NumFabric).ToString();
				}

				public override bool Test(Sim a, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					if (GetFabricCost(NumFabric) > a.FamilyFunds)
					{
						greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("NotEnoughMoney"));
						return false;
					}
					return true;
				}
			}

			public static InteractionDefinition Singleton = new Definition();

			public override bool Run()
			{
				Definition definition = base.InteractionDefinition as Definition;
				if (IngredientFabric.CreateAndAddToInventory(base.Target, definition.NumFabric))
				{
					base.Actor.ModifyFunds(-GetFabricCost(definition.NumFabric));
				}
				return true;
			}

			public static int GetFabricCost(int numUnits)
			{
				return (int)Math.Floor((double)((float)numUnits * (float)kFabricUnitValue * 5f));
			}
		}		

		public override void OnCreation()
		{
			base.OnCreation();
		}

		public override void OnLoad()
		{
			mHasInitialFabric = true;
			base.OnLoad();
		}

		public override void OnStartup()
		{
			base.AddComponent<InventoryComponent>(new object[0]);
			if (!mHasInitialFabric && !Sims3.SimIFace.Environment.HasEditInGameModeSwitch)
			{
				IngredientFabric.CreateAndAddToInventory(this, 50);
				mHasInitialFabric = true;
			}
			base.AddInteraction(SewingObjects.Singleton);
			base.AddInteraction(Practise.Singleton);
			base.AddInteraction(Workbench_OpenInventory.Singleton);
			base.AddInteraction(Restock.Singleton);
			Inventory inventory = base.Inventory;
			inventory.EventCallback = (InventoryEventCallback)Delegate.Combine(inventory.EventCallback, new InventoryEventCallback(InventoryEventCallback));
			GetAllSimulationObjectKeysForDialogue();
			print(EasySewablesList.Count.ToString());
		}

		//boolean for checking whether the dialogue has been populated before to prevent duplicates.
		public static bool DidIRunBefore = false;

		public static bool ShowSewableItemsDialog()
		{	
			print("Inside showdialogueitem");
			List<ObjectPicker.HeaderInfo> 	header = new List<ObjectPicker.HeaderInfo>();
			List<ObjectPicker.TabInfo> 		tab = new List<ObjectPicker.TabInfo>();

			// This is always the same. you can only select one project at a time.
			int numSelectableRows = 1;
			header.Add(new ObjectPicker.HeaderInfo("Header test", "HeaderTestTooltip", 250));
			ObjectPicker.TabInfo tabinfo = new ObjectPicker.TabInfo(string.Empty, LocalizeString("TabText"), new List<ObjectPicker.RowInfo>());
			HudModel hudModel = new HudModel();

			// Check so that we don't have duplicates in the list.
//			if(!DidIRunBefore)
//			{
				// See code above on how we're getting the allGameSewables populated
//				foreach(ResourceKey resKeyinGame in EasySewablesList)
//				{
//					GameObject mCreatedObject = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(resKeyinGame, null, null);
//					ThumbnailKey thumbnail = new ThumbnailKey(resKeyinGame, ThumbnailSize.Medium);
//					
//					ObjectPicker.RowInfo rowInfoAllGames = new ObjectPicker.RowInfo(resKeyinGame, new List<ObjectPicker.ColumnInfo>());
//					rowInfoAllGames.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(
//						mCreatedObject.GetThumbnailKey(),
//						mCreatedObject.GetLocalizedName()
//						)
//					);
//					// Add it to the dialogue's entry options.
//					tabinfo.RowInfo.Add(rowInfoAllGames);
//				}
//				
//				// See GetAllSimulationObjectKeysLyralei() on how we're getting the CustomGameObjectsList populated
//				foreach(ResourceKey resKey in CustomGameObjectsList)
//				{
//					GameObject mCreatedObject = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(resKey, null, null);
//					ThumbnailKey thumbnail = new ThumbnailKey(resKey, ThumbnailSize.Medium);
//					
//					ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(resKey, new List<ObjectPicker.ColumnInfo>());
//					rowInfo.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(
//						thumbnail,
//						mCreatedObject.GetLocalizedName()
//						)
//					);
//					// Add it to the dialogue's entry options.
//					tabinfo.RowInfo.Add(rowInfo);
//				}
				// If sim is a supernatural kind with magic, then list the magical bear:
//				if(mSimDescription.IsWitch || (mSimDescription.IsFairy) || (mSimDescription.IsGenie) || (mSimDescription.IsImaginaryFriend))
//				{
//					if(Sims3.SimIFace.GameUtils.IsInstalled(ProductVersion.EP2))
//					{
//						GameObject mCreatedObject2 = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(ResourceKey.FromString("0x319E4F1D-0x18000000-0x0000000000989EDF"), null, null);
//						ObjectPicker.RowInfo rowInfo2 = new ObjectPicker.RowInfo(ResourceKey.FromString("0x319E4F1D-0x18000000-0x0000000000989EDF"), new List<ObjectPicker.ColumnInfo>());
//						rowInfo2.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(
//							mCreatedObject2.GetThumbnailKey(),
//							mCreatedObject2.GetLocalizedName()
//							)
//						);
//						tabinfo.RowInfo.Add(rowInfo2);
//					}
//				}
//				MergedObjects.AddRange(EasySewablesList);
//				MergedObjects.AddRange(CustomGameObjectsList);
//			}
//			if(DidIRunBefore)
//			{
//				// Make a list from the cached 
//				foreach(ResourceKey resKeyCache in getAllCachedDialogueObjects)
//				{
//					GameObject mCreatedObject = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(resKeyCache, null, null);
//					ThumbnailKey thumbnail = new ThumbnailKey(resKeyCache, ThumbnailSize.Medium);
//					
//					ObjectPicker.RowInfo rowInfoCache = new ObjectPicker.RowInfo(resKeyCache, new List<ObjectPicker.ColumnInfo>());
//					rowInfoCache.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(
//						thumbnail,
//						mCreatedObject.GetLocalizedName()
//						)
//					);
//					// Add it to the dialogue's entry options.
//					tabinfo.RowInfo.Add(rowInfoCache);
//				}	
//			}
//			DidIRunBefore = true;
			tab.Add(tabinfo);
			try {
				// Return the Object the user selected. 
				List<ObjectPicker.RowInfo> rowInfo1 = ObjectPickerDialog.Show(
					true,
					ModalDialog.PauseMode.PauseSimulator, 
					"Choose a project to work on", 
					Localization.LocalizeString("Ui/Caption/Global:Ok"), 
					Localization.LocalizeString("Ui/Caption/Global:Cancel"),  
					tab, 
					header, 
					numSelectableRows
				);
				
				// Makes sure that the chosen object will be instantiated and placed after the sim is done making it			
				//prepareChosenObject(rowInfo1[0].Item.ToString());
				return true;
			}
			catch(System.NullReferenceException)
			{
				print("Lyralei's sewing table: The modal wasn't able to find the custom object! If this keeps occuring, make sure to let Lyralei know! Sewing table OUT.");
				return false;
			}
		}
		public static GameObject mObjectChosenGeneral;

		public static GameObject mPlaceholderSewable;

       // Makes sure that the chosen object will be instantiated and placed after the sim is done making it
       public static bool prepareChosenObject(String resKeyString)
       {
       		//print(resKeyString);
       		GameObject mObjectChosen = (GameObject)GlobalFunctions.CreateObject(ResourceKey.FromString(resKeyString), target2.GetSlotPosition(Slot.ContainmentSlot_1), 0, target2.GetForwardOfSlot(Slot.ContainmentSlot_1), null, null);
       		mObjectChosen.SetOpacity(0f, 0f);
       		mObjectChosenGeneral = mObjectChosen;

       		if(mObjectChosen != null)
       		{
       			print("chosen object isn't empty!");
       			return true;
       		}
       		print("chosen object is empty!");
       		return false;
       }
	
       
       // Convert our In-GAME object strings to proper ResourceKeys
       public static List<ResourceKey> ConvertListStringsToResKeys(List<string> objectsToConvert)
       {
   			/*
			 * Easy sewables GroupID:   		0x1001A575
			 * Medium Sewables GroupID: 		0x74031C5A
			 * Hard Sewables GroupID:   		0x86C8D040
			 * Hardest Sewables GroupID: 		0x74322790
			 * MagicHardestSewables GroupID: 	0x21BC2F49				
			 */
       		foreach(string converts in objectsToConvert)
       		{
   				ResourceKey convertedstring = ResourceKey.FromString(converts);
   				allGameSewables.Add(convertedstring);
   				//return allGameSewables;
       		}
       		return allGameSewables;
       }
		public static void GetAllSimulationObjectKeysForDialogue()
		{
			ulong[] array 		 	= ScriptCore.Simulator.Simulator_GetAllSimulationObjectKeysImpl();
			int num				 	= array.Length;
			ResourceKey[] array2	= new ResourceKey[num];
			ResourceKey sewableKey 	= new ResourceKey();
			
			
			//Find in-game objects.
			for (int i = 0; i < easySewables.Count; i++)
			{
				//Check for easy
				if (Sims3.SimIFace.Simulator.CountResources(easySewables[i]) > 0)
				{
					EasySewablesList.Add(sewableKey);
				}
			}
			
//			for (int i = 0; i < num; i++)
//			{
				/* CUSTOM GROUPS:
				 * Easy sewables GroupID:   		0x1001A575
				 * Medium Sewables GroupID: 		0x74031C5A
				 * Hard Sewables GroupID:   		0x86C8D040
				 * Hardest Sewables GroupID: 		0x74322790
				 * MagicHardestSewables GroupID: 	0x21BC2F49				
				*/
//				for (int j = 0; j < customSewableGroups.Count; j++)
//				{
//					sewableKey = new ResourceKey(array[i], 0x319E4F1D, customSewableGroups[j]);
//					if (Sims3.SimIFace.Simulator.CountResources(sewableKey) > 0)
//					{
//						if (j == 0) EasySewablesList.Add(sewableKey);
//						if (j == 1) MediumSewablesList.Add(sewableKey);
//						if (j == 2) HardSewablesList.Add(sewableKey);
//						if (j == 3) HarderSewablesList.Add(sewableKey);
//						if (j == 4) MagicHarderSewablesList.Add(sewableKey); 
//						break;
//					}
//				}
//			}
		}
		
		//Placeholder from now on.
		public static ResourceKey fabricIngredientTESTRK = new ResourceKey(0xC480800032188243, 0x319E4F1D, 0x00000000);
		
		public void AddPlaceholderSewable()
		{
			mObjectChosenGeneral.Destroy();
			mObjectChosenGeneral = null;
			mPlaceholderSewable = (GlobalFunctions.CreateObject(fabricIngredientTESTRK, GetSlotPosition(Slot.ContainmentSlot_1), 0, target2.GetForwardOfSlot(Slot.ContainmentSlot_1), null, null) as IngredientFabric);
			if (mPlaceholderSewable != null)
			{
				mPlaceholderSewable.SetOpacity(1f, 0f);
				mPlaceholderSewable.ParentToSlot(target2, Slot.ContainmentSlot_1);
				mPlaceholderSewable.FadeIn(true);
			}
		}
		
		public void RemovePlaceholderSewable()
		{
			if (mPlaceholderSewable != null)
			{
				mPlaceholderSewable.UnParent();
				mPlaceholderSewable.FadeOut(true);
				mPlaceholderSewable.Destroy();
				mPlaceholderSewable = null;
			}
			print("Nothing to remove!");
		}		
		
		public static float GetTimeToCompletion()
		{
			PaintingSkill skill = ActorCurr.SkillManager.GetSkill<PaintingSkill>(SkillNames.Painting);
//			float num = MathHelpers.LinearInterpolate((float)0, (float)skill.MaxSkillLevel, (float)kBaseMinTimeMakeSewable, (float)kBaseMaxTimeMakeSewable, (float)skill.SkillLevel);
//			if (num <= 0 || (num <= 0f ))
//			{
//				num = (float)kBaseMinTimeMakeSewable;
//			}
			return 30f; //num;
		}

		public static void print(string text)
		{
			StyledNotification.Show(new StyledNotification.Format(text, StyledNotification.NotificationStyle.kDebugAlert));
		}
		
		public void InventoryEventCallback(uint stackNumber, InventoryEvent inventoryEvent, IGameObject obj)
		{
			if (inventoryEvent != InventoryEvent.kStackAddedTo && inventoryEvent != InventoryEvent.kStackRemovedFrom && inventoryEvent != InventoryEvent.kStackCreated && inventoryEvent != InventoryEvent.kStackDeleted)
			{
				return;
			}
		}
		
		public static Slot[] sContainmentSlotChair = new Slot[4]
		{
			Slot.ContainmentSlot_0,
			Slot.ContainmentSlot_1,
			Slot.ContainmentSlot_2,
			Slot.ContainmentSlot_3
		};
		
		public override bool IsObjectInFrontOfMe(IGameObject gameObject)
		{
			ChairDining chairDining = gameObject as ChairDining;
			if (chairDining == null)
			{
				return false;
			}
			SewingTable sewingTable = chairDining.Parent as SewingTable;
			return sewingTable == this;
		}
		
		public int GetTotalNumChairsAtTable()
		{
			int num = 0;
			Slot[] containmentSlots = base.GetContainmentSlots();
			Slot[] array = containmentSlots;
			foreach (Slot slotName in array)
			{
				IGameObject containedObject = base.GetContainedObject(slotName);
				if (containedObject != null)
				{
					num++;
				}
			}
			return num;
		}
			
		public bool IsActorSittingAtTable(Sim a)
		{
			Slot[] containmentSlots = base.GetContainmentSlots();
			Slot[] array = containmentSlots;
			foreach (Slot slotName in array)
			{
				IGameObject containedObject = base.GetContainedObject(slotName);
				if (containedObject != null && containedObject.IsActorUsingMe(a))
				{
					return true;
				}
			}
			return false;
		}		
		
		public int GetNumAvailableChairSlots(Sim a)
		{
			int num = 0;
			Slot[] containmentSlots = base.GetContainmentSlots();
			Slot[] array = containmentSlots;
			foreach (Slot slotName in array)
			{
				IGameObject containedObject = base.GetContainedObject(slotName);
				if (containedObject != null && (!containedObject.InUse || containedObject.IsActorUsingMe(a)))
				{
					num++;
				}
			}
			return num;
		}
		
		// Kindly stolen from Domino chair
		public bool ScootInActor(Sim actor)
		{
			if (actor != null && actor.Posture != null)
			{
				ChairDining chairDining = actor.Posture.Container as ChairDining;
				if (chairDining != null && chairDining.Parent != null && actor.CurrentInteraction != null)
				{
					if (chairDining.ChairState == ChairDining.State.Angled && base.Parent != actor)
					{
						InteractionInstance interactionInstance = SitTransitionAngledToStraight.Singleton.CreateInstance(actor.Posture.Container, actor, actor.CurrentInteraction.GetPriority(), false, false);
						return interactionInstance.RunInteraction();
					}
					if (chairDining.ChairState == ChairDining.State.Straight)
					{
						return true;
					}
				}
			}
			return false;
		}	
	}
}