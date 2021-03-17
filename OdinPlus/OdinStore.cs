using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OdinPlus
{
	class OdinStore : MonoBehaviour, Hoverable, Interactable
	{
		#region Var
		public string TraderName;

		#endregion,
		#region Mono
		public void Awake()
		{

		}
		#endregion
		#region Features

		#endregion
		#region Valheim
		public string GetHoverText()
		{
			string n = string.Format("\n<color=blue><b>{0}</b></color>", TraderName);
			string u = "\n[<color=yellow><b>$KEY_Use</b></color>] $odin_buy";
			return Localization.instance.Localize(n + u);
		}
		public string GetHoverName()
		{
			//trans
			return TraderName;
		}
		public bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			Say("Want something magic,Warrior?");
			return true;
		}
		public bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}
		#endregion
		#region Tool
		public void Say(string text)
		{
			Chat.instance.SetNpcText(gameObject, Vector3.up * 1.5f, 60f, 5, TraderName, text, false);
		}
		#endregion
	}
}
