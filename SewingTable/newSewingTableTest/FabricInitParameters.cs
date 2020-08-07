using System;
using Sims3.SimIFace;

namespace Sims3.Gameplay.Objects.Lyralei
{
	/// <summary>
	/// Used to transfer data between the scrap and the Sewing table
	/// </summary>
	public class FabricInitParameters : Simulator.ObjectInitParameters
	{
		public int Amount;
	
		public FabricInitParameters()
		{
		}
	
		public FabricInitParameters(int amount)
		{
			Amount = amount;
		}
	}
}
