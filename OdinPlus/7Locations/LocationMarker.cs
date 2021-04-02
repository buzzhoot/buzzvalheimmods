using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	public class LocationMarker : MonoBehaviour
	{
		public static Dictionary<string,LocationMarker> MarkList =new Dictionary<string, LocationMarker> ();
		private List<Container> m_container = new List<Container>();
		public string ID = "";
		public bool used = false;
		private ZNetView m_nview;
		private LocationProxy m_locationProxy;
		private Vector3 m_pos;
		private void Awake()
		{
			m_nview=GetComponent<ZNetView>();
			m_locationProxy= transform.parent.GetComponent<LocationProxy>();
			m_pos=m_locationProxy.transform.position;
			if (m_nview.GetZDO() == null)
			{
				DBG.blogWarning("Mark Report zdo null");
				return;
			}
			Debug.LogWarning("Mark report zdo get");
			MarkList.Add(ID,this);
		}
	}
}

