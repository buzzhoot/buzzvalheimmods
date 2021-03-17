using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
//||X||Sell Value Don't Resolve Here!!!
namespace OdinPlus
{
	public class OdinGod : MonoBehaviour, Hoverable, Interactable
	{
		#region Var
		public static OdinGod m_instance;
		private List<string> slist = new List<string>();
		private List<Skills.SkillType> stlist = new List<Skills.SkillType>();
		private string cskill;
		private int cskillIndex = 0;
		private Transform m_head;
		#endregion
		#region util
		private Vector3 FindSpawnPoint()
		{
			var a = UnityEngine.Random.Range(10, 10);
			var b = UnityEngine.Random.Range(10, 10);
			var c = ZoneSystem.instance.GetGroundHeight(new Vector3(a, 500, b));
			ZoneSystem.LocationInstance locationInstance;
			if (ZoneSystem.instance.FindClosestLocation("StartTemple", Vector3.zero, out locationInstance))
			{
				var p = locationInstance.m_position + new Vector3(-6, 0.2f, -8);
				return p;
			}
			DBG.blogWarning("Cant Find a point to Spawn Odin use /odin respawn");//notice
			return new Vector3(a,c,b);
		}
		private string randomName()
		{
			UnityEngine.Random.InitState(Mathf.FloorToInt(Time.realtimeSinceStartup));
			var l = OdinScore.ItemSellValue;
			int i = UnityEngine.Random.Range(0, l.Count);
			return l.ElementAt(i).Key;
		}
		public static bool IsInstantiated()
		{
			return m_instance == null;
		}
		public void RestTerrian()
		{
			Terrain.ResetTerrain(this.transform.position, 10);
		}
		#endregion
		#region Mono
		private void OnDestroy()
		{
			RestTerrian();
			if (m_instance == this)
			{
				m_instance = null;
			}
		}
		private void Awake()
		{
			m_instance = this;
			Summon();
			m_head = this.gameObject.transform.Find("visual/Armature/Hips/Spine0/Spine1/Spine2/Head");
		}
		#endregion
		#region Tool
		public static void Say(string text)
		{
			Chat.instance.SetNpcText(m_instance.gameObject, Vector3.up * 1.5f, 60f, 5, "Odin", text, false);
		}
		public bool Summon()
		{
			this.transform.parent.localPosition = FindSpawnPoint();
			Terrain.Flatten(3.5f, 3.5f, this.transform);
			Terrain.RemoveFlora(4f, this.transform.position);
			ReadSkill();
			return true;
		}
		#endregion
		#region valheim
		public bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			//Say("Greeting,Warrior?");
			//user.GetSkills().CheatRaiseSkill(cskill,1);
			if (!OdinScore.remove(15))//---------------------cfg
			{
				Say("Hard work is the only way to get reward");
				return false;
			}
			user.GetSkills().RaiseSkill(stlist[cskillIndex], 50f);//---------------------cfg
			Say("I made you stronger,warrior");
			return true;
		}
		public string GetHoverText()
		{
			string n = "<color=blue><b>ODIN</b></color>";
			string s = string.Format("\n<color=green><b>Score:{0}</b></color>", OdinScore.score);
			string a = string.Format("\n[<color=yellow><b>$KEY_Use</b></color>] $odin_use[<color=green><b>{0}</b></color>]", cskill);
			string b = "\n[<color=yellow><b>1-8</b></color>]Offer your gifts";
			string c = "\n[<color=yellow><b>F</b></color>]Switch Skill";
			return Localization.instance.Localize(n + s + a + b + c);
		}
		public string GetHoverName()
		{
			return "odin";
		}
		public bool UseItem(Humanoid user, ItemDrop.ItemData item)//trans
		{
			var name = item.m_dropPrefab.name;
			int value = 1;
			if (!OdinScore.ItemSellValue.ContainsKey(name))
			{
				Say("I need Something useful...like " + randomName());
				return false;
			}
			value = OdinScore.ItemSellValue[name];
			OdinScore.add(value * item.m_stack, m_head);
			user.GetInventory().RemoveItem(item.m_shared.m_name, item.m_stack);
			Say("Nice,bring back more");
			return true;
		}
		#endregion
		#region feature
		private void ReadSkill()
		{
			foreach (object obj in Enum.GetValues(typeof(Skills.SkillType)))
			{
				Skills.SkillType skillType = (Skills.SkillType)obj;
				var s = skillType.ToString();
				if (s != "None" && s != "" && s != "All")
				{
					slist.Add(skillType.ToString());
					stlist.Add(skillType);
				}
			}
			cskill = slist[cskillIndex];
		}
		public void SwitchSkill()
		{
			cskillIndex += 1;
			if (cskillIndex + 1 > slist.Count())
			{
				cskillIndex = 0;
			}
			cskill = slist[cskillIndex];
		}
		#endregion

	}
}
