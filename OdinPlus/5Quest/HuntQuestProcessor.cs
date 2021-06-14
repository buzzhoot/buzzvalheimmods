using System.Text.RegularExpressions;

namespace OdinPlus
{
  public class HuntQuestProcessor : QuestProcessor
  {
    public HuntQuestProcessor(Quest inq)
    {
      quest = inq;
    }

    public override void Begin()
    {
      SetMonsterName();
      base.Begin();
    }

    public override void Place(LocationMarker lm)
    {
      HuntTarget.Place(lm.GetPosition(), quest.locName, quest.ID, quest.m_ownerName, quest.Key, quest.Level);
      base.Place(lm);
    }

    private void SetMonsterName()
    {
      var Key = quest.Key;
      if (Key >= 5 || Key <= 0)
      {
        Key = 4.RollDice() + 1;
      }

      quest.locName = QuestRef.HunterMonsterList[Key - 1];
      quest.locName = Regex.Replace(quest.locName, @"[_]", "");
    }
  }
}
