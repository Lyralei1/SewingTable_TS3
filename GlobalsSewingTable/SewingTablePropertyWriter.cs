/*
 * Created by SharpDevelop.
 * User: Lisa
 * Date: 06/07/2020
 * Time: 14:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Lyralei;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Skills.Lyralei;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace GlobalDLLImporterTest
{
	/// <summary>
	/// Description of SewingTablePropertyWriter.
	/// </summary>
	public class SewingTablePropertyWriter : PropertyStreamWriter
	{
		public List<string> ExportedSewablesAsString = new List<string>();
		
		public bool Export(List<ResourceKey> DiscoveredSewables)
		{
			if (DiscoveredSewables == null)
			{
				return true;
			}
			base.mStream.Write(1);
			int count = DiscoveredSewables.Count;
			base.mStream.Write(count);
			uint num = 0x35D115EE;
			foreach (ResourceKey telescopeDiscovery in DiscoveredSewables)
			{
				IPropertyStreamWriter propertyStreamWriter = base.CreateChild(num++);
				ExportedSewablesAsString.Add(telescopeDiscovery.ToString());
				propertyStreamWriter.WriteString(0x35D115EE, telescopeDiscovery.ToString());
				//telescopeDiscovery.Export(propertyStreamWriter);
				base.CommitChild();
			}
			return true;
		}
	}
	
	public class SewingTablePropertyReader : PropertyStreamReader
	{
		public bool Import(out List<ResourceKey> DiscoveredSewables)
		{
			DiscoveredSewables = new List<ResourceKey>();
			if (base.mStream == null)
			{
				return false;
			}
			uint num = base.mStream.ReadUInt32();
			if (num < 1)
			{
				return false;
			}
			uint num2 = base.mStream.ReadUInt32();
			foreach(ResourceKey key in DiscoveredSewables)
			{
				DiscoveredSewables.Add(key);
			}
			return true;
		}
	}
}
