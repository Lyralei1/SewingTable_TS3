using System;
using Lyralei;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Lyralei;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	public class BrowseSewingDIY : Computer.ComputerInteraction
	{
		public sealed class Definition : InteractionDefinition<Sim, Computer, BrowseSewingDIY>
		{
			public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
			{
				return Localization.LocalizeString("Lyralei/Localized/BrowseWebForPatterns:InteractionName", new object[0]);
			}
			public override bool Test(Sim actor, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				int skillLevel = actor.SkillManager.GetSkillLevel(SewingSkill.kSewingSkillGUID);
				if(skillLevel > 2)
				{
					return target.IsComputerUsable(actor, true, false, isAutonomous);
				}
				return false;
			}
		}
		public static InteractionDefinition Singleton = new Definition();
		 
		public override bool Run()
		{
			base.StandardEntry();
			if(!base.Target.StartComputing(this, SurfaceHeight.Table, true))
			{
				base.StandardExit();
				return false;
			}
			base.Target.StartVideo(Computer.VideoType.Browse);
			base.BeginCommodityUpdates();
			base.AnimateSim("GenericTyping");
			bool flag = base.DoLoop(ExitReason.Default, LoopDel, null);
			base.EndCommodityUpdates(flag);
			base.Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
			if(RandomUtil.RandomChance(20f))
			{
				Pattern randomPattern = Pattern.DiscoverPatternForGlobalObjects(base.Actor);
				if(randomPattern != null)
				{
					base.Actor.Inventory.TryToAdd(randomPattern);
					base.Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/BrowseWebForPatternsSuccess:InteractionName", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
				}
			}
			base.StandardExit();
			return flag;
		}
		
		public void LoopDel(StateMachineClient smc, LoopData loopData)
		{
			if (base.Actor.HasTrait(TraitNames.AntiTV) && loopData.mLifeTime > Computer.kTechnophobeTraitMaximumChatTime)
			{
				base.Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/BrowseWebForPatternsHateComputers:InteractionName", new object[0]), StyledNotification.NotificationStyle.kSimTalking);
				base.Actor.AddExitReason(ExitReason.Finished);
			}
		}
		public void UpdateBuffs()
		{
			// Add buff "excitement!" 
		}
	}
}
