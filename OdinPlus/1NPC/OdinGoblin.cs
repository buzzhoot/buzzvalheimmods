using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	class OdinGoblin : OdinNPC
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
		public override void SecondaryInteract (Humanoid user)
		{

		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			n += string.Format("\n<color=green><b>Credits:{0}</b></color>", OdinData.Credits);
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