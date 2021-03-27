using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HarmonyLib;
//opt:Create base class for task target//notice Use Invoke
namespace OdinPlus
{
	public class HuntTarget : MonoBehaviour
	{
		#region var
		private ZNetView m_nview;
		public string ID = "";
		public int Level;
		public int Key;
		private Transform m_task;
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

			if (ID != "")
			{
				m_nview.GetZDO().Set("TaskID", ID);
				m_nview.GetZDO().Set("HuntLevel", Level);
				m_nview.GetZDO().Set("HuntKey", Key);
				Tweakers.ValSpawn("vfx_GodExplosion", transform.position);
			}
			else
			{
				ID = m_nview.GetZDO().GetString("TaskID");
				Level = m_nview.GetZDO().GetInt("HuntLevel");
				Key = m_nview.GetZDO().GetInt("HuntKey");
			}
			m_mai.SetPatrolPoint();
			Traverse.Create(m_hum).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(Level).Key);
			CreateDrop();
		}
		private void Update()
		{
			if (!OdinPlus.isLoaded)
			{
				return;
			}
			m_task = TaskManager.Root.transform.Find("Task" + ID);
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
			Tweakers.ValSpawn("vfx_GodExplosion", transform.position);
			m_task.GetComponent<HuntTask>().Finish();
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
			go.name = name + "Hunt";
			go.AddComponent<HuntTarget>();
			go.GetComponent<Humanoid>().m_name+=" $op_hunt_target";
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
			d.m_levelMultiplier=false;
			d.m_prefab = ZNetScene.instance.GetPrefab("OdinLegacy");
			m_cDrop.m_drops = new List<CharacterDrop.Drop>();
			Traverse.Create(m_cDrop).Field<bool>("m_dropsEnabled").Value = true;
			m_cDrop.m_drops.Add(d);
		}
		#endregion Tool

	}

}