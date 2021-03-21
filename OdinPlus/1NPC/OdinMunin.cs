using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	class OdinMunin : OdinNPC
	{
		private void Awake()
		{
			this.m_name = "$odin.munin";
			this.m_talker = this.gameObject;
		}
		private void Start()
		{
			gameObject.transform.Rotate(0, -45f, 0);
		}
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			return true;
		}
		public override void SecondaryInteract(Humanoid user)
		{

		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=blue><b>{0}</b></color>", m_name);
			n += string.Format("\n<color=green><b>Score:{0}</b></color>", OdinScore.score);
			n +=  "\n[<color=yellow><b>$KEY_Use</b></color>] $odin_munin_use";
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>$odin_munin_2use", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
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