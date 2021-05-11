using System;
using System.Collections.Generic;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Socializing;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// This is an ingredient that can be used only for Sewable projects.
	/// </summary>
	public class IngredientFabric : GameObject, IObjectUI, IScriptObject, IScriptLogic
	{
		public bool IsMagicFabric = false;
		
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
            base.ItemComp.mValidInventories = new Type[]
            {
               typeof(Sim),
               typeof(SewingTable),
            };

            base.AddInteraction(PutInInventory.Singleton);
			base.OnStartup();
		}
		
		public override void OnCreation()
		{
			FabricInitParameters fabricInitParameters = Simulator.GetObjectInitParameters(this.ObjectId) as FabricInitParameters;
			if (fabricInitParameters != null)
			{
				NumScraps = fabricInitParameters.Amount;
			}
			base.OnCreation();
		}
		
		public static IGameObject objAdded;
		
		public static string mMakeObject = "IngredientFabric";
		
		public static IGameObject mNewObject;
		
		// OBJD key for Ingredient Fabric
		public static ResourceKey fabricIngredientRK = new ResourceKey(0xC480800032188243, 0x319E4F1D, 0x00000000);
		
		public static ResourceKey fabricKnittedRK = new ResourceKey(0x3C96B7DA2CBDE481, 0x319E4F1D, 0x00000000);
		public static ResourceKey fabricCottonRK = new ResourceKey(0x2DBDD15E39D569EF, 0x319E4F1D, 0x00000000);
		public static ResourceKey fabricLeatherRK = new ResourceKey(0x11161CED442DEF00, 0x319E4F1D, 0x00000000);
		public static ResourceKey fabricSyntheticRK = new ResourceKey(0x3E63BBD043EFFA82, 0x319E4F1D, 0x00000000);
		public static ResourceKey fabricDenimRK = new ResourceKey(0x3DDE8AF95DC13323, 0x319E4F1D, 0x00000000);
		public static ResourceKey fabricSatinRK = new ResourceKey(0x7F40C19E35F4924F, 0x319E4F1D, 0x00000000);
		
		
		public bool HasBeenCounted;
		
		public int mNumScraps = 10;
		
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
		public static void PostLoadWorldFixup()
		{
			List<IngredientFabric> list = new List<IngredientFabric>();
			List<IGameObject> list2 = new List<IGameObject>();
			List<IngredientFabric> list3 = new List<IngredientFabric>(Sims3.Gameplay.Queries.GetObjects<IngredientFabric>());
			foreach (IngredientFabric item in list3)
			{
				if (item.ItemComp == null || item.ItemComp.InventoryParent == null || item.ItemComp.InventoryParent.Owner == null)
				{
					if (item.Position == Vector3.Invalid || item.Position == Vector3.OutOfWorld || item.Position == Vector3.Zero)
					{
						list.Add(item);
					}
				}
				else
				{
					IGameObject owner = item.ItemComp.InventoryParent.Owner;
					if (!(owner is SharedFamilyInventory))
					{
						if (list2.Contains(owner))
						{
							list.Add(item);
						}
						else if (!Sims3.SimIFace.Objects.IsValid(owner.ObjectId))
						{
							list.Add(item);
							list2.Add(owner);
						}
					}
				}
			}
			foreach (IngredientFabric item2 in list)
			{
				item2.Destroy();
			}
			foreach (IGameObject item3 in list2)
			{
				item3.Destroy();
			}
		}
		
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
		
		public static bool RemoveFabricForProject(GameObject obj, Sim actor, int amount)
		{
			List<IngredientFabric> list = actor.Inventory.FindAll<IngredientFabric>(true);
			List<IngredientFabric> list2 = obj.Inventory.FindAll<IngredientFabric>(true);
			List<IngredientFabric> list3 = new List<IngredientFabric>();
			
			if(obj != null && obj.Inventory != null)
			{
				if(list2.Count + list.Count < amount)
				{
					return false;
				}
				list3.AddRange(list2);
				list3.AddRange(list);

				for(int j = 0; j < amount; j++)
				{
					IngredientFabric fabric = list3[j];
					if(!obj.Inventory.TryToRemove(fabric))
					{
						SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
						return false;
					}
					list3.Remove(fabric);
					SewingTable.print("Was able to remove fabric from table");
				}
			}
			return false;
		}
	
		
		// If we know the type of fabric, use this one instead
		public static bool RemoveFabricForProject(GameObject obj, Sim actor, SewingSkill.FabricType type, int amount)
		{
			
			List<FabricCotton> listCotton 			= actor.Inventory.FindAll<FabricCotton>(true);
			List<FabricCotton> list2Cotton 			= obj.Inventory.FindAll<FabricCotton>(true);
			
			List<FabricDenim> listDenim 			= actor.Inventory.FindAll<FabricDenim>(true);
			List<FabricDenim> list2Denim 			= obj.Inventory.FindAll<FabricDenim>(true);
			
			List<FabricKnitted> listKnitted 		= actor.Inventory.FindAll<FabricKnitted>(true);
			List<FabricKnitted> list2Knitted 		= obj.Inventory.FindAll<FabricKnitted>(true);
			
			List<FabricLeather> listLeather 		= actor.Inventory.FindAll<FabricLeather>(true);
			List<FabricLeather> list2Leather 		= obj.Inventory.FindAll<FabricLeather>(true);
			
			List<FabricSatin> listSatin 			= actor.Inventory.FindAll<FabricSatin>(true);
			List<FabricSatin> list2Satin 			= obj.Inventory.FindAll<FabricSatin>(true);
			
			List<FabricSynthetic> listSynthetic 	= actor.Inventory.FindAll<FabricSynthetic>(true);
			List<FabricSynthetic> list2Synthetic 	= obj.Inventory.FindAll<FabricSynthetic>(true);
			
			List<FabricCotton> list3Cotton 			= new List<FabricCotton>();
			List<FabricDenim> list3Denim 			= new List<FabricDenim>();
			List<FabricKnitted> list3Knitted 		= new List<FabricKnitted>();
			List<FabricLeather> list3Leather 		= new List<FabricLeather>();
			List<FabricSatin> list3Satin 			= new List<FabricSatin>();
			List<FabricSynthetic> list3Synthetic 	= new List<FabricSynthetic>();
			
			if(obj != null && obj.Inventory != null)
			{
				if(type == SewingSkill.FabricType.Cotton)
				{
					list3Cotton.AddRange(listCotton);
					list3Cotton.AddRange(list2Cotton);
					if(list3Cotton.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricCotton fabric = list3Cotton[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Cotton.Remove(fabric);
					}
				}
				
				if(type == SewingSkill.FabricType.Denim)
				{
					list3Denim.AddRange(listDenim);
					list3Denim.AddRange(list2Denim);
					if(list3Denim.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricDenim fabric = list3Denim[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Denim.Remove(fabric);
					}
				}
				
				if(type == SewingSkill.FabricType.Knitted)
				{
					list3Knitted.AddRange(listKnitted);
					list3Knitted.AddRange(list2Knitted);
					if(list3Knitted.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricKnitted fabric = list3Knitted[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Knitted.Remove(fabric);
					}
				}
				if(type == SewingSkill.FabricType.Leather)
				{
					list3Leather.AddRange(listLeather);
					list3Leather.AddRange(list2Leather);
					if(list3Leather.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricLeather fabric = list3Leather[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Leather.Remove(fabric);

					}
				}	
				if(type == SewingSkill.FabricType.Satin)
				{
					list3Satin.AddRange(listSatin);
					list3Satin.AddRange(list2Satin);
					if(list3Satin.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricSatin fabric = list3Satin[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Satin.Remove(fabric);
					}
				}				
				if(type == SewingSkill.FabricType.Synthetic)
				{
					list3Synthetic.AddRange(listSynthetic);
					list3Synthetic.AddRange(list2Synthetic);
					if(list3Synthetic.Count < amount)
					{
						SewingTable.print("Lyralei's Sewing Table: all inventories don't have enough fabric in it");
						return false;
					}	
					for(int j = 0; j < amount; j++)
					{
						FabricSynthetic fabric = list3Synthetic[j];
						if(!obj.Inventory.TryToRemove(fabric))
						{
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric ingredient, Destroyed item.");
							return false;
						}
						list3Synthetic.Remove(fabric);
					}
				}
			}
			return true;
		}
		
		public static bool CreateAndAddToInventory(GameObject obj, int amount)
		{
			if(obj != null && obj.Inventory != null)
			{
				for(int i = 0; i < amount; i++)
				{
					FabricInitParameters initData = new FabricInitParameters(1);
					
					IGameObject Knitted = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricKnittedRK, null, initData);
					FabricKnitted ingredientKnitted = Knitted as FabricKnitted;
					
					IGameObject Cotton = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricCottonRK, null, initData);
					FabricCotton ingredientCotton = Cotton as FabricCotton;
					
					IGameObject leather = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricLeatherRK, null, initData);
					FabricLeather ingredientLeather = leather as FabricLeather;
					
					IGameObject synthetic = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricSyntheticRK, null, initData);
					FabricSynthetic ingredientSynthetic = synthetic as FabricSynthetic;

					IGameObject denim = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricDenimRK, null, initData);
					FabricDenim ingredientDenim = denim as FabricDenim;

					IGameObject satin = (IngredientFabric)GlobalFunctions.CreateObjectOutOfWorld(fabricSatinRK, null, initData);
					FabricSatin ingredientSatin = satin as FabricSatin;
				
					if(ingredientKnitted != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientKnitted))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Knitted, Destroyed item.");
							ingredientKnitted.Destroy();
							return false;
						}
					}
					if(ingredientLeather != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientLeather))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Leather, Destroyed item.");
							ingredientLeather.Destroy();
							return false;
						}
					}
					if(ingredientSynthetic != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientSynthetic))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Synthetic, Destroyed item.");
							ingredientSynthetic.Destroy();
							return false;
						}
					}					
					if(ingredientCotton != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientCotton))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Cotton, Destroyed item.");
							ingredientCotton.Destroy();
							return false;
						}
					}				
					if(ingredientDenim != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientDenim))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Denim, Destroyed item.");
							ingredientDenim.Destroy();
							return false;
						}
					}			
					if(ingredientSatin != null)
					{
						if(!obj.Inventory.TryToAdd(ingredientSatin))
						{	
							SewingTable.print("Lyralei's SewingTable: Couldn't add Fabric Satin, Destroyed item.");
							ingredientSatin.Destroy();
							return false;
						}
					}						
				}
                return true;
			}
            return false;
		}
		
		public override bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
		{
			bool result = base.ExportContent(resKeyTable, objIdTable, writer);
			writer.WriteInt32(0x0243DF80, NumScraps);
			return result;
		}
	
		public override bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
		{
			bool result = base.ImportContent(resKeyTable, objIdTable, reader);
			reader.ReadInt32(0x0243DF80, out mNumScraps, 1);
			return result;
		}
	}
}
