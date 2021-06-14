namespace OdinPlus
{
  public class QuestProcessor
  {
    protected Quest quest;

    public static QuestProcessor Create(Quest inq)
    {
      switch (inq.m_type)
      {
        case QuestType.Dungeon:
          return new DungeonQuestProcessor(inq);
        case QuestType.Treasure:
          return new TreasureQuestProcessor(inq);
        case QuestType.Hunt:
          return new HuntQuestProcessor(inq);
        case QuestType.Search:
          return new SearchQuestProcessor(inq);
        default:
          return new QuestProcessor(inq);
      }
    }

    public void SetQuest(Quest inq)
    {
      quest = inq;
    }

    public QuestProcessor(Quest inq)
    {
      quest = inq;
    }

    protected QuestProcessor()
    {
    }

    public virtual void Init()
    {
      var list1 = QuestRef.LocDic[quest.GetQuestType()];
      var list2 = list1[quest.Key];
      quest.locName = list2.GetRandomElement();
      QuestManager.instance.Invoke("ShowWaitError", 10);
      ZRoutedRpc.instance.InvokeRoutedRPC("RPC_ServerFindLocation", quest.locName, quest.m_realPostion);
    }

    public virtual void Begin()
    {
      QuestManager.instance.MyQuests.Add(quest.ID, quest);
      quest.m_ownerName = Player.m_localPlayer.GetPlayerName();
      quest.Begin();
    }

    public virtual void Place(LocationMarker lm)
    {
      lm.Used();
    }

    public virtual void Finish()
    {
      quest.Finish();
    }
  }
}
