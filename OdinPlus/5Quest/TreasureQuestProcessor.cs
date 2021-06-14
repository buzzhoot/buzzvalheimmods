using UnityEngine;

namespace OdinPlus
{
  public class TreasureQuestProcessor : QuestProcessor
  {
    public TreasureQuestProcessor(Quest inq)
    {
      quest = inq;
    }

    public override void Place(LocationMarker lm)
    {
      var pos = lm.GetPosition();
      float y = -2f;
      float x = 4f;
      float z = 3.999f;
      if (quest.Key == 0)
      {
        y = 0;
        x = 2f;
        z = 1.999f;
      }

      pos += new Vector3(x.RollDice(), y, z.RollDice());
      var chest = LegacyChest.Place(pos, quest.ID, quest.m_ownerName, quest.Key);
      DBG.blogWarning("Client Placed LegacyChest at : " + pos);
      base.Place(lm);
    }
  }
}
