using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
	class OdinShaman : OdinNPC, Hoverable, Interactable, OdinInteractable
	{
		//private static bool isInit = false;
		public Dictionary<string, GoodsDate> GoodsList = new Dictionary<string, GoodsDate>{
		{"TrophyFrostTroll", new GoodsDate { Good = "ScrollTroll", Value = 3 }},
		{"TrophyWolf", new GoodsDate { Good = "ScrollWolf", Value = 3 }}
		};
		public struct GoodsDate
		{
			public string Good;
			public int Value;


		}
		public bool crealvl = true;
		#region  Mono
		private void Awake()
		{
			m_name = "$op_shaman";
			m_talker = gameObject;
		}

		#endregion  Mono
		private void Start()
		{
			var prefab = gameObject;
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
			foreach (var comp in gameObject.GetComponents<Component>())
			{
				if (!(comp is Transform) && !(comp is OdinShaman)&& !(comp is CapsuleCollider))
				{
					DestroyImmediate(comp);
				}
			}
			var a = Traverse.Create(ZNetScene.instance).Field<Dictionary<ZDO, ZNetView>>("m_instances").Value;
			a.Remove(zdo);
			ZDOMan.instance.DestroyZDO(zdo);
			prefab.gameObject.transform.Rotate(0, 30f, 0);
		}
		//remvoe
		bool IsAssemblyExists(string assemblyName)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (assembly.FullName.StartsWith(assemblyName))
					return true;
			}
			return false;
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
			//n += string.Format("\n<color=green><b>Credits:{0}</b></color>", OdinData.Credits);
			//n += "\n[<color=yellow><b>$KEY_Use</b></color>] $op_buy";
			n += "\n[<color=yellow><b>1-8</b></color>]$op_shaman_offer";
			//n += String.Format("\n<color=yellow><b>[{0}]</b></color>$op_shaman_use", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			return Localization.instance.Localize(n);
		}
		public override string GetHoverName()
		{
			return Localization.instance.Localize(m_name);
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
					DBG.InfoCT("$op_inventory_full");
					return true;
				}
				Say("$op_shaman_notenough");
				return true;
			}
			Say("$op_shaman_no");
			return true;
		}
	}
}