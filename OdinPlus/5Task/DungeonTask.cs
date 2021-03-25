using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class DungeonTask : OdinTask
	{
		private void Awake()
		{
			if (loading)
			{
				return;
			}
			m_type = TaskManager.TaskType.Dungeon;
			m_tier1 = new string[] { "WoodHouse3", "WoodHouse4", "Ruin2", "Ruins1", "ShipSetting01", "Runestone_Boars", "Runestone_Meadows", "Runestone_Greydwarfs", "Runestone_BlackForest" };
			m_tier2 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };
			m_tier3 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };
			m_tier4 = new string[] { "SwampRuinX", "SwampRuinY", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr" };
			
			base.Begin();
		}
	}
}