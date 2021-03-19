using System;
using System.Collections.Generic;
using UnityEngine;
namespace AllTameable
{
	public class Hatch : Growup
	{
		//public int m_growTime;
		//public GameObject m_grownPrefab;
		//private ZNetView m_nview;
		private new ZNetView m_nview;
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
			if (GetTimeSinceSpawned().TotalSeconds > (double)this.m_growTime)
			{
				Tameable component2 = UnityEngine.Object.Instantiate<GameObject>(this.m_grownPrefab, base.transform.position, base.transform.rotation).GetComponent<Tameable>();
				if (component2)
				{
					component2.Tame();
					//component2.SetLevel(component.GetLevel());
				}
				this.m_nview.Destroy();
			}

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
