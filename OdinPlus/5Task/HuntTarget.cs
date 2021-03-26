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
			m_chrct.m_onDeath = (Action)Delegate.Combine(new Action(this.OnDeath), m_chrct.m_onDeath);
			m_hum = gameObject.GetComponent<Humanoid>();
			Traverse.Create(m_hum).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(Level).Key);
		}
		private void Start()
		{

			if (ID != "")
			{
				m_nview.GetZDO().Set("TaskID", ID);
				Tweakers.ValSpawn("vfx_GodExplosion", transform.position);
			}
			else
			{
				ID = m_nview.GetZDO().GetString("TaskID");
			}
			m_mai.SetPatrolPoint();
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
				m_cDrop.m_dropsEnabled = false;
				ZNetScene.instance.Destroy(gameObject);
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
		public void Setup(int Key, int lvl)
		{
			Level=lvl;
			transform.SetParent(OdinPlus.PrefabParent.transform);
			m_chrct.SetLevel(Mathf.Clamp(Key, 2, 5));
			m_chrct.m_health *= (0.5f * Level + 1);
			m_hum.m_faction=Character.Faction.Boss;
			Traverse.Create(m_hum).Field<SEMan>("m_seman").Value.AddStatusEffect(OdinSE.MonsterSEList.ElementAt(Level).Key);
			transform.SetParent(OdinPlus.PrefabParent.transform.parent.parent);//opt
		}
		public static GameObject CreateMonster(string name)
		{
			var go = Instantiate(ZNetScene.instance.GetPrefab(name),OdinPlus.PrefabParent.transform);
			go.AddComponent<HuntTarget>();
			return go;
		}
		#endregion Tool

	}

}