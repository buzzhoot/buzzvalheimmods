using System;
using System.Collections.Generic;
using UnityEngine;
namespace AllTameable
{
	public class Hatch : Growup,Hoverable
	{
		//public int m_growTime;
		//public GameObject m_grownPrefab;
		//private ZNetView m_nview;
		public string m_name;
		private new ZNetView m_nview;
		private double growStats;

		private new void  Start()
		{
			m_nview = gameObject.GetComponent<ZNetView>();
			InvokeRepeating("GrowUpdate", UnityEngine.Random.Range(10f, 15f), 10f);
		}
		private new void GrowUpdate()
		{
			if (!this.m_nview.IsValid() || !this.m_nview.IsOwner())
			{
				return;
			}
			growStats=GetTimeSinceSpawned().TotalSeconds/(double)this.m_growTime*100;
			this.GetComponent<Character>().m_name+=string.Format("\n Hatching Progress:<color=yellow><b>{0}</b></color>%",growStats.ToString());
			if (GetTimeSinceSpawned().TotalSeconds > (double)this.m_growTime)
			{
				Tameable component2 = UnityEngine.Object.Instantiate<GameObject>(this.m_grownPrefab, base.transform.position, base.transform.rotation).GetComponent<Tameable>();
				if (component2)
				{
					component2.Tame();
				}
				this.m_nview.Destroy();
			}

		}
		public string GetHoverText()
		{
			string n = m_name;
			n +=string.Format("\n Hatching Progress:<color=yellow><b>{0}</b></color>%",growStats.ToString());
			return n;
		}
		public string GetHoverName()
		{
			return "HatchingEgg";
		}
		private TimeSpan GetTimeSinceSpawned()
		{
			long num = this.m_nview.GetZDO().GetLong("spawntime", 0L);
			if (num == 0L)
			{
				num = ZNet.instance.GetTime().Ticks;
				this.m_nview.GetZDO().Set("spawntime", num);
			}
			DateTime d = new DateTime(num);
			return ZNet.instance.GetTime() - d;
		}
	}
}
