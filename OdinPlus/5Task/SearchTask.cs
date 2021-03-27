using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace OdinPlus
{
	public class SearchTask : OdinTask
	{
		private string[] m_targetList;
		private List<string[]> m_itemList = new List<string[]>();
		private string m_item;
		private int m_count;
		private void Awake()
		{
			if (loading)
			{
				return;
			}
			m_type = TaskManager.TaskType.Search;

			m_tier0 = new string[] { "LeatherScraps:20", "Mushroom:20", "CookedMeat:20", "Raspberry:20", "Stone:50", "Wood:50", "DeerHide:15", "Resin:20" };
			m_tier1 = new string[] { "TrollHide:10", "Coins:100", "Resin:50", "BoneFragments:20", "MushroomYellow:20", "Blueberries:20", "HardAntler:1" };
			m_tier2 = new string[] { "Guck:30", "Ooze:10", "SurtlingCore:20", "ElderBark:50", "Amber:20", "AmberPearl:20", "Resin:100" };
			m_tier3 = new string[] { "Guck:50", "Ooze:20", "SurtlingCore:30", "ElderBark:50", "DragonEgg:1", "WolfFang:20", "WolfPelt:10", "Resin:100" };
			m_tier4 = new string[] { "Amber:20", "AmberPearl:20", "Bread:50", "Guck:80", "Ooze:40", "SurtlingCore:30", "ElderBark:80", "DragonEgg:1", "WolfFang:20", "WolfPelt:20", "Resin:100" };
			m_itemList.Add(m_tier0);
			m_itemList.Add(m_tier1);
			m_itemList.Add(m_tier2);
			m_itemList.Add(m_tier3);
			m_itemList.Add(m_tier4);
			Begin();
		}
		protected override void Update()
		{
			return;
		}
		private bool PickItem()
		{
			var l1 = new Dictionary<string, int>();
			foreach (var item in m_itemList[Key])
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
			m_count = l1.ElementAt(ind).Value * Level;
			return true;

		}
		protected override void Begin()
		{
			isMain = TaskManager.isMain;
			Key = TaskManager.GameKey;
			//!
			if (Key == 5)
			{
				Key = Key.RollDice();
			}
			//!
			Level = TaskManager.Level;
			if (!PickItem())
			{
				DBG.InfoCT("Clear some search Quest then Come back");
				DestroyImmediate(this.gameObject);
				return;
			}
			var item = Tweakers.GetItemData(m_item);
			HintTarget = String.Format("Find [<color=yellow><b>{0} {1}</b></color>] for Munin,he'll give you something nice ", m_count, item.m_shared.m_name);
			taskName = item.m_shared.m_name + " Search";

			OdinData.Data.TaskCount++;
			m_index = OdinData.Data.TaskCount;

			Id = m_item;
			gameObject.name = "Task" + Id;
			OdinData.Data.SearchTaskList.Add(m_item, m_count);

			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + "\n" + taskName + "\nStart", true);
			Tweakers.TaskHintHugin((isMain ? "Main" : "Side") + "Quest " + m_index + " : " + taskName, HintTarget);
			m_isInit = true;
		}
		public override void Finish()
		{
			MessageHud.instance.ShowBiomeFoundMsg((isMain ? "Main" : "Side") + " Quest " + m_index + "\n" + taskName + "\nClear", true);

			OdinData.Data.SearchTaskList.Remove(Id);
			OdinMunin.Reward(Key, Level);
			base.Clear();
		}
		public override void Clear()
		{
			OdinData.Data.SearchTaskList.Remove(Id);
			base.Clear();
		}
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
			Debug.LogWarning(count);
			var id = Tweakers.GetItemData(item);
			var mstk = id.m_shared.m_maxStackSize;

			if (count > mstk)
			{
				inv.RemoveItem(Tweakers.GetItemData(item), id.m_shared.m_maxStackSize);
				OdinData.Data.SearchTaskList[item] -= mstk;
				return false;
			}
			inv.RemoveItem(Tweakers.GetItemData(item), count);
			var t = TaskManager.Root.transform.Find("Task" + item);
			t.gameObject.GetComponent<SearchTask>().Finish();
			return true;
		}
	}
}