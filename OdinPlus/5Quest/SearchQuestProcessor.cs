using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OdinPlus
{
  public class SearchQuestProcessor : QuestProcessor
  {
    #region Var

    private string m_item;
    private int m_count;

    #endregion Var

    #region Main

    public SearchQuestProcessor(Quest inq)
    {
      quest = inq;
    }

    public override void Init()
    {
      if (!PickItem())
      {
        DBG.InfoCT("Clear some search Quest then Come back");
        //upd Failed process
        return;
      }

      quest.locName = m_count.ToString() + m_item;
      quest.m_range = 0;
      Begin();
    }

    public override void Begin()
    {
      quest.ID = m_item;
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
#if DEBUG
      DBG.blogInfo($"OdinData.Data.SearchTaskList:{OdinData.Data.SearchTaskList.Count}, item:{item}");
      foreach (var kvp in OdinData.Data.SearchTaskList)
      {
        DBG.blogInfo($"key: {kvp.Key}, value: {kvp.Value}");
      }
#endif
      var inv = Player.m_localPlayer.GetInventory();
      int count = OdinData.Data.SearchTaskList[item];
      string iname = Tweakers.GetItemData(item).m_shared.m_name;
      Debug.LogWarning($"Needed: {item} x{count}");
      if (inv.CountItems(iname) >= count)
      {
        inv.RemoveItem(iname, count);

        DBG.blogInfo($"Current Quests");
        foreach (KeyValuePair<string, Quest> kvp in QuestManager.instance.MyQuests)
        {
          DBG.blogInfo($"key: {kvp.Key}, value: {kvp.Value.QuestName}");
        }

        if (QuestManager.instance.MyQuests.ContainsKey(item))
        {
          var quest = QuestManager.instance.MyQuests[item];
          quest.Finish();
          OdinMunin.Reward(quest.Key, quest.Level);
        }

        if (OdinData.Data.SearchTaskList.ContainsKey(item))
        {
          OdinData.Data.SearchTaskList.Remove(item);
        }
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
        var a1 = item.Split(new char[] {':'});
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
      OdinData.Data.SearchTaskList.Add(m_item, m_count);
      return true;
    }

    #endregion Tool
  }
}
