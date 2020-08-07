using System;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Seating;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.Gameplay.Objects.HobbiesSkills;

namespace Sims3.Gameplay.Objects.Lyralei
{
	public class SewingTable : GameObject
	{
		/* Pre-startup
		*  - Get Fabric/cloth
		*  - Find the chair
		*  - Skill level (Do later)
		*  - Progress (Do later)
		*  - Get all sewing thingies you can make!
		*/
		
		/* Startup
		*  - Make sure that the sim is sitting on the chair
		*  - That the table is not in use. 
		*/
		
	   /* Progress
		*  - Initiate the Fabric/Cloth prop. 
		*  - Grab the Jazz script.
		*  - Skillset activated (Do later)
		*  - base.BeginCommodityUpdates();
		*  - Aquire the animation name. 
		*  - Call the sims, but also the objects (cloth included).
		*  - Animate the sim or object
		*  - After finished/canceled, play exit animation
		*  - base.EndCommodityUpdates();
		*  - base.StandardExit();
		*/
		
		/* EXIT
		 *  - After done, put crafty thing in Inventory. 
		 */

		public class Sewing : Interaction<Sim, SewingTable>
        {
            public class Definition :  InteractionDefinition<Sim, SewingTable, SewingTable.Sewing>
            {
                public override string GetInteractionName(Sim actor, SewingTable target, InteractionObjectPair interaction)
                {
                    return "Talk To Me";
                }
                
                public override bool Test(Sim actor, SewingTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                	if (target.Chair == null)
					{
						//greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("HasNoChair"));
						actor.ShowTNSIfSelectable("This table has no chair! :(", StyledNotification.NotificationStyle.kSimTalking);
						return false;
					}
					if (target.InUse)
					{
						 actor.ShowTNSIfSelectable("This table is already occupied!", StyledNotification.NotificationStyle.kSimTalking);
						return false;
					}
                    return true;
                }
            }
            
            public static readonly InteractionDefinition Singleton = new Definition();
            
            public SkillNames mSkillName;
            
            public override bool Run()
            {
            	
				if (!base.Target.ScootInActor(base.Actor))
				{
					base.Actor.AddExitReason(ExitReason.FailedToStart);
					return false;
				}
				
            	base.StandardEntry();
            	base.Actor.LookAtManager.DisableLookAts();
				Skill skill = base.Actor.SkillManager.AddElement(SkillNames.Painting);
				
				if (base.Actor.TraitManager.HasElement(TraitNames.Artistic))
				{
					base.Actor.SkillManager.ChangeSkillGainModifier(skill.Guid, 0.5f);
				}
				
				base.BeginCommodityUpdates();
            	
				//Add in object GUID/ Prop whenever done
				
				/* Needs to line up with Jazz script:
 				* "SewingTable" = MachineName,
				* "Enter" = Entry state 
				* "X" = Actor
				* "sewingtable_table" = Actor (table)
				*/
				
				base.AcquireStateMachine("SewingTable");
				//base.EnterStateMachine("SewingTable", "Enter", "x");
				
				base.SetActor("x", base.Actor);
				base.SetActor("sewingtable_table", base.Target);
				base.SetActor("chairDining", base.Actor.Posture.Container);
				
				// Defines Actor and State name
            	base.EnterState("x", "Enter");
            	
            	bool flag = DoLoop(ExitReason.Default, SewingLoop, base.mCurrentStateMachine);
            	
            	
            	base.Actor.LookAtManager.EnableLookAts();
            	base.AnimateSim("Exit");
            	
            	base.EndCommodityUpdates(flag);
            	base.StandardExit();
                base.Actor.ShowTNSIfSelectable("I just successfully played!", StyledNotification.NotificationStyle.kSimTalking);
                return flag;
            }
            
           public void SewingLoop(StateMachineClient smc, LoopData ld)
		   {
				StyledNotification.Show(new StyledNotification.Format("Looping is happening!  from SewingLoop function", StyledNotification.NotificationStyle.kGameMessageNegative));
		   }
           
           public override void Cleanup()
           {
           	base.Cleanup();
           }
            
        }
		
	

		
		public ChairDining Chair
		{
			get
			{
				return SewingTable.GetContainedObject(Slot.ContainmentSlot_0) as ChairDining;
			}
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
		
		
		
		public override void OnCreation()
		{
			base.OnCreation();
		}
		
		public override void OnStartup()
		{
			base.AddInteraction(Sewing.Singleton);
		}
	}
}