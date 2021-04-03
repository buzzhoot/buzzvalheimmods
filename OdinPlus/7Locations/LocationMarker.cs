using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	public class LocationMarker : MonoBehaviour
	{
		#region Var
		public static Dictionary<string, LocationMarker> MarkList = new Dictionary<string, LocationMarker>();
		public static Dictionary<string, LocationMarker> DubList = new Dictionary<string, LocationMarker>();
		public static GameObject Prefab;

		#region Setting
		public string ID = "";
		public string owner = "";
		#endregion Setting
		#region Internal
		private ZNetView m_nview;
		private LocationProxy m_locationProxy;
		#endregion Internal
		#region Out
		private List<Container> m_container = new List<Container>();
		private Vector3 m_pos;

		#endregion Out

		#endregion Var
		#region Mono

		#endregion Mono
		private void Awake()
		{
			//add
			m_nview = GetComponent<ZNetView>();
			if (m_nview.GetZDO() == null)
			{
				DBG.blogWarning("Mark Report zdo null");
				return;
			}
			ID = ZoneSystem.instance.GetZone(transform.position).Pak();

			if (transform.GetComponentInParent<DungeonGenerator>())
			{
				var ctns = transform.parent.GetComponentsInChildren<Container>(true);
				if (ctns == null)
				{
					DBG.blogWarning("Can't Find Container");
				}
				else
				{
					DBG.blogWarning("Found ctn: " + ctns.Length);
				}
			}
			if (MarkList.ContainsKey(ID))
			{
				DubList.Add(ID, this);
			}
			else
			{
				MarkList.Add(ID, this);
			}
		}
		private void Start()
		{
			DBG.blogWarning("Start");
		}
		private void OnDestroy()
		{
			MarkList.Remove(ID);
		}
		public void Used()
		{
			m_nview.GetZDO().Set("Used", true);
		}

		#region static
		public static void CreatePrefab()
		{
			//GameObject go = new GameObject("LocMark");
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.transform.localScale = Vector3.one * 3;
			go.GetComponent<Renderer>().material.color = Color.red;
			go.name = ("LocMark");
			go.transform.SetParent(OdinPlus.PrefabParent.transform);
			go.AddComponent<LocationMarker>();
			var zv = go.AddComponent<ZNetView>();
			zv.m_persistent = true;
			zv.m_type = ZDO.ObjectType.Solid;
			PrefabManager.PrefabList.Add(go.name, go);
			Prefab = go;
		}
		public static void HackLoctaions()
		{
			if (!ZNet.instance.IsServer())
			{
				return;
			}
			var locPar = GameObject.Find("/_Locations").transform;
			for (int i = 0; i < locPar.childCount; i++)
			{
				var par = locPar.GetChild(i);
				for (int k = 0; k < par.childCount; k++)
				{
					var par2 = par.GetChild(k);
					var dgg = par2.GetComponentInChildren<DungeonGenerator>();
					if (dgg)
					{
						var go = Instantiate(Prefab, dgg.transform);
						go.name = ("LocMark");
						DBG.blogWarning("Hack Dungeon" + par2.name);
					}
					else
					{
						var go2 = Instantiate(Prefab, par2);
						go2.name = ("LocMark");
					}


				}
			}
		}
		#endregion Start

		#region Debug
		public void WatchMe()
		{
			GameCamera.instance.transform.localPosition = transform.position + Vector3.forward * 1;
		}
		#endregion Debug

		//EndClass
	}
}

