using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OdinPlus
{
	public class SearchQuestProcesser : QuestProcesser
	{
		#region Var
		private string m_item;
		private int m_count;
		#endregion Var

		#region Main
		public override void Init()
		{
			if (!PickItem())
			{
				DBG.InfoCT("Clear some search Quest then Come back");
				//upd Failed process
				return;
			}
			quest.locName=m_item;
			Begin();
		}
		 public override void Begin()
		{
			quest.ID=m_item;
			base.Begin();
		}
		#endregion Main
		#region Feature
		public static bool CanOffer(string item)
		{
			if (OdinData.Data.SearchTaskList.ContainsKey(item))
			{
				return true;
			}
			return false;
		}
		public static bool CanFinish(string item)
		{
			var inv = Player.m_localPlayer.GetInventory();
			int count = OdinData.Data.SearchTaskList[item];
			string iname = Tweakers.GetItemData(item).m_shared.m_name;
			Debug.LogWarning(count);
			if (inv.CountItems(iname) >= count)
			{
				inv.RemoveItem(iname, count);
				var t = TaskManager.Root.transform.Find("Task" + item);
				t.gameObject.GetComponent<SearchTask>().Finish();
				return true;
			}
			return false;
		}

		#endregion Feature

		#region Tool
		private bool PickItem()
		{
			var m_itemList = QuestRef.LocDic[quest.GetQuestType()];
			var l1 = new Dictionary<string, int>();
			foreach (var item in m_itemList[quest.Key])
			{
				var a1 = item.Split(new char[] { ':' });
				l1.Add(a1[0], int.Parse(a1[1]));
			}
			foreach (var item in OdinData.Data.SearchTaskList.Keys)
			{
				if (l1.ContainsKey(item))
				{
					l1.Remove(item);
				}
			}
			if (l1.Count == 0)
			{
				return false;
			}
			int ind = l1.Count.RollDice();
			m_item = l1.ElementAt(ind).Key;
			m_count = l1.ElementAt(ind).Value * quest.Level;
			return true;
		}
		#endregion Tool
	}
}