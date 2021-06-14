using System;
using UnityEngine;

namespace OdinPlus
{

	public class HumanNPC : OdinNPC, Hoverable, Interactable, OdinInteractable
	{
		#region var

		#region ref
		protected ZNetView m_nview;
		protected VisEquipment m_vis;
		protected Animator m_ani;
		protected Humanoid m_hum;
		protected MonsterAI monsterAI;
		#endregion ref
		#region Interal
		public string[] ChoiceList = { "$op_talk" };
		private int index;
		private string currentChoice = "";
		#endregion Interal
		#endregion var
		protected virtual void Awake()
		{

			monsterAI = GetComponent<MonsterAI>();
			m_talker = gameObject;
			m_nview = GetComponent<ZNetView>();
			m_ani = GetComponentInChildren<Animator>();
			m_hum = GetComponent<Humanoid>();
			m_vis = GetComponent<VisEquipment>();
			//RemoveUnusedComp();
			currentChoice = ChoiceList[index];
		}

		private void RemoveUnusedComp()
		{
			foreach (var comp in gameObject.GetComponents<Component>())
			{
				if (!(comp is Transform) && !(comp is HumanNPC) && !(comp is CapsuleCollider) && !(comp is ZNetView) && !(comp is VisEquipment) && !(comp is MonsterAI) && !(comp is Humanoid))
				{
					DestroyImmediate(comp);
				}
			}
		}
		public override void Say(string text)
		{
			
			Say(text, "emote_wave");
		}
		public void Say(string text, string emote)
		{
			if (m_hum.m_faction != Character.Faction.Players)
			{
				return;
			}
			text = Localization.instance.Localize(text);
			var tname = Localization.instance.Localize(m_name);
			Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
			m_ani.SetTrigger(emote);
		}
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			Invoke("Choice" + index, 0f);
			return true;
		}
		public virtual void Choice0()
		{
			Say("Greeting");
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
			if (m_hum.m_faction != Character.Faction.Players)
			{
				return "";
			}
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			n += "\n[<color=yellow><b>$KEY_Use</b></color>]" + currentChoice;
			n += "\n[<color=yellow><b>1-8</b></color>]$op_offer";
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>$op_switch", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			return Localization.instance.Localize(n);
		}
		public override string GetHoverName()
		{
			return Localization.instance.Localize(m_name);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}

		public void ChangeFaction(Character target)
		{
			m_hum.m_faction = Character.Faction.PlainsMonsters;
		}
		public void ChangeFaction(Character.Faction f)
		{
			m_hum.m_faction = f;
		}
		#region Debug

		#endregion Debug
	}
}