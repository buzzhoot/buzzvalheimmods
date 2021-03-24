using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{

	public class LegacyChest : MonoBehaviour
	{
		private Container m_container;
		private ZNetView m_nview;
		public string ID = "";
		private Transform m_task;
		private void Start()
		{
			m_nview = gameObject.GetComponent<ZNetView>();
			m_container = gameObject.GetComponent<Container>();
			if (ID != "")
			{
				m_nview.GetZDO().Set("TaskID", ID);
			}
			else
			{
				ID = m_nview.GetZDO().GetString("TaskID");
			}
		}
		private void Update()
		{
			if (!OdinPlus.isLoaded)
			{
				return;
			}
			m_task = TaskManager.Root.transform.Find("Task" + ID);
			if (m_task == null)//Decide whether the task is given up
			{
				DBG.blogInfo("Cant find task,Destroy"+ID);
				m_container.GetInventory().RemoveAll();
				ZNetScene.instance.Destroy(gameObject);
				return;
			}
			if (m_container.GetInventory() == null)
			{
				return;
			}
			if (m_container.GetInventory().NrOfItems() == 0)
			{
				DBG.blogInfo("Task Finish,Destroy");
				ZNetScene.instance.Destroy(gameObject);
				Instantiate(NpcManager.RavenPrefab.GetComponent<Raven>().m_despawnEffect.m_effectPrefabs[0].m_prefab,gameObject.transform.position,Quaternion.identity);
				m_task.GetComponent<OdinTask>().Finish();
				m_task.GetComponent<OdinTask>().Clear();
			}
		}
	}
}