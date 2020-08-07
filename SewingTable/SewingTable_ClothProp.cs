using System;
using Sims3.Gameplay.Abstracts;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// This is a class cannot be edited whatsoever. You'll break the sewing machine *for real*. With that, it will make Lyralei cry. 
	/// Don't, please for the love of coffee, use this script as a reference. 
	/// 
	/// What it does is it initiates the cloth prop on the table as a game object. It's therefore not connected to the sim's hand but the Sewing table's slots. 
	/// </summary>
	public class SewingTable_ClothProp : GameObject
	{
		public override void OnCreation()
		{
			base.OnCreation();
		}
	}
}
