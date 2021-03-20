using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	class OdinShaman : OdinNPC
	{
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
			string n = string.Format("<color=blue><b>{0}</b></color>", m_name);
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