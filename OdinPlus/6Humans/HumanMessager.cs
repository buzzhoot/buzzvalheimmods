using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace OdinPlus
{
	public class HumanMessager : QuestVillager, Hoverable, Interactable, OdinInteractable
	{

		protected override void Awake()
		{
			base.Awake();
			ChoiceList = new string[2] { "$op_talk", "$op_human_quest_take" };
		}
		public override void Choice0()
		{
			Say("I need some help,can you take a message for me ?");//trans
		}
		public void Choice1()
		{
			if (!IsQuestReady())
			{
				return;
			}
			var key = HumanVis.NPCnames.GetRandomElement();
			OdinData.AddKey(key);
			PlaceRandom(key);
			string n = String.Format("Thx, you can find <color=yellow><b>{0}</b></color> near our village", key);
			Say(n);
			ResetQuestCD();
		}
		private void PlaceQuestHuman(string key,Vector3 pos)
		{
			var pgo = ZNetScene.instance.GetPrefab("WorkerNPCHuman");
			var go  = Instantiate(pgo,PrefabManager.Root.transform);
			go.GetComponent<HumanVis>().m_name= key;
			float y;
			ZoneSystem.instance.FindFloor(pos,out y);
			pos = new Vector3(pos.x,y+2,pos.z);
			go.transform.localPosition = pos;
			go.transform.SetParent(transform.parent.parent);
			DBG.blogWarning("Place Quest Worker at " + pos);
		}
		private bool PlaceRandom(string key)
		{
			foreach (var item in LocationMarker.MarkList.Values)
			{
				var dis = Utils.DistanceXZ(item.GetPosition(),transform.position);
				if (dis>100)
				{
					PlaceQuestHuman(key,item.GetPosition());
					return true;
				}
			}
			return false;
		}

	}
}
