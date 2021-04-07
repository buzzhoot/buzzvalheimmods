using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OdinPlus
{
	public class LocationMarker : MonoBehaviour
	{
		#region Var
		public static Dictionary<string, LocationMarker> MarkList = new Dictionary<string, LocationMarker>();
		public static Dictionary<string, LocationMarker> DunList = new Dictionary<string, LocationMarker>();
		public static Dictionary<string, LocationMarker> DubList = new Dictionary<string, LocationMarker>();
		public static GameObject Prefab;

		#region Setting
		public string ID = "";
		public string owner = "";
		#endregion Setting
		#region Internal
		private ZNetView m_nview;
		#endregion Internal
		#region Out
		public struct CtnInfo
		{
			public Vector3 Pos;
			public Quaternion Rot;
		}
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
			if (m_nview.GetZDO().GetBool("LocMarkUsed", false))
			{
				MarkList.Remove(ID);
				Destroy(this);
				return;
			}
			else
			{
				ID = ZoneSystem.instance.GetZone(transform.position).Pak();
				if (MarkList.ContainsKey(ID))
				{
					DubList.Add(ID, this);
				}
				else
				{
					MarkList.Add(ID, this);
				}

				DBG.blogInfo("locmark placed at zone : " + ID);
			}
		}
		private void Start()
		{

		}

		private void OnDestroy()
		{
			if (ID == "")
			{
				return;
			}
			MarkList.Remove(ID);
		}
		#region feature
		public CtnInfo GetCtnInfo()
		{
			CtnInfo result;
			var ctns = GetComponentsInChildren<Container>(true);
			if (ctns.Length != 0)
			{
				var ctn = ctns[ctns.Length.RollDice()];
				return result = new CtnInfo { Pos = ctn.transform.position, Rot = ctn.transform.rotation };
			}
			Room[] array = GetComponentsInChildren<Room>();
			var array2 = array.Where(c => c.m_endCap != true).ToArray();
			var room = array2[array2.Length.RollDice()];
			var y = room.GetComponentInChildren<RoomConnection>().transform.localPosition.y;
			var x = room.m_size.x / 2;
			var z = room.m_size.z / 2;
			var pos = new Vector3(0, y + 0.2f, 0) + room.transform.position;
			result = new CtnInfo { Pos = pos, Rot = Quaternion.identity };
			return result;
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}



		public void Used()
		{
			m_nview.GetZDO().Set("LocMarkUsed", true);
			Destroy(this);
		}

		#endregion feature

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
						DBG.blogWarning("Hack Dungeon: " + par2.name);
					}
					else
					{
						var go2 = Instantiate(Prefab, par2);
						go2.name = ("LocMark");
						DBG.blogWarning("Hack Location: " + par2.name);
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
		public void DrawBall()
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.transform.position = transform.position;
			go.transform.localScale = Vector3.one * 3;
			go.GetComponent<Renderer>().material.color = Color.red;
			go.name = ("LocMark");
			go.transform.SetParent(transform);
		}
		public void test()
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Vector3 pos;
			Quaternion rot;
			var info = GetCtnInfo();
			pos = info.Pos;
			rot = info.Rot;
			go.transform.localScale = Vector3.one;
			go.transform.position = pos;
			go.transform.rotation = rot;
			go.GetComponent<Renderer>().material.color = Color.red;
			go.AddComponent<Light>();
			go.name = ("Fake Chest" + ID);
			GameCamera.instance.transform.localPosition = pos;
		}
		#endregion Debug

		//EndClass
	}
}

