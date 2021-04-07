using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.Text.RegularExpressions;

//opt:Create base class for task target//notice Use Invoke
namespace OdinPlus
{
	public class HuntTarget : MonoBehaviour
	{
		#region var
		private ZNetView m_nview;
		public string ID = "";
		public int Level=1;
		public int Key=1;
		public string m_ownerName = "";
		public bool Placing = false;
		private Character m_chrct;
		private Humanoid m_hum;
		private CharacterDrop m_cDrop;
		private MonsterAI m_mai;

		#endregion var

		#region Mono +Death
		private void Awake()
		{
			m_nview = gameObject.GetComponent<ZNetView>();
			m_chrct = gameObject.GetComponent<Character>();
			m_mai = gameObject.GetComponent<MonsterAI>();
			m_cDrop = GetComponent<CharacterDrop>();
			m_chrct.m_onDeath = (Action)Delegate.Combine(new Action(this.OnDeath), m_chrct.m_onDeath);
			m_hum = gameObject.GetComponent<Humanoid>();

		}
		private void Start()
		{
			var zdo = m_nview.GetZDO();
			if (ID != "")
			{

				zdo.Set("TaskID", ID);
				zdo.Set("HuntLevel", Level);
				zdo.Set("HuntKey", Key);
				zdo.Set("OwnerName", m_ownerName);
				Tweakers.ValSpawn("vfx_GodExplosion", transform.position);
			}
			else
			{
				ID = zdo.GetString("TaskID");
				Level = zdo.GetInt("HuntLevel");
				Key = zdo.GetInt("HuntKey");
				m_ownerName = zdo.GetString("OwnerName", "");
			}
			m_mai.SetPatrolPoint();
			Traverse.Create(m_hum).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(Level).Key);
		}
		private void Update()
		{
			if (ID=="")
			{
				return;
			}
			var m_task = QuestManager.instance.GetQuest(ID);
			if (m_task == null)
			{
				DBG.blogInfo("Cant find task,Destroy Hunt Target" + ID);
				Traverse.Create(m_cDrop).Field<bool>("m_dropsEnabled").Value = false;
				m_nview.Destroy();
				return;
			}
		}
		public void OnDeath()
		{
			if (Player.GetClosestPlayer(transform.position, 100).GetHoverName() == m_ownerName)
			{
				QuestManager.instance.GetQuest(ID).Finish();
			}
			else
			{
				if (m_ownerName == "")
				{

				}
				else
				{
					string n = string.Format("Hey you found the chest belong to <color=yellow><b>{0}</b></color>", m_ownerName);//trans
					DBG.InfoCT(n);
				}
			}
			Tweakers.ValSpawn("vfx_GodExplosion", transform.position);
			var r = Instantiate(ZNetScene.instance.GetPrefab("OdinLegacy"), transform.localPosition, Quaternion.identity);
			r.GetComponent<ItemDrop>().m_itemData.m_quality = Key;
			r.GetComponent<ItemDrop>().m_itemData.m_stack = Level;

		}

		#endregion Mono

		#region Tool
		public void Setup(int key, int lvl)
		{
			Level = lvl;
			Key = key;
			m_chrct.SetLevel(Mathf.Clamp(Level + 2, 2, 5));
			m_chrct.m_health *= (0.5f * Level + 1);
			m_hum.m_faction = Character.Faction.Boss;
		}
		public static GameObject CreateMonster(string name)
		{
			var go = Instantiate(ZNetScene.instance.GetPrefab(name), OdinPlus.PrefabParent.transform);
			name = Regex.Replace(name, @"[_]", "");
			go.name = name + "Hunt";
			go.AddComponent<HuntTarget>();
			go.GetComponent<Humanoid>().m_name += " $op_hunt_target";
			DestroyImmediate(go.GetComponent<CharacterDrop>());
			var fx = Instantiate(FxAssetManager.GetFxNN("GreenSmoke"), go.transform);
			fx.transform.position = go.FindObject("Spine2").transform.position;//opt Random smoke
			return go;
		}
		public void CreateDrop()
		{
			var d = new CharacterDrop.Drop();
			d.m_chance = 1;
			d.m_amountMax = Level + Key;
			d.m_amountMin = d.m_amountMax;
			d.m_levelMultiplier = false;
			d.m_prefab = ZNetScene.instance.GetPrefab("OdinLegacy");
			m_cDrop.m_drops = new List<CharacterDrop.Drop>();
			Traverse.Create(m_cDrop).Field<bool>("m_dropsEnabled").Value = true;
			m_cDrop.m_drops.Add(d);
		}
		public static void Place(Vector3 pos, string monster, string id,string _owner, int p_key, int p_lvl)
		{
			float y = 0;
			ZoneSystem.instance.FindFloor(pos, out y);
			pos = new Vector3(pos.x, y + 2, pos.z + 5);
			var Reward = Instantiate(ZNetScene.instance.GetPrefab(monster + "Hunt"), pos, Quaternion.identity);
			Reward.GetComponent<HuntTarget>().ID = id;
			Reward.GetComponent<HuntTarget>().m_ownerName=_owner;
			Reward.GetComponent<HuntTarget>().Setup(p_key, p_lvl);
			DBG.blogWarning("Placed Hunt " + monster + " at : " + Reward.transform.localPosition);
		}
		#endregion Tool

	}

}