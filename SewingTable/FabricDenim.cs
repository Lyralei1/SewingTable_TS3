using System;
using System.Collections.Generic;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Lyralei;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// Description of FabricKnitted.
	/// </summary>
	public class FabricDenim : IngredientFabric
	{
		public override void OnStartup()
		{
//			this.FabricType.Denim;
			base.AddComponent<ItemComponent>(new object[1]
			{
				new List<Type>(new Type[2]
				{
					typeof(Sim),
					typeof(SewingTable)
				})
			});
			base.AddInteraction(PutInInventory.Singleton);
			base.OnStartup();
		}
		
		public override bool StacksWith(IGameObject other)
		{
			FabricDenim ingredientFabric = other as FabricDenim;
			if(ingredientFabric != null)
			{
				return true;
			}
			return false;
		}
	}
}
