using System;
using UnityEngine;
namespace OdinPlus
{
	public class TreasureQuestProcesser : QuestProcesser
	{
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
			pos +=new Vector3(x.RollDice(),y,z.RollDice());
			var chest=LegacyChest.Place(pos,quest.m_ownerName,quest.ID,quest.Key);
			DBG.blogWarning("Sever Placed LegacyChest at : " + pos);
			base.Place(lm);
		}
	}
}