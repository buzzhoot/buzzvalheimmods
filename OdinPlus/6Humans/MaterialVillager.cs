using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class MaterialVillager : HumanVillager, Hoverable, Interactable, OdinInteractable
	{
		public readonly string[] m_item = new string[] { "Wood", "Stone" };
		protected override void Awake()
		{
			base.Awake();
            
		}
	}
}
