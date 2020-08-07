using System;
using System.Collections.Generic;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// This is an ingredient that can be used only for Sewable projects.
	/// </summary>
	public class IngredientFabric : GameObject, IGameObject, IObjectUI, IScriptObject, IScriptLogic
	{
		public static List<SewingTable> sewingTableType;
		
		public override void OnStartup()
		{

			base.AddComponent<ItemComponent>(new object[1]
			{
				new List<Type>(new Type[2]
				{
					typeof(Sim),
					typeof(SewingTable)
				})
			});
			
			base.AddInteraction(PutInInventory.Singleton);
			//base.AddInteraction(Pickup.PickupSingleton);
		}
		
		public override void OnCreation()
		{
//			FabricInitParameters fabricInitParameters = Simulator.GetObjectInitParameters(this.ObjectId) as FabricInitParameters;
//			if (fabricInitParameters != null)
//			{
//				NumScraps = fabricInitParameters.Amount;
//			}
			base.OnCreation();
		}
		
		public static IGameObject objAdded;
		
		public static string mMakeObject = "IngredientFabric";
		
		public static IGameObject mNewObject;
		
		// OBJD key for Ingredient Fabric
		public static ResourceKey fabricIngredientRK = new ResourceKey(0xC480800032188243, 0x319E4F1D, 0x00000000);
		
		public bool HasBeenCounted;
		
		public int mNumScraps = 1;
		
		public int NumScraps
		{
			get
			{
				return mNumScraps;
			}
			set
			{
				mNumScraps = value;
			}
		}
		
		// Kindly stolen from scrap :p
//		public static void PostLoadWorldFixup()
//		{
//			List<IngredientFabric> list = new List<IngredientFabric>();
//			List<IGameObject> list2 = new List<IGameObject>();
//			List<IngredientFabric> list3 = new List<IngredientFabric>(Sims3.Gameplay.Queries.GetObjects<IngredientFabric>());
//			foreach (IngredientFabric item in list3)
//			{
//				if (item.ItemComp == null || item.ItemComp.InventoryParent == null || item.ItemComp.InventoryParent.Owner == null)
//				{
//					if (item.Position == Vector3.Invalid || item.Position == Vector3.OutOfWorld || item.Position == Vector3.Zero)
//					{
//						list.Add(item);
//					}
//				}
//				else
//				{
//					IGameObject owner = item.ItemComp.InventoryParent.Owner;
//					if (!(owner is SharedFamilyInventory))
//					{
//						if (list2.Contains(owner))
//						{
//							list.Add(item);
//						}
//						else if (!Sims3.SimIFace.Objects.IsValid(owner.ObjectId))
//						{
//							list.Add(item);
//							list2.Add(owner);
//						}
//					}
//				}
//			}
//			foreach (IngredientFabric item2 in list)
//			{
//				item2.Destroy();
//			}
//			foreach (IGameObject item3 in list2)
//			{
//				item3.Destroy();
//			}
//		}
//		
		public override bool StacksWith(IGameObject other)
		{
			IngredientFabric ingredientFabric = other as IngredientFabric;
			if(ingredientFabric != null)
			{
				return true;
			}
			return false;
		}
		
		public override bool PackForMovingOverride()
		{
			return true;
		}
		
		public static bool CreateAndAddToInventory(GameObject obj, int amount)
		{
			if(obj != null && obj.Inventory != null)
			{
				for(int i = 0; i < amount; i++)
				{
					FabricInitParameters initData = new FabricInitParameters(amount);
					IGameObject gameObject = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricIngredientRK, null, initData);
					IngredientFabric ingredientFabric = gameObject as IngredientFabric;
					
					if(ingredientFabric != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientFabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							ingredientFabric.Destroy();
							return false;
						}
						return true;
					}
					else if(gameObject != null)
					{
						SewingTable.print("Lyralei's SewingTable: was regular gameObject and has been destroyed, object was: \n" + gameObject.GetLocalizedName().ToString());
						gameObject.Destroy();
						return false;
					}
				}
				return false;
			}
			return false;
		}
	}
}
