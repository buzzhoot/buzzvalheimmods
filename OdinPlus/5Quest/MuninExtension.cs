using System;

namespace OdinPlus
{
	public static class MuninExtension
	{
		public static void SetMuninHints(this Quest quest)
		{
			switch (quest.GetQuestType())
			{
				case QuestType.Hunt:
					//HintTarget = string.Format("Looks like you are close to the <color=yellow><b>[{0}]</b></color> Watchout!", locName);
					quest.HintStart = String.Format("$op_quest_hunt_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_hunt_target_po_1 ", quest.locName);
					quest.HintTarget = String.Format("$op_task_hunt_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_hunt_target_po_1 ", quest.locName);
					break;
				case QuestType.Treasure:
					//HintTarget = string.Format("Looks like you are close to the chest,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					quest.HintStart = String.Format("$op_quest_treasure_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_treasure_target_po_1 ", quest.locName);
					quest.HintTarget = String.Format("$op_task_treasure_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_treasure_target_po_1 ", quest.locName);
					break;
				case QuestType.Dungeon:
					//HintTarget = string.Format("Looks like you are close to the dungeon,look around find a <color=yellow><b>[{0}]</b></color>", locName);
					quest.HintStart = String.Format("$op_quest_dungeon_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_quest_dungeon_target_po_1 ", quest.locName);
					quest.HintTarget = String.Format("$op_task_dungeon_target_pr_1 <color=yellow><b>[{0}]</b></color> $op_task_dungeon_target_po_1 ", quest.locName);
					break;
			}
		}
		public static void SetMuninMessage(this Quest quest)
		{
			quest.m_message=quest.m_index+" $op_quest_side " + " $op_quest_quest "  + "\n" + quest.QuestName;
		}

	}
}