using System;
using System.Collections.Generic;
using Lyralei;
using sims3.UI.Lyralei;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Lyralei;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;

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
		public static List<ResourceKey> MergedPatterns    = new List<ResourceKey>();
		      
		public bool mHasInitialFabric = false;
		
		public bool mHasInitialPatterns = false;
		
		public static int mCurrentSkillLevel = 0;
		
		public static List<GameObject> mCreatedObjects = new List<GameObject>();

        public float Progress;

        public float ProgressClothes;

        public static SewingTable target2;
       	
       	public static SimDescription mSimDescription;
       	
       	public static Sim ActorCurr;
       	
        public static SewingTable_ClothProp mFabric;
        
        public static GameObject mObjectChosenGeneral;

        public static Pattern mClothingChosenGeneral;

        public static GameObject mStoredObject;

        public static Pattern mStoredClothing;

        public static GameObject mPlaceholderSewable;
		
		[Tunable]
		public static float SuccessChance = 10f;
		
		[Tunable]
		public static float SuccessIncreasePerLevel = 5f;
		
		public static List<IngredientFabric> mFabricList = new List<IngredientFabric>();
       	
       	[TunableComment("Min time it will take to make the min level sewable")]
       	[Tunable]
		public static int kBaseMinTimeMakeSewable = 300;

		[TunableComment("Max time it will take to make the max level sewable")]
		[Tunable]
		public static int kBaseMaxTimeMakeSewable = 3000;
		
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
		
		[TunableComment("The chance a sim has to discover new patterns")]
		[Tunable]
		public static float kChanceToDiscoverPatterns = 100;
		
        public bool IsComplete
		{
			get
			{
				return Progress >= 1f;
			}
		}

        public bool IsCompleteClothing
        {
            get
            {
                return ProgressClothes >= 1f;
            }
        }

        public class SewingObjects : Interaction<Sim, SewingTable>
        {
            public class Definition :  InteractionDefinition<Sim, SewingTable, SewingObjects>
            {
            	public bool IsContinuation = false;
            	
            	public Definition()
				{
				}
            	
	 			public Definition(bool isContinuation)
				{
					IsContinuation = isContinuation;
				}
	 			
                public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair interaction)
                {
	                if (target.Progress > 0f && !target.IsActorUsingMe(actor as Sim))
					{
	                	IsContinuation = true;
	                	return Localization.LocalizeString("Lyralei/Localized/ContinueProject:InteractionName", new object[0]);
		                //return "Continue Project"; //Add Percentage here later. See ChemistryLab
	                }
                    if (mStoredObject == null)
                    {
                        return Localization.LocalizeString("Lyralei/Localized/SewObject:InteractionName", new object[0]);
                    }
                    return Localization.LocalizeString("Lyralei/Localized/SewObject:InteractionName", new object[0]);
                   // return "Sew an Object";
                }
                
                public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                	SewingSkill sewingSkill = actor.SkillManager.GetElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
                	int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
                	
               		if(skillLevel < 1)
                	{
	                	if(sewingSkill != null)
	                	{
	                		if(target.Progress > 0f && !target.IsActorUsingMe(actor as Sim))
							{
	                			IsContinuation = true;
	                			if(skillLevel < mCurrentSkillLevel)
	                			{
	                				greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/DontKnowHow:Test", new object[0]));
	                				return false;
	                			}
	                		}
	                	}
	                	return false;
                	}
                    if (skillLevel > 1 && target.Progress > 0f)
                    {
                        return true;
                    }
                    // The code below will check if there's a chair attached to any containment slot.
                    if (target.GetTotalNumChairsAtTable() < 1)
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/NoChair:Test", new object[0]));
						return false;
					}
                	if(target.IsActorUsingMe(actor as Sim))
 					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/InUse:Test", new object[0]));
						return false;
					}
	            	return true;
                }
            }
            
            public static InteractionDefinition Singleton = new Definition();
            
            public ScriptCore.GameUtils gUtils;
            
            //ADD PROP FOR BRACELET WITH PINS
            public override bool Run()
            {
            	Definition definition = base.InteractionDefinition as Definition;
            	base.Target.RemovePlaceholderSewable();
				if (!base.Target.ScootInActor(base.Actor))
				{
					base.Actor.PlayRouteFailThoughtBalloon(base.Target.GetThumbnailKey());
					base.Actor.AddExitReason(ExitReason.FailedToStart);
					return false;
				}
				target2 		= base.Target;
				mSimDescription = base.Actor.mSimDescription;
				ActorCurr		= base.Actor;
				
            	SewingSkill sewingSkill = base.Actor.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
            	if (sewingSkill == null)
				{
            		print("Lyralei's sewing table: Sewing skill doens't exist!");
					return false;
				}
				
            	if(!definition.IsContinuation)
            	{
            		if(!ShowSewableItemsDialog())
					{
						base.Actor.AddExitReason(ExitReason.FailedToStart);
						return false;
					}
            	}
            	
				sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
				base.StandardEntry();

                GlobalOptionsSewingTable.TriggerLesson((Lessons)120, base.Actor);

                mFabric = null;
				Vector3 slotPosition = base.Target.GetSlotPosition(Slot.ContainmentSlot_1);
				Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(Slot.ContainmentSlot_1);
				mFabric = (SewingTable_ClothProp)GlobalFunctions.CreateObject(ResourceKey.FromString("0x319E4F1D:0x00000000:0x0361BDCA336A8546"), slotPosition, 0, forwardOfSlot, null, null);
				base.EnterStateMachine("SewingTable", "Enter", "x");
				
				if(mFabric != null)
				{
					//Pattern.SetPatternMaterial(mFabric, 0, base.Actor);
					mFabric.ParentToSlot(base.Target, Slot.ContainmentSlot_1);
					base.SetActor("fabric", mFabric);
				}
				
				//Sewing table exists here
				base.SetActor("sewingtable_table", base.Target);
				base.SetActor("chairDining", base.Actor.Posture.Container);
				base.EnterState("x", "Enter");
				
				// This defines how long a stage should play for
				SimpleStage simpleStage = new SimpleStage(GetInteractionName(), GetTimeToCompletion(), DraftProgressTest, true, true, true);
				base.Stages = new List<Stage>(new Stage[1]
				{
					simpleStage
				});
				
				sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
				base.BeginCommodityUpdates();
				base.AnimateSim("Loop");
				
            	bool GainSkillOnLoop = DoLoop(ExitReason.Default, Loop, null);
            	base.EndCommodityUpdates(GainSkillOnLoop);
            	sewingSkill.StopSkillGain();
            	if (base.Actor.HasExitReason(ExitReason.Finished) && GainSkillOnLoop)
				{
            		int skillLevel = base.Actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
            		if(!RandomUtil.RandomChance(SuccessChance + SuccessIncreasePerLevel * (float)skillLevel))
					{
            			base.AnimateSim("ExitFail");
						Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/SimDialogue:FailedToSew", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
						mObjectChosenGeneral.Destroy();
						mObjectChosenGeneral = null;
	            	}
            		else
            		{
            			mObjectChosenGeneral.SetOpacity(1f, 0.3f);
            			Actor.ShowTNSIfSelectable(mObjectChosenGeneral.GetLocalizedName().ToString() + Localization.LocalizeString("Lyralei/Localized/SimDialogue:AddedToInventory", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
                        sewingSkill.AddFinishedProjectsCount(1);
                        if (!Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(mObjectChosenGeneral))
						{
							mObjectChosenGeneral.Destroy();
							mObjectChosenGeneral = null;
						}
						base.AnimateSim("Exit");
            		}
            		definition.IsContinuation = false;
                    base.Target.Progress = 0f;
				}
            	if(Actor.HasExitReason(ExitReason.UserCanceled) || Actor.HasExitReason(ExitReason.MoodFailure) || Actor.HasExitReason(ExitReason.Canceled))
            	{
            		if (base.Target.Progress > 0f)
					{
            			mStoredObject = mObjectChosenGeneral;
						base.Target.AddPlaceholderSewable();
            		}
            		base.AnimateSim("Exit");
                    base.StandardExit(false);

                }
            	base.StandardExit(GainSkillOnLoop);
            	return GainSkillOnLoop;
            }
            
            public float DraftProgressTest(InteractionInstance instance)
			{
				return base.Target.Progress;
			}
            
            public void Loop(StateMachineClient smc, LoopData loopData)
            {
            	//Add failure state here
            	Definition definition = base.InteractionDefinition as Definition;
                base.Target.Progress += loopData.mDeltaTime / kBaseMinTimeMakeSewable;
            	
            	if(base.Target.IsComplete)
            	{
            		if(!definition.IsContinuation)
	            	{
            			base.Actor.AddExitReason(ExitReason.Finished);
            		}
	            	if(definition.IsContinuation)
	            	{
	            		if(mStoredObject != null)
		            	{
	            			mObjectChosenGeneral = mStoredObject;
	            		}
	            		base.Actor.AddExitReason(ExitReason.Finished);
	            	}
	            	base.Actor.AddExitReason(ExitReason.Finished);
            	}
            }
            
            public override void Cleanup()
            {
	           	if (mFabric != null)
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

        public class SewingClothes : Interaction<Sim, SewingTable>
        {
            public class Definition : InteractionDefinition<Sim, SewingTable, SewingClothes>
            {
                public bool IsContinuationClothing = false;

                public Definition()
                {
                }

                public Definition(bool isContinuationClothing)
                {
                    IsContinuationClothing = isContinuationClothing;
                }

                public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair interaction)
                {
                    if (target.ProgressClothes > 0f && !target.IsActorUsingMe(actor as Sim))
                    {
                        IsContinuationClothing = true;
                        return Localization.LocalizeString("Lyralei/Localized/ContinueProjectClothing:InteractionName", new object[0]);
                        //return "Continue Project"; //Add Percentage here later. See ChemistryLab
                    }
                    if(mStoredClothing == null)
                    {
                        return Localization.LocalizeString("Lyralei/Localized/SewClothing:InteractionName", new object[0]);
                    }
                    return Localization.LocalizeString("Lyralei/Localized/SewClothing:InteractionName", new object[0]);
                }

                public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    SewingSkill sewingSkill = actor.SkillManager.GetElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
                    int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);

                    if (skillLevel < 6)
                    {
                        if (sewingSkill != null)
                        {
                            if (target.ProgressClothes > 0f && !target.IsActorUsingMe(actor as Sim))
                            {
                                IsContinuationClothing = true;
                                if (skillLevel < mCurrentSkillLevel)
                                {
                                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/DontKnowHow:Test", new object[0]));
                                    return false;
                                }
                            }
                        }
                        return false;
                    }
                    if(skillLevel > 1 && target.ProgressClothes > 0f)
                    {
                        return true;
                    }

                    // The code below will check if there's a chair attached to any containment slot.
                    if (target.GetTotalNumChairsAtTable() < 1)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/NoChair:Test", new object[0]));
                        return false;
                    }
                    if (target.IsActorUsingMe(actor as Sim))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/InUse:Test", new object[0]));
                        return false;
                    }
                    return true;
                }
            }

            public static InteractionDefinition Singleton = new Definition();

            public ScriptCore.GameUtils gUtils;

            //ADD PROP FOR BRACELET WITH PINS
            public override bool Run()
            {
                Definition definition = base.InteractionDefinition as Definition;
                base.Target.RemovePlaceholderSewable();
                if (!base.Target.ScootInActor(base.Actor))
                {
                    base.Actor.PlayRouteFailThoughtBalloon(base.Target.GetThumbnailKey());
                    base.Actor.AddExitReason(ExitReason.FailedToStart);
                    return false;
                }
                target2 = base.Target;
                mSimDescription = base.Actor.mSimDescription;
                ActorCurr = base.Actor;

                SewingSkill sewingSkill = base.Actor.SkillManager.AddElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
                if (sewingSkill == null)
                {
                    print("Lyralei's sewing table: Sewing skill doesn't exist!");
                    return false;
                }

                if (!definition.IsContinuationClothing)
                {
                    List<Pattern> mResKeysInInventory = new List<Pattern>();
                    foreach (Pattern obj in base.Target.Inventory.FindAll<Pattern>(false))
                    {
                        if (obj.mPatternInfo.isClothing)
                        {
                            mResKeysInInventory.Add(obj);
                        }
                    }
                    foreach (Pattern obj in base.Actor.Inventory.FindAll<Pattern>(false))
                    {
                        if (obj.IsClothing)
                        {
                            // This isn't being fired!
                            mResKeysInInventory.Add(obj);
                        }
                    }

                    mClothingChosenGeneral = CreateClothingSelector.Show(mResKeysInInventory);
                    
                    if(mClothingChosenGeneral == null)
                    {
                        print("Nothing was selected, so the interaction is canceled");
                        base.Actor.AddExitReason(ExitReason.FailedToStart);
                        return false;
                    }
                    else
                    {
                        prepareChosenClothing(mClothingChosenGeneral);
                    }
                }

                sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
                base.StandardEntry();

                GlobalOptionsSewingTable.TriggerLesson((Lessons)207, base.Actor);

                mFabric = null;
                Vector3 slotPosition = base.Target.GetSlotPosition(Slot.ContainmentSlot_1);
                Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(Slot.ContainmentSlot_1);
                mFabric = (SewingTable_ClothProp)GlobalFunctions.CreateObject(ResourceKey.FromString("0x319E4F1D:0x00000000:0x0361BDCA336A8546"), slotPosition, 0, forwardOfSlot, null, null);
                base.EnterStateMachine("SewingTable", "Enter", "x");

                if (mFabric != null)
                {
                    //Pattern.SetPatternMaterial(mFabric, 0, base.Actor);
                    mFabric.ParentToSlot(base.Target, Slot.ContainmentSlot_1);
                    base.SetActor("fabric", mFabric);
                }

                //Sewing table exists here
                base.SetActor("sewingtable_table", base.Target);
                base.SetActor("chairDining", base.Actor.Posture.Container);
                base.EnterState("x", "Enter");

                // This defines how long a stage should play for
                SimpleStage simpleStage = new SimpleStage(GetInteractionName(), GetTimeToCompletion(), DraftProgressTest, true, true, true);
                base.Stages = new List<Stage>(new Stage[1]
                {
                    simpleStage
                });

                sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
                base.BeginCommodityUpdates();
                base.AnimateSim("Loop");

                bool GainSkillOnLoop = DoLoop(ExitReason.Default, Loop, null);
                base.EndCommodityUpdates(GainSkillOnLoop);
                sewingSkill.StopSkillGain();
                if (base.Actor.HasExitReason(ExitReason.Finished) && GainSkillOnLoop)
                {
                    int skillLevel = base.Actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
                    //if (!RandomUtil.RandomChance(SuccessChance + SuccessIncreasePerLevel * (float)skillLevel))
                    //{
                    //    base.AnimateSim("ExitFail");
                    //    Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/SimDialogue:FailedToSew", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
                    //    //mClothingChosenGeneral.Destroy();
                        
                    //    mClothingChosenGeneral = null;
                    //}
                    if(mClothingChosenGeneral != null)
                    {
                        //Household household = base.Actor.Household;
                        AddOutfitToWardrobe(mClothingChosenGeneral.mPatternInfo.resKeyPattern);
                        //mObjectChosenGeneral.SetOpacity(1f, 0.3f);
                        Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/SimDialogue:AddedToInventoryClothing", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
                        
                        if(!Pattern.AddClothingToGiftable(mClothingChosenGeneral, base.Actor))
                        {
                            base.AnimateSim("Exit");
                            print("Lyralei's sewing table: Pattern didn't want to be saved as a gift! While the clothing piece may be available in the sim's wardrobe, it might not be available as a giftable item. Usually why this happens is due to that the sim has already made this item before and can't have 2 of these items! This could mean it is therefore giftable, but thats something for you to check. Else ignore this message! Either try again or report to Lyralei with the package that has the clothing piece you're using.");
                            base.Actor.AddExitReason(ExitReason.Finished);
                            definition.IsContinuationClothing = false;
                            //base.AnimateSim("Exit");
                            base.StandardExit(true);
                            mClothingChosenGeneral = null;
                            return false;
                        }
                        sewingSkill.AddFinishedProjectsCount(1);
                        //if (!Actor.Household.SharedFamilyInventory.Inventory.TryToAdd(mObjectChosenGeneral))
                        //{
                        //    mObjectChosenGeneral.Destroy();
                        //    mObjectChosenGeneral = null;
                        //}
                        base.AnimateSim("Exit");
                    }
                    definition.IsContinuationClothing = false;
                    base.Target.ProgressClothes = 0f;
                }
                if (Actor.HasExitReason(ExitReason.UserCanceled) || Actor.HasExitReason(ExitReason.MoodFailure) || Actor.HasExitReason(ExitReason.Canceled) || Actor.HasExitReason(ExitReason.CanceledByScript) || Actor.HasExitReason(ExitReason.CancelExternal))
                {
                    if (base.Target.ProgressClothes > 0f)
                    {
                        mStoredClothing = mClothingChosenGeneral;
                        base.Target.AddPlaceholderSewable();
                    }
                    base.AnimateSim("Exit");
                    base.StandardExit(true);
                }
                base.StandardExit(GainSkillOnLoop);
                return GainSkillOnLoop;
            }

            public void AddOutfitToWardrobe(ResourceKey outfitKey)
            {
                if (outfitKey != ResourceKey.kInvalidResourceKey)
                {
                    SimOutfit simOutfit = new SimOutfit(outfitKey);
                    if (simOutfit != null)
                    {
                        CASPart[] parts = simOutfit.Parts;
                        if(parts.Length <= 0)
                        {
                            if (!base.Actor.Household.mWardrobeCasParts.Contains(simOutfit.Key.InstanceId))
                            {
                                base.Actor.Household.mWardrobeCasParts.Add(simOutfit.Key.InstanceId);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < parts.Length; i++)
                            {
                                CASPart cASPart = parts[i];
                                if (!base.Actor.Household.mWardrobeCasParts.Contains(cASPart.Key.InstanceId))
                                {
                                    base.Actor.Household.mWardrobeCasParts.Add(cASPart.Key.InstanceId);
                                }
                            }
                        }
                    }
                }
            }

            public float DraftProgressTest(InteractionInstance instance)
            {
                return base.Target.ProgressClothes;
            }

            public void Loop(StateMachineClient smc, LoopData loopData)
            {
                //Add failure state here
                Definition definition = base.InteractionDefinition as Definition;
                base.Target.ProgressClothes += loopData.mDeltaTime / kBaseMinTimeMakeSewable;

                if (base.Target.IsCompleteClothing)
                {
                    if (!definition.IsContinuationClothing)
                    {
                        base.Actor.AddExitReason(ExitReason.Finished);
                    }
                    if (definition.IsContinuationClothing)
                    {
                        if (mStoredClothing != null)
                        {
                            mClothingChosenGeneral = mStoredClothing;
                        }
                        base.Actor.AddExitReason(ExitReason.Finished);
                    }
                    base.Actor.AddExitReason(ExitReason.Finished);
                }
            }

            public override void Cleanup()
            {
                if (mFabric != null)
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

        [Tunable]
        public static float kChanceDiscoverPatternSewingtable = 5f;

        public class Practise : Interaction<Sim, SewingTable>
		{
			public class Definition :  InteractionDefinition<Sim, SewingTable, Practise>
			{
				public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
                    
                    // The code below will check if there's a chair attached to any containment slot.
                    if (target.GetTotalNumChairsAtTable() < 1)
					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/NoChair:Test", new object[0]));
						return false;
					}
                	if(target.IsActorUsingMe(actor as Sim))
 					{
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/InUse:Test", new object[0]));
						return false;
					}
//                	if (target.GetNumAvailableChairSlots(actor) == 0 && target.IsActorSittingAtTable(actor))
//					{
//                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Is in Use!");
//						return false;
//					}
	            	return true;
				}

				public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair iop)
				{
					return Localization.LocalizeString("Lyralei/Localized/Practise:InteractionName", new object[0]);
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
            		print("Lyralei's Sewing Table: Failed to load sewing skill");
					return false;
				}
				sewingSkill.StartSkillGain(SewingSkill.kSewingSkillGainRate);
				
				target2 		= base.Target;
				mSimDescription = base.Actor.mSimDescription;
				ActorCurr		= base.Actor;
				
				base.StandardEntry();

                GlobalOptionsSewingTable.TriggerLesson((Lessons)207, base.Actor);

                Vector3 slotPosition = base.Target.GetSlotPosition(Slot.ContainmentSlot_1);
                Vector3 forwardOfSlot = base.Target.GetForwardOfSlot(Slot.ContainmentSlot_1);
                SewingTable_ClothProp mFabric = GlobalFunctions.CreateObject(ResourceKey.FromString("0x319E4F1D:0x00000000:0x0361BDCA336A8546"), slotPosition, 0, forwardOfSlot, null, null) as SewingTable_ClothProp;
				
				base.EnterStateMachine("SewingTable", "Enter", "x");
				
				if(mFabric != null)
				{
					//Pattern.SetPatternMaterial(mFabric, 0, base.Actor);
					mFabric.ParentToSlot(base.Target, Slot.ContainmentSlot_1);
					base.SetActor("fabric", mFabric);
				}
				
				base.SetActor("sewingtable_table", base.Target);
				base.SetActor("chairDining", base.Actor.Posture.Container);
				
				base.EnterState("x", "Enter");
				base.AnimateSim("Loop");
				bool GainSkillOnLoop = DoLoop(ExitReason.Default, LoopInfinite, null, 10f);

                base.AnimateSim("Exit");
				sewingSkill.StopSkillGain();
				base.StandardExit();
				return GainSkillOnLoop;
			}

			public void LoopInfinite(StateMachineClient smc, LoopData loopData)
            {
				if(RandomUtil.RandomChance(kChanceDiscoverPatternSewingtable))
				{
					Pattern.DiscoverPatternForGlobalObjects(base.Actor);
                    Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/BrowseWebForPatternsSuccess:InteractionName", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
                }
				
				if(Actor.HasExitReason(ExitReason.UserCanceled) || (Actor.HasExitReason(ExitReason.MoodFailure)) || (Actor.HasExitReason(ExitReason.Canceled)))
				{
				   	base.Actor.AddExitReason(ExitReason.UserCanceled);
				   	base.Actor.AddExitReason(ExitReason.MoodFailure);
				}
            }
			
			public override void Cleanup()
            {
				
	           	if (mFabric != null)
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
					return Localization.LocalizeString("Lyralei/Localized/OpenInventory:InteractionName", new object[0]);
				}
			}
	
			public static InteractionDefinition Singleton = new Definition();
	
			public override bool Run()
			{
				HudModel.OpenObjectInventoryForOwner(base.Target);
				return true;
			}
		}
		
		public class DEBUG_GetAllPatterns : Interaction<Sim, SewingTable>
		{
			public class Definition : InteractionDefinition<Sim, SewingTable, DEBUG_GetAllPatterns>
			{
				public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
	
				public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair iop)
				{
					return "DEBUG_GetAllPatterns";
				}
			}
	
			public static InteractionDefinition Singleton = new Definition();
	
			public override bool Run()
			{
				Pattern.DiscoverAllPatterns(base.Actor);
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
					return new string[] { Localization.LocalizeString(isFemale, "Lyralei/Localized/RestockFabric:Path", new object[0]) };
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
					return Localization.LocalizeString("Lyralei/Localized/Restock:InteractionName", new object[0]) + NumFabric.ToString() + ",  $:" + GetFabricCost(NumFabric).ToString();
				}

				public override bool Test(Sim a, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					if (GetFabricCost(NumFabric) > a.FamilyFunds)
					{
						greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Lyralei/Localized/NotEnoughMoney:Test", new object[0]));
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
			mHasInitialPatterns = true;
			base.OnLoad();
		}
		
		public static int DiscoverCount = 0;
		
		public override void OnStartup()
		{
			//MergedPatterns.AddRange(ObjectLoader.EasySewablesList);
			//MergedPatterns.AddRange(ObjectLoader.MediumSewablesList);
			//MergedPatterns.AddRange(ObjectLoader.HardSewablesList);
			//MergedPatterns.AddRange(ObjectLoader.HarderSewablesList);
			//MergedPatterns.AddRange(ObjectLoader.MagicHarderSewablesList);
			
			base.AddComponent<InventoryComponent>(new object[0]);
			if (!mHasInitialFabric && !Sims3.SimIFace.Environment.HasEditInGameModeSwitch)
			{
				IngredientFabric.CreateAndAddToInventory(this, 50);
				mHasInitialFabric = true;
			}
			
			// Only discover this amount of objects. make this a tuning. 
			if(GlobalOptionsSewingTable.retrieveData.mDiscoveredObjects.Count < 3 && !mHasInitialPatterns)
			{
				// Calculate the chance on getting this discovered pattern.
				for(int j = 0; j < 3; j++)
				{
					Pattern randomPattern = Pattern.DiscoverPatternStartUp();
					if(randomPattern != null)
					{
						this.Inventory.TryToAdd(randomPattern);
						mHasInitialPatterns = true;
					}
				}
			}
			base.AddInteraction(SewingObjects.Singleton);
            base.AddInteraction(SewingClothes.Singleton);
            base.AddInteraction(Practise.Singleton);
			base.AddInteraction(Workbench_OpenInventory.Singleton);
			base.AddInteraction(Restock.Singleton);
            base.AddInteraction(DEBUG_ThumbnailCameraCenterView.Singleton);
            Inventory inventory = base.Inventory;
			inventory.EventCallback = (InventoryEventCallback)Delegate.Combine(inventory.EventCallback, new InventoryEventCallback(InventoryEventCallback));
		}
		
		public override void AddDebugInteractions(List<InteractionDefinition> debugInteractions)
		{
			debugInteractions.Add(DEBUG_GetAllPatterns.Singleton);
			base.AddDebugInteractions(debugInteractions);
		}
		public static bool ShowSewableItemsDialog()
		{
			List<ObjectPicker.HeaderInfo> 	header = new List<ObjectPicker.HeaderInfo>();
			List<ObjectPicker.TabInfo> 		tab = new List<ObjectPicker.TabInfo>();

			// This is always the same. you can only select one project at a time.
			int numSelectableRows = 1;
			header.Add(new ObjectPicker.HeaderInfo("Header test", "HeaderTestTooltip", 250));
			ObjectPicker.TabInfo tabinfo = new ObjectPicker.TabInfo(string.Empty, LocalizeString("TabText"), new List<ObjectPicker.RowInfo>());
			HudModel hudModel = new HudModel();

			int skillLevel = ActorCurr.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
			int counter = 0;
			List<Pattern> mResKeysInInventory = new List<Pattern>();
			foreach(Pattern obj in target2.Inventory.FindAll<Pattern>(false))
			{
				mResKeysInInventory.Add(obj);
			}
			foreach(Pattern obj in ActorCurr.Inventory.FindAll<Pattern>(false))
			{
				mResKeysInInventory.Add(obj);
			}
			foreach(Pattern resKeyInGame in mResKeysInInventory)
			{
				if(skillLevel >= resKeyInGame.SkillLevel)
				{
					counter++;
					GameObject mCreatedObject;
					if(counter > mCreatedObjects.Count)
					{
						mCreatedObject = (GameObject)GlobalFunctions.CreateObjectOutOfWorld(resKeyInGame.ResourceKeyPattern, null, null);
						mCreatedObjects.Add(mCreatedObject);
					}
					else 
					{
                        mCreatedObject = mCreatedObjects[counter - 1];
					}
					ThumbnailKey thumbnail = new ThumbnailKey(resKeyInGame.ResourceKeyPattern, ThumbnailSize.Medium);
					ObjectPicker.RowInfo rowInfoAllGames = new ObjectPicker.RowInfo(resKeyInGame.ResourceKeyPattern, new List<ObjectPicker.ColumnInfo>());
					rowInfoAllGames.ColumnInfo.Add(new ObjectPicker.ThumbAndTextColumn(
						thumbnail,
						mCreatedObject.GetLocalizedName()
						)
					);
					// Add it to the dialogue's entry options.
					tabinfo.RowInfo.Add(rowInfoAllGames);
				}
			}
			tab.Add(tabinfo);
			try 
			{
				// Return the Object the user selected.
				List<ObjectPicker.RowInfo> rowInfo1 = ObjectPickerDialog.Show(
					true,
					ModalDialog.PauseMode.PauseSimulator,
					Localization.LocalizeString("Lyralei/Localized/HeaderTitle:Modal", new object[0]),
					Localization.LocalizeString("Ui/Caption/Global:Ok"),
					Localization.LocalizeString("Ui/Caption/Global:Cancel"),
					tab,
					header,
					numSelectableRows
				);
				// return chosen object and instantiate it.
				if(!prepareChosenObject(rowInfo1[0].Item.ToString()))
				{
					print("Row: " + rowInfo1[0].Item.ToString());
					print("Couldn't prepare");
					return false;
				}
				return true;
			}
			catch(System.NullReferenceException)
			{
				//print("Lyralei's sewing table: The modal wasn't able to find the custom object! If this keeps occuring, make sure to let Lyralei know! Sewing table OUT.");
				return false;
			}
		}

        //        public static bool ShowClothingDialogue()
        //        {
        //            return true;
        //        }
        //		
        public static bool prepareChosenClothing(Pattern ClothingPattern)
        {
            ResourceKey sewableKey = ClothingPattern.mPatternInfo.resKeyPattern;
            if(ClothingPattern == null)
            {
                print("Clothing was null");
                return false;
            }
            for (int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
            {
                if (ObjectLoader.sewableSettings[i].key == sewableKey)
                {
                    foreach (SewingSkill.FabricType fabrics in ObjectLoader.sewableSettings[i].typeFabric)
                    {
                        //Knitted, Cotton, Satin, Leather, Denim, Synthetic
                        if (fabrics == SewingSkill.FabricType.Knitted)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Knitted, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                        if (fabrics == SewingSkill.FabricType.Cotton)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Cotton, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                        if (fabrics == SewingSkill.FabricType.Satin)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Satin, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                        if (fabrics == SewingSkill.FabricType.Leather)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Leather, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                        if (fabrics == SewingSkill.FabricType.Denim)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Denim, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                        if (fabrics == SewingSkill.FabricType.Synthetic)
                        {
                            if (!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Synthetic, ObjectLoader.sewableSettings[i].amountRemoveFabric))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            mClothingChosenGeneral = ClothingPattern;

            if (ClothingPattern != null)
            {
                return true;
            }
            return false;
        }

        // Create chosen object, instantiate and place it after the sim is done making it
        public static bool prepareChosenObject(string resKeyString)
        {
       	ResourceKey sewableKey = ResourceKey.FromString(resKeyString);
       	GameObject mObjectChosen = (GameObject)GlobalFunctions.CreateObject(sewableKey, target2.GetSlotPosition(Slot.ContainmentSlot_1), 0, target2.GetForwardOfSlot(Slot.ContainmentSlot_1), null, null);

        // Change opacity to invisible. So it's existing already but just invisble :)
        mObjectChosen.SetOpacity(0f, 0f);
       		
       	//Check (and set) which skill level the chosen object belongs to. (See function SewingObjects > test())
       	if(ObjectLoader.EasySewablesList.Contains(sewableKey))
       	{
       		mCurrentSkillLevel = 0;
       	}
       	if(ObjectLoader.MediumSewablesList.Contains(sewableKey))
       	{
       		mCurrentSkillLevel = 3;
       	}
       	if(ObjectLoader.HardSewablesList.Contains(sewableKey))
       	{
       		mCurrentSkillLevel = 6;
       	}
       	if(ObjectLoader.HarderSewablesList.Contains(sewableKey))
       	{
       		mCurrentSkillLevel = 9;
       	}
       	if(mSimDescription.IsWitch || (mSimDescription.IsFairy) || (mSimDescription.IsGenie) || (mSimDescription.IsImaginaryFriend))
		{
	       	if(ObjectLoader.MagicHarderSewablesList.Contains(sewableKey))
	       	{
	       		mCurrentSkillLevel = 9;
	       	}
       	}
       	for(int i = 0; i < ObjectLoader.sewableSettings.Count; i++)
       	{
       		if(ObjectLoader.sewableSettings[i].key == sewableKey)
	       	{
       			foreach(SewingSkill.FabricType fabrics in ObjectLoader.sewableSettings[i].typeFabric)
       			{
       				//Knitted, Cotton, Satin, Leather, Denim, Synthetic
       				if(fabrics == SewingSkill.FabricType.Knitted)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Knitted, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       				if(fabrics == SewingSkill.FabricType.Cotton)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Cotton, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       				if(fabrics == SewingSkill.FabricType.Satin)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Satin, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       				if(fabrics == SewingSkill.FabricType.Leather)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Leather, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       				if(fabrics == SewingSkill.FabricType.Denim)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Denim, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       				if(fabrics == SewingSkill.FabricType.Synthetic)
	       			{
       					if(!IngredientFabric.RemoveFabricForProject(target2, ActorCurr, SewingSkill.FabricType.Synthetic, ObjectLoader.sewableSettings[i].amountRemoveFabric))
       					{
       						return false;
       					}
	       			}
       			}
	       	}
       	}
       		
       	mObjectChosenGeneral = mObjectChosen;

       	if(mObjectChosen != null)
       	{
       		return true;
       	}
       	return false;
    }
       
       public override void Dispose()
       {
       		Inventory inventory = base.Inventory;
			inventory.EventCallback = (InventoryEventCallback)Delegate.Remove(inventory.EventCallback, new InventoryEventCallback(InventoryEventCallback));
			if (base.Inventory != null)
			{
					base.Inventory.DestroyItems();
			}
			base.Dispose();
       }
		
       	public override bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
		{
			bool result = base.ExportContent(resKeyTable, objIdTable, writer);
			writer.WriteBool(0x7A82A2C9, mHasInitialFabric);
			return result;
		}
	
		public override bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
		{
			bool result = base.ImportContent(resKeyTable, objIdTable, reader);
			reader.ReadBool(0x7A82A2C9, out mHasInitialFabric, false);
			if (Progress > 0f)
			{
				GameObject gameObject = base.GetContainedObject(Slot.ContainmentSlot_0) as GameObject;
				if (gameObject != null)
				{
					mObjectChosenGeneral = gameObject;
				}
			}
			return result;
		}
       
		
		//Placeholder from now on.
		public static ResourceKey fabricIngredientTESTRK = new ResourceKey(0xC480800032188243, 0x319E4F1D, 0x00000000);
		
		public void AddPlaceholderSewable()
		{
			if(mObjectChosenGeneral != null)
			{
				mObjectChosenGeneral.SetOpacity(0f, 0f);
			}
			if(mStoredObject != null)
			{
				mStoredObject.SetOpacity(0f, 0f);
			}
			
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
		}
		
		public static float GetTimeToCompletion()
		{
			SewingSkill skill = ActorCurr.SkillManager.GetSkill<SewingSkill>(SewingSkill.kSewingSkillGUID);
			float num = MathHelpers.LinearInterpolate(0f, (float)skill.MaxSkillLevel, (float)kBaseMinTimeMakeSewable, (float)kBaseMaxTimeMakeSewable, (float)skill.SkillLevel);
			if (num <= 0f )
			{
				num = (float)kBaseMinTimeMakeSewable;
			}
			return num; //30f;
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