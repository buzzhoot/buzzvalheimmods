using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	public class LegacyChest : MonoBehaviour
	{

		private ZNetView m_nview;
		public string ID = "";
		public bool Placing = false;
		public bool isPublic = false;
		public string OwenerID = "";
		private Transform m_task;
		private Container m_container;
		private void Start()
		{
			if (gameObject.transform.position.y > 4000)
			{
				DestroyImmediate(this.GetComponent<StaticPhysics>());
			}
			m_nview = gameObject.GetComponent<ZNetView>();
			m_container = gameObject.GetComponent<Container>();
			if (Placing)
			{
				m_nview.GetZDO().Set("TaskID", ID);
				return;
			}
			else
			{
				ID = m_nview.GetZDO().GetString("TaskID");
			}
			if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
			{
				//Destroy(gameObject);
			}


		}
		private void Update()
		{
			ID = m_nview.GetZDO().GetString("TaskID");
			if (m_container.GetInventory() == null)
			{
				DBG.blogWarning("Cant find inv");
				return;
			}
			if (m_container.GetInventory().NrOfItems() == 0)
			{
				ZRoutedRpc.instance.InvokeRoutedRPC("RPC_FinishTask", new object[] { ID });
				Instantiate(NpcManager.RavenPrefab.GetComponent<Raven>().m_despawnEffect.m_effectPrefabs[0].m_prefab, gameObject.transform.position, Quaternion.identity);
				ZNetScene.instance.Destroy(gameObject);
			}
		}
		public void OnOpen(Humanoid user)
		{
			if (user.m_name==OwenerID)
			{
				//+ finish
				return;
			}
			//+Stoled

		}
	}
}