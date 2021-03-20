using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	class OdinShaman : OdinNPC
	{
		private void Start()
		{
			
		}
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			return true;
		}
		public override void SecondaryInteract()
		{

		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=blue><b>{0}}/b></color>", m_name);
			n += string.Format("\n<color=green><b>Score:{0}</b></color>", OdinScore.score);
			return Localization.instance.Localize(n);
		}
		public override string GetHoverName()
		{
			return Localization.instance.Localize(this.m_name);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}
	}
}