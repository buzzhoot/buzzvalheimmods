using UnityEngine;

namespace OdinPlus
{
  public class LegacyChest : MonoBehaviour
	{

		private ZNetView m_nview;
		public bool Placing;
		public bool m_sphy;
		//upd maybe make this private box??public bool isPublic = false;
		public string m_id = "";
		public string m_ownerName = "";
		private Container m_container;
		private void Awake()
		{
			m_nview = gameObject.GetComponent<ZNetView>();
			m_container = gameObject.GetComponent<Container>();
			var zdo = m_nview.GetZDO();
			if (Placing)
			{

				zdo.Set("QuestID", m_id);
				zdo.Set("QuestSphy", m_sphy);
				zdo.Set("QuestOwener", m_ownerName);
				return;
			}

      m_id = zdo.GetString("QuestID", "public");
      m_sphy = zdo.GetBool("QuestSphy", true);
      m_ownerName = zdo.GetString("QuestOwener", "public");
      if (!m_sphy)
			{
				DestroyImmediate(GetComponent<StaticPhysics>());
			}
			/* 			if (ZNet.instance.IsServer() && ZNet.instance.IsDedicated())
						{
							Destroy(gameObject);
						} */
		}
		private void Update()
		{
			if (m_container.GetInventory() == null)
			{
				DBG.blogWarning("Cant find inv");
				return;
			}
			if (m_container.GetInventory().NrOfItems() == 0)
			{

				Instantiate(NpcManager.RavenPrefab.GetComponent<Raven>().m_despawnEffect.m_effectPrefabs[0].m_prefab, gameObject.transform.position, Quaternion.identity);
				ZNetScene.instance.Destroy(gameObject);
			}
		}
		//HELP how to make a delegate here?//notice
		public void OnOpen(Humanoid user, bool hold)
		{
			if (hold)
			{
				return;
			}
			if (user.GetHoverName() == m_ownerName)
			{
				var quest = QuestManager.instance.GetQuest(m_id);
				if (quest != null)
				{
					//upd should select in base? yes!!!!!!!!!
					QuestProcessor.Create(quest).Finish();
					return;
				}
				//upd giveup without destroy?
			}
			string n = string.Format("Hey you found the chest belong to <color=yellow><b>{0}</b></color>", m_ownerName);//trans
			DBG.InfoCT(n);

		}
		public void WatchMe()
		{
			GameCamera.instance.transform.localPosition = transform.position + Vector3.forward * 1;
		}

		#region Static
		public static GameObject Place(Vector3 pos, Quaternion rot, float p_range, string p_id, string p_owner, int p_key, bool sphy = true)
		{
			DestroyCTN(pos,p_range);
			return Place(pos, p_id, p_owner, p_key, rot, sphy);

		}
		public static void DestroyCTN(Vector3 pos,float p_range)
		{
			Collider[] array = Physics.OverlapBox(pos, Vector3.one * p_range);
			foreach (var col in array)
			{
				Container ctn = col.GetComponent<Container>();
				Container ctn2 = col.transform?.parent?.GetComponent<Container>();
				Container ctn3 = col.transform?.parent?.parent?.GetComponent<Container>();
				if (ctn != null)
				{
					ctn.gameObject.GetComponent<ZNetView>().Destroy();
					DBG.blogWarning("Find destroyable ctn");
					return;
				}
				if (ctn2 != null)
				{
					col.transform?.parent?.GetComponent<ZNetView>().Destroy();
					DBG.blogWarning("Find destroyable ctn parent");
					return;
				}
				if (ctn3 != null)
				{
					ctn3?.GetComponent<ZNetView>().Destroy();
					DBG.blogWarning("Find destroyable ctn grandparent");
					return;
				}
			}
		}
		public static GameObject Place(Vector3 pos, string p_id, string p_owner, int p_key, bool sphy = true)
		{
			return Place(pos, p_id, p_owner, p_key, Quaternion.identity, sphy);
		}
		public static GameObject Place(Vector3 pos, string p_id, string p_owner, int p_key, Quaternion rot, bool sphy = true)
		{
			GameObject chest;
			chest = Instantiate(ZNetScene.instance.GetPrefab("LegacyChest" + (p_key + 1)), pos, rot, OdinPlus.PrefabParent.transform);

			var lc = chest.GetComponent<LegacyChest>();
			lc.Placing = true;
			lc.m_id = p_id;
			lc.m_sphy = sphy;
			lc.m_ownerName = p_owner;

			if (!sphy)
			{
				DestroyImmediate(chest.GetComponent<StaticPhysics>());
			}
			DBG.blogWarning("Placed Chest at " + pos);
			chest.transform.SetParent(OdinPlus.Root.transform);
			return chest;
		}
		#endregion Static
	}
}