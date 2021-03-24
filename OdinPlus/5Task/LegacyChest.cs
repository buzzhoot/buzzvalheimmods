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
		private GameObject m_task;
		private void Awake()
		{
			m_nview = gameObject.GetComponent<ZNetView>();
			m_container = gameObject.GetComponent<Container>();
			if (ID != null)
			{
				m_nview.GetZDO().Set("TaskID", ID);
			}
			else
			{
				ID = m_nview.GetZDO().GetString("TaskID");
			}
		}
		private void Start()
		{
			m_task = TaskManager.Root.FindObject("Task" + ID);
		}
		private void Update()
		{
			if (m_task == null)
			{
				m_container.GetInventory().RemoveAll();
				ZNetScene.instance.Destroy(gameObject);
			}
			if (m_container.GetInventory().NrOfItems() == 0)
			{
				ZNetScene.instance.Destroy(gameObject);
				m_task.GetComponent<OdinTask>().Finish();
			}
		}
	}
}