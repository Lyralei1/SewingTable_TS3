/*
 * Created by SharpDevelop.
 * User: Lisa
 * Date: 19/06/2020
 * Time: 10:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Lyralei;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Lyralei;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// Description of JoinPatternClub.
	/// </summary>
	public class JoinPatternClub : Computer.ComputerInteraction
	{
		public class Definition : InteractionDefinition<Sim, Computer, JoinPatternClub>
		{
			public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
			{
				return Localization.LocalizeString("Lyralei/Localized/JoinPatternClub:InteractionName", new object[0]);
			}
			public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				if (a.SkillManager.HasElement(SewingSkill.kSewingSkillGUID) && target.IsComputerUsable(a, true, false, isAutonomous) && !SewingSkill.isInPatternClub(a.SimDescription))
				{
					return a.FamilyFunds >= kCost;
				}
				return false;
			}
		}
	
		public static InteractionDefinition Singleton = new Definition();
		
		[TunableComment("Range:  Simoleons.  Description:  Lifetime membership cost of joining the pattern club.")]
		[Tunable]
		public static int kCost = 200;
		
		public Sim mActor;
		
		public override bool Run()
		{
			base.StandardEntry();
			if (!base.Target.StartComputing(this, SurfaceHeight.Table, true))
			{
				base.StandardExit();
				return false;
			}
			//mActor = Actor;
			base.Target.StartVideo(Computer.VideoType.Browse);
			base.BeginCommodityUpdates();
			base.AnimateSim("WorkTyping");
			bool flag = TwoButtonDialog.Show(Localization.LocalizeString("Lyralei/Localized/JoinPatternClubDesc:InteractionName", new object[0]) + kCost.ToString(), Localization.LocalizeString("Lyralei/Localized/JoinPatternClubYES:InteractionName", new object[0]), Localization.LocalizeString("Lyralei/Localized/JoinPatternClubNO:InteractionName", new object[0]));
			if (flag)
			{
				if (base.Actor.FamilyFunds >= kCost)
				{
					base.Actor.ModifyFunds(-kCost);
				}
				else if (!GameUtils.IsFutureWorld())
				{
					//base.Actor.UnpaidBills += kCost;
					StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Lyralei/Localized/NotEnoughMoney:Test", new object[0]), base.Actor.ObjectId, base.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessageNegative);
					StyledNotification.Show(format);
				}
                Mailbox mailbox = Mailbox.GetMailboxOnLot(base.Actor.LotHome);

                mailbox.AddAlarmDay(1f, DaysOfTheWeek.Thursday, GlobalOptionsSewingTable.SendPatterns, "Mailbox:  Pattern club " + base.Actor.mSimDescription.mSimDescriptionId.ToString(), AlarmType.AlwaysPersisted);
				GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.Add(Actor.SimDescription.mSimDescriptionId, true);
				base.Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/JoinedPatternClub:InteractionName", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive);
			}
			base.Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
			base.EndCommodityUpdates(flag);
			base.StandardExit();
			return true;
		}
	}

    public class BuyClothingPatterns : Computer.ComputerInteraction
    {
        public class Definition : InteractionDefinition<Sim, Computer, BuyClothingPatterns>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Localization.LocalizeString("Lyralei/Localized/BuyClothingPattern:InteractionName", new object[0]);
            }
            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                SewingSkill sewingSkill = a.SkillManager.GetElement(SewingSkill.kSewingSkillGUID) as SewingSkill;
                if (a.SkillManager.HasElement(SewingSkill.kSewingSkillGUID) && target.IsComputerUsable(a, true, false, isAutonomous) && sewingSkill.SkillLevel > 6)
                {
                    return a.FamilyFunds >= kCost;
                }
                return false;
            }
        }

        public static InteractionDefinition Singleton = new Definition();

        [TunableComment("Range:  Simoleons.  Description:  Lifetime membership cost of joining the pattern club.")]
        [Tunable]
        public static int kCost = 200;

        public Sim mActor;

        public override bool Run()
        {
            base.StandardEntry();
            if (!base.Target.StartComputing(this, SurfaceHeight.Table, true))
            {
                base.StandardExit();
                return false;
            }
            mActor = Actor;
            base.Target.StartVideo(Computer.VideoType.Browse);
            base.BeginCommodityUpdates();
            base.AnimateSim("WorkTyping");
            bool flag = TwoButtonDialog.Show(Localization.LocalizeString("Lyralei/Localized/BuyClothingPatternDialog:InteractionName"), Localization.LocalizeString("Ui/Caption/Global:Yes", new object[0]), Localization.LocalizeString("Ui/Caption/Global:No", new object[0]));
            if (flag)
            {
                if (base.Actor.FamilyFunds >= kCost)
                {
                    base.Actor.ModifyFunds(-kCost);
                }
                else if (!GameUtils.IsFutureWorld())
                {
                    //base.Actor.UnpaidBills += kCost;
                    StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Lyralei/Localized/NotEnoughMoney:Test", new object[0]), base.Actor.ObjectId, base.Target.ObjectId, StyledNotification.NotificationStyle.kGameMessageNegative);
                    StyledNotification.Show(format);
                }
                Pattern pattern = Pattern.GetRandomClothingPattern(base.Actor);
                SimDescription ActorDesc = Actor.SimDescription;
                base.Actor.ShowTNSIfSelectable(Localization.LocalizeString("Lyralei/Localized/GotPattern:InteractionName", new object[0]) + pattern.mPatternInfo.Name, StyledNotification.NotificationStyle.kGameMessagePositive);
            }
            base.Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
            base.EndCommodityUpdates(flag);
            base.StandardExit();
            return true;
        }
    }
}
