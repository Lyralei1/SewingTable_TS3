/*
 * Created by SharpDevelop.
 * User: Lisa
 * Date: 19/06/2020
 * Time: 11:11
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
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// Description of LeavePatternClub.
	/// </summary>
	public class LeavePatternClub : Sims3.Gameplay.Objects.Electronics.Computer.ComputerInteraction
	{
		public class Definition : InteractionDefinition<Sim, Computer, LeavePatternClub>
		{
			public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
			{
					return Localization.LocalizeString("Lyralei/Localized/LeavePatternClub:InteractionName", new object[0]);
			}
			public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
			{
				if (target.IsComputerUsable(a, true, false, isAutonomous))
				{
					return SewingSkill.isInPatternClub(a.SimDescription);
				}
				return false;
			}
		}
	
		public static InteractionDefinition Singleton = new Definition();

//		public static string LocalizeString(string name, params object[] parameters)
//		{
//			return Localization.LocalizeString("Gameplay/Objects/Electronics/Computer/LeaveBookClub:" + name, parameters);
//		}

		public override bool Run()
		{
			base.StandardEntry();
			if (!base.Target.StartComputing(this, SurfaceHeight.Table, true))
			{
				base.StandardExit();
				return false;
			}
			base.Target.StartVideo(Computer.VideoType.Browse);
			base.BeginCommodityUpdates();
			base.AnimateSim("WorkTyping");
			if (TwoButtonDialog.Show(Localization.LocalizeString("Lyralei/Localized/LeavePatternClubDesc:InteractionName", new object[0]), Localization.LocalizeString("Lyralei/Localized/LeavePatternClubYES:InteractionName", new object[0]), Localization.LocalizeString("Lyralei/Localized/LeavePatternClubNO:InteractionName", new object[0])))
			{
				SimDescription ActorDesc = base.Actor.SimDescription;
				if(GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.ContainsKey(ActorDesc.mSimDescriptionId))
				{
					GlobalOptionsSewingTable.retrieveData.whoIsInPatternClub.Remove(ActorDesc.mSimDescriptionId);
				}
                Mailbox mailbox = Mailbox.GetMailboxOnLot(base.Actor.LotHome);
                mailbox.RemoveAlarm(GlobalOptionsSewingTable.mPatternClubAlarm);
				GlobalOptionsSewingTable.mPatternClubAlarm = AlarmHandle.kInvalidHandle;
			}
			base.Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
			base.EndCommodityUpdates(true);
			base.StandardExit();
			return true;
		}
	}

}
