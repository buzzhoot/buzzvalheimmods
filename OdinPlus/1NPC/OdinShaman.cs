using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	class OdinShaman : OdinNPC, Hoverable, Interactable, OdinInteractable
	{
		//private static bool isInit = false;
		public  Dictionary<string, GoodsDate> GoodsList = new Dictionary<string, GoodsDate>{
		{"TrophyFrostTroll", new GoodsDate { Good = "ScrolTroll", Value = 3 }},
		{"TrophyWolf", new GoodsDate { Good = "ScrollWolf", Value = 3 }}
		};
		public struct GoodsDate
		{
			public string Good;
			public int Value;

		}
		#region  Mono
		private void Awake()
		{
			m_name = "$odin_shaman";
			m_talker = this.gameObject;
		}

		#endregion  Mono
		private void Start()
		{
			var prefab = this.gameObject;
			ZNetView znv = prefab.GetComponent<ZNetView>();
			ZDO zdo = prefab.GetComponent<ZNetView>().GetZDO();
			DestroyImmediate(prefab.GetComponent<ZNetView>());
			DestroyImmediate(prefab.GetComponent<ZSyncAnimation>());
			DestroyImmediate(prefab.GetComponent<ZSyncTransform>());
			DestroyImmediate(prefab.GetComponent<MonsterAI>());
			DestroyImmediate(prefab.GetComponent<VisEquipment>());
			DestroyImmediate(prefab.GetComponent<CharacterDrop>());
			DestroyImmediate(prefab.GetComponent<Humanoid>());
			DestroyImmediate(prefab.GetComponent<FootStep>());
			DestroyImmediate(prefab.GetComponent<Rigidbody>());
			var a = Traverse.Create(ZNetScene.instance).Field<Dictionary<ZDO, ZNetView>>("m_instances").Value;
			a.Remove(zdo);
			ZDOMan.instance.DestroyZDO(zdo);
			prefab.gameObject.transform.Rotate(0, 30f, 0);
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
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			//n += string.Format("\n<color=green><b>Score:{0}</b></color>", OdinData.score);
			n += "\n[<color=yellow><b>$KEY_Use</b></color>] $odin_buy(Devloping Not Working)";
			n += "\n[<color=yellow><b>1-8</b></color>]Offer your Trophies(3,Wolf or Troll)";
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>$odin_shaman_use(Devloping Not Working)", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			return Localization.instance.Localize(n);
		}
		public override string GetHoverName()
		{
			return Localization.instance.Localize(this.m_name);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			var name = item.m_dropPrefab.name;
			if (GoodsList.ContainsKey(name))
			{
				var gd = GoodsList[name];
				if (item.m_stack >= gd.Value)
				{
					var goodItemData = OdinItem.GetItemData(gd.Good);
					if (user.GetInventory().AddItem(goodItemData))
					{
						user.GetInventory().RemoveItem(item, gd.Value);
						Say(goodItemData.m_shared.m_description);
						return true;
					}
					DBG.InfoCT("$odin_inventory_full");
					return true;
				}
				Say("$odin_shaman_cantbuy");
				return true;
			}
			Say("Hmm that's something new,can't take that right now");
			return true;
		}
	}
}