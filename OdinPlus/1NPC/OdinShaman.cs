using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	class OdinShaman : OdinNPC
	{
		public static Dictionary<string, string> ItemList = new Dictionary<string, string>();
		public static Dictionary<string, string> ValueList = new Dictionary<string, string>();
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
		public override void SecondaryInteract (Humanoid user)
		{

		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=blue><b>{0}</b></color>", m_name);
			//n += string.Format("\n<color=green><b>Score:{0}</b></color>", OdinScore.score);
			n += "\n[<color=yellow><b>1-8</b></color>]Offer your Trophies";
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>$odin_shaman_use", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
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