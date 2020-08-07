using System;
using System.Collections.Generic;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.SimIFace;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// THIS SCRIPT SHOULD NEVER BE ALTERED. It's a bridge on only getting game objects that are sewable.
	/// </summary>
	public class Sewables : GameObject
	{
		public override void OnLoad()
		{
			GetThumbnailKey();
			base.OnLoad();
		}
		
		public override void OnStartup()
		{
			base.AddComponent<ItemComponent>(new object[1]
			{
				new List<Type>(new Type[3]
				{
					typeof(Sim),
					typeof(SharedFamilyInventory),
					typeof(SewingTable),
				})
			});
			base.AddInteraction(PutInInventory.Singleton);
			base.OnCreation();
		}

		public override void OnCreation()
		{
			GetThumbnailKey();
			base.OnCreation();
		}

		public bool IsDraggable = true;

		public IGameObject objects;

//		public override bool StacksWith(IGameObject other)
//		{
//			Sewables sewables = other as Sewables;
//			if(sewables != null)
//			{
//				return true;
//			}
//			return false;
//		}
		
		public override bool HandToolAllowUserPickup()
		{
			if (!IsDraggable)
			{
				return false;
			}
			return base.HandToolAllowUserPickup();
		}
	}
}
