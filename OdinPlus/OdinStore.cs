using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OdinPlus
{
	class OdinStore : Trader, Hoverable, Interactable
	{
		#region Var
		public GameObject m_talker;
		public string m_talkername;

		#endregion,
		#region Mono
		private void Awake()
		{
			var td = ZNetScene.instance.GetPrefab("Haldor").GetComponent<Trader>();
			m_talker = this.gameObject;
			m_randomStartTradeFX = td.m_randomStartTradeFX;
		}
		private new void Start() { }
		private new void Update() { }
		#endregion
		#region Features

		#endregion
		#region Valheim
		public new string GetHoverText()
		{
			string n = string.Format("\n<color=blue><b>{0}</b></color>", m_name);
			string u = "\n[<color=yellow><b>$KEY_Use</b></color>] $odin_buy";
			return Localization.instance.Localize(n + u);
		}
		public new string GetHoverName()
		{
			return m_name;
		}
		public new bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			Say("Want something magic,Warrior?");
			StoreGui.instance.Show(this);
			m_randomStartTradeFX.Create(m_talker.transform.position, Quaternion.identity, null, 1f);
			m_randomStartTradeFX.Create(this.transform.position, Quaternion.identity, null, 1f);
			//test.testdebg();
			return true;
		}
		public new bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}
		#endregion
		#region Tool
		private void Say(string text)
		{
			Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, m_talkername, text, false);
		}
		public static void TweakGui(StoreGui __instance, bool set)
		{
			var go = __instance.gameObject;
			var sell = go.transform.Find("SellPanel");
			var icon = go.transform.Find("coin icon").GetComponent<Image>();
			sell.gameObject.SetActive(!set);
			icon.sprite=!set?OdinPlus.CoinsIcon:OdinPlus.OdinCreditIcon;
		}

		#endregion
		#region Override
		private new void Say(List<string> texts, string trigger) { return; }
		private new void Say(string text, string trigger) { return; }
		private new void RandomTalk() { return; }
		private new void OnBought(TradeItem item) { return; }
		private new void OnSold() { return; }
		#endregion
	}
}
