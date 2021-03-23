using System;
using UnityEngine;
namespace OdinPlus
{
	public static class Tweakers
	{
		public static Humanoid ChangeSpeed(this Humanoid humanoid, float speed)
		{
			humanoid.m_speed = speed;
			return humanoid;
		}
		public static Tutorial.TutorialText SendRavenMessage(string messageName, string messageText)
		{
			Tutorial.TutorialText tutorialText = new Tutorial.TutorialText
			{
				m_label = "OdinQuest",
				m_name = messageName,
				m_text = messageText,
				m_topic = "Quest Hints"
			};
			if (!Tutorial.instance.m_texts.Contains(tutorialText))
			{
				Tutorial.instance.m_texts.Add(tutorialText);
			}
			Player.m_localPlayer.ShowTutorial(tutorialText.m_name, false);
			return tutorialText;
		}
		public static string GetTransName(this string str)
		{
			return ObjectDB.instance.GetItemPrefab(str).GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
		}
		public static string GetLocal(this string str)
		{
			return Localization.instance.Localize(str);
		}
	}

}