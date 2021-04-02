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

	public class HumanNPC : OdinNPC, Hoverable, Interactable, OdinInteractable
	{
		#region var

		#region Visuals
		public static string[] NPCnames = { "$op_npc_name1", "$op_npc_name2", "$op_npc_name3", "$op_npc_name4", "$op_npc_name5", "$op_npc_name6", "$op_npc_name7", "$op_npc_name8", "$op_npc_name9", "$op_npc_name10", "$op_npc_name11", "$op_npc_name12", "$op_npc_name13", "$op_npc_name14", "$op_npc_name15", "$op_npc_name16", "$op_npc_name17", "$op_npc_name18", "$op_npc_name19", "$op_npc_name20", "$op_npc_name21", "$op_npc_name22", "$op_npc_name23", "$op_npc_name24", "$op_npc_name25", "$op_npc_name26", "$op_npc_name27", "$op_npc_name28", "$op_npc_name29", "$op_npc_name30", "$op_npc_name31", "$op_npc_name32", "$op_npc_name33", "$op_npc_name34", "$op_npc_name35", "$op_npc_name36", "$op_npc_name37", "$op_npc_name38", "$op_npc_name39", "$op_npc_name40", "$op_npc_name41", "$op_npc_name42", "$op_npc_name43", "$op_npc_name44", "$op_npc_name45", "$op_npc_name46", "$op_npc_name47", "$op_npc_name48", "$op_npc_name49", "$op_npc_name50" };
		public string[] m_beardItem = { "Beard2", "Beard3", "Beard4", "Beard5", "Beard6", "Beard7", "Beard8", "Beard9", "Beard10" };
		public string[] m_hairItem = { "Hair1", "Hair2", "Hair3", "Hair4", "Hair5", "Hair6", "Hair7", "Hair8", "Hair9", "Hair10" };
		public string[] m_helmetItem = { "" };
		public string[] m_shoulderItem = { "" };
		public string[] m_leftItem = { "" };
		public string[] m_rightItem = { "" };
		public string[] m_chestItem = { "" };
		public string[] m_legItem = { "" };
		#endregion Visuals	
		#region ref
		protected ZNetView m_nview;
		protected VisEquipment m_vis;
		protected Animator m_ani;
		#endregion ref
		#region Interal
		public string[] ChoiceList = { "$op_talk" };
		private int index = 0;
		private string currentChoice = "";
		#endregion Interal
		#endregion var


		protected virtual void Awake()
		{
			m_talker = gameObject;
			m_nview = GetComponent<ZNetView>();
			m_ani = GetComponentInChildren<Animator>();
			Util.seed = (int)((gameObject.transform.position.x + gameObject.transform.position.y) * 1000);
			SetName();
			SetupVisual();
			RemoveUnusedComp();
			currentChoice = ChoiceList[index];
		}
		protected virtual void SetName()
		{
			m_name = m_nview.GetZDO().GetString("op_npcname", NPCnames.GetRandomElement());
		}
		protected virtual void SetupVisual()
		{
			SetItem("BeardItem", m_beardItem);
			SetItem("HairItem", m_hairItem);
			if (m_helmetItem != new string[] { "" })
			{
				SetItem("HelmetItem", m_helmetItem);
			}
			SetItem("ChestItem", m_chestItem);
			SetItem("ShoulderItem", m_shoulderItem);
			SetItem("LegItem", m_legItem);
			Traverse.Create(m_vis).Field<int>("m_modelIndex").Value = m_nview.GetZDO().GetInt("ModelIndex", 2.RollDices());
			Traverse.Create(m_vis).Field<Vector3>("m_hairColor").Value = m_nview.GetZDO().GetVec3("HairColor", new Vector3(1f.RollDices(), 1f.RollDices(), 1f.RollDices()));
			Traverse.Create(m_vis).Field<Vector3>("m_skinColor").Value = m_nview.GetZDO().GetVec3("SkinColor", new Vector3(1f.RollDices(), 1f.RollDices(), 1f.RollDices()));
			//m_vis.m_skinColor = new Vector3(1f.RollDices(), 1f.RollDices(), 1);
		}
		protected void SetItem(string slot, string[] items)
		{
			m_nview.GetZDO().Set(slot, items.GetRandomElement().GetStableHashCode());
		}

		private void RemoveUnusedComp()
		{
			foreach (var comp in gameObject.GetComponents<UnityEngine.Component>())
			{
				if (!(comp is Transform) && !(comp is HumanNPC) && !(comp is CapsuleCollider) && !(comp is ZNetView) && !(comp is VisEquipment))
				{
					DestroyImmediate(comp);
				}
			}
		}
		public override void Say(string text)
		{
			text = Localization.instance.Localize(text);
			var tname = Localization.instance.Localize(m_name);
			Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
			m_ani.SetTrigger("emote_wave");
		}
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			Invoke("Choice"+index.ToString(),0f);
			return true;
		}
		public virtual void Choice0()
		{
			Say("Find me some <color=lightblue><b>BlueBerry</b></color> then i will tell you where to go");
		}
		public override void SecondaryInteract(Humanoid user)
		{
			index += 1;
			if (index + 1 > ChoiceList.Length)
			{
				index = 0;
			}
			currentChoice = ChoiceList[index];
		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			n += string.Format("\n<color=green><b>Credits:{0}</b></color>", OdinData.Credits);
			n += "\n[<color=yellow><b>$KEY_Use</b></color>]" + currentChoice;
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