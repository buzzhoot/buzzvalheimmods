using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
namespace OdinPlus
{

	public class RandomNPC : OdinNPC, Hoverable, Interactable, OdinInteractable
	{

		private void Start()
		{
			var prefab = this.gameObject;
			ZNetView znv = prefab.GetComponent<ZNetView>();
			ZDO zdo = prefab.GetComponent<ZNetView>().GetZDO();
			
			foreach (var comp in gameObject.GetComponents<Component>())
			{
				if (!(comp is Transform) && !(comp is RandomNPC) && !(comp is CapsuleCollider))
				{
					DestroyImmediate(comp);
				}
			}
			var a = Traverse.Create(ZNetScene.instance).Field<Dictionary<ZDO, ZNetView>>("m_instances").Value;
			a.Remove(zdo);
			ZDOMan.instance.DestroyZDO(zdo);
			m_name = "Ian Curtis";
			m_talker = gameObject;
		}
		public override void Say(string text)
		{
			text = Localization.instance.Localize(text);
			var tname = Localization.instance.Localize(m_name);
			Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
		}
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			Say("Find me some <color=lightblue><b>BlueBerry</b></color> then i will tell you where to go");
			return true;
		}
		public override void SecondaryInteract(Humanoid user)
		{

		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			n += string.Format("\n<color=green><b>Credits:{0}</b></color>", OdinData.Credits);
			n += "\n[<color=yellow><b>1-8</b></color>]$op_offer";
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>$op_switch", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
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