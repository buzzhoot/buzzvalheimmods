using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
	public class LocationMarker : MonoBehaviour
	{
		#region Var
		public static Dictionary<string, LocationMarker> MarkList = new Dictionary<string, LocationMarker>();

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
			if (m_nview.GetZDO().GetBool("Used", false))
			{
				ZNetScene.instance.Destroy(gameObject);
				//add remove list
			}
			m_locationProxy = transform.parent.GetComponent<LocationProxy>();
			m_pos = m_locationProxy.transform.position;
			ID = m_nview.GetZDO().GetString("MarkID", ID);
			DBG.blogWarning("Mark report zdo get");
			MarkList.Add(ID, this);
		}
		private void Start()
		{
			DBG.blogWarning("Start");
		}
		public void Used()
		{
			m_nview.GetZDO().Set("Used", true);
		}
	}
}

