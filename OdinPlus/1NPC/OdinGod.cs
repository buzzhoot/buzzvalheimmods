using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//||X||Sell Value Don't Resolve Here!!!
namespace OdinPlus
{
	public class OdinGod : OdinNPC, Hoverable, Interactable, OdinInteractable
	{
		#region Var
		public static OdinGod m_instance;
		private List<string> slist = new List<string>();
		private List<Skills.SkillType> stlist = new List<Skills.SkillType>();
		private string cskill;
		private int cskillIndex;
		#endregion
		#region util
		private Vector3 FindSpawnPoint()
		{
			var a = Random.Range(10, 10);
			var b = Random.Range(10, 10);
			var c = ZoneSystem.instance.GetGroundHeight(new Vector3(a, 500, b));
			ZoneSystem.LocationInstance locationInstance;
			if (ZoneSystem.instance.FindClosestLocation("StartTemple", Vector3.zero, out locationInstance))
			{
				var p = locationInstance.m_position + new Vector3(-6, 0.2f, -8);
				return p;
			}
			DBG.blogWarning("Cant Find a point to Spawn Odin use /odin respawn");//notice
			return new Vector3(a, c, b);
		}
		private string randomName()
		{
			Random.InitState(Mathf.FloorToInt(Time.realtimeSinceStartup));
			var l = OdinData.ItemSellValue;
			int i = Random.Range(0, l.Count - 1);
			return l.ElementAt(i).Key.GetTransName();
		}
		public static bool IsInstantiated()
		{
			return m_instance == null;
		}
		public void RestTerrian()
		{
			//Terrain.ResetTerrain(this.transform.position, 10);
		}
		#endregion
		#region Mono

		private void Awake()
		{
			m_instance = this;
			Summon();
			m_head = gameObject.transform.Find("visual/Armature/Hips/Spine0/Spine1/Spine2/Head");
			m_name = "$op_god";
			m_talker = gameObject;
			InvokeRepeating("requestOidnPosition", 1, 3);
			DBG.blogInfo("Client start to Calling Request Odin Location");
		}
		private void requestOidnPosition()
		{
			if (NpcManager.Root.transform.position == Vector3.zero)
			{
				LocationManager.GetStartPos();
				return;
			}
			DBG.blogInfo("Client Stop Request odin position");
			CancelInvoke("requestOidnPosition");
		}
		private void Start()
		{
			Debug.LogWarning(gameObject.transform.parent.rotation);
			gameObject.transform.parent.Rotate(0, 42, 0);
			Debug.LogWarning(gameObject.transform.parent.rotation);
		}
		private void OnDestroy()
		{
			//RestTerrian();
			if (m_instance == this)
			{
				m_instance = null;
			}
		}
		#endregion
		#region Tool
		public bool Summon()
		{
			//this.transform.parent.localPosition = FindSpawnPoint();
			ReadSkill();
			return true;
		}

		#endregion
		#region valheim
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			if (!OdinData.RemoveCredits(Plugin.RaiseCost))
			{
				Say("$op_god_nocrd");
				return false;
			}

			user.GetSkills().RaiseSkill(stlist[cskillIndex], Plugin.RaiseFactor);
			Say("$op_raise");
			return true;
		}
		public override void SecondaryInteract(Humanoid user)
		{
			SwitchSkill();
		}
		public override string GetHoverText()
		{
			string n = "<color=lightblue><b>ODIN</b></color>";
			string s = string.Format("\n<color=lightblue><b>$op_crd:{0}</b></color>", OdinData.Credits);
			string a = string.Format("\n[<color=yellow><b>$KEY_Use</b></color>] $op_use[<color=green><b>{0}</b></color>]", cskill);
			string b = "\n[<color=yellow><b>1-8</b></color>]$op_offer";
			b += String.Format("\n<color=yellow><b>[{0}]</b></color>$op_switch", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			return Localization.instance.Localize(n + s + a + b);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)//trans
		{
			var name = item.m_dropPrefab.name;
			int value = 1;
			if (!OdinData.ItemSellValue.ContainsKey(name))
			{
				Say("$op_god_randomitem " + randomName());
				return false;
			}
			value = OdinData.ItemSellValue[name];
			OdinData.AddCredits(value * item.m_stack * item.m_quality, m_head);
			user.GetInventory().RemoveItem(item.m_shared.m_name, item.m_stack);
			Say("$op_god_takeoffer");
			return true;
		}
		#endregion
		#region feature
		private void ReadSkill()
		{
			slist.Clear();
			stlist.Clear();
			foreach (object obj in Enum.GetValues(typeof(Skills.SkillType)))
			{
				Skills.SkillType skillType = (Skills.SkillType)obj;
				var s = skillType.ToString();
				if (s != "None" && s != "FrostMagic" && s != "All" && s != "FireMagic")
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
