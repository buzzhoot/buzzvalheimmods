using System;
using System.Collections.Generic;
using UnityEngine;
namespace OdinPlus
{
	class OdinMunin : OdinNPC
	{
		private string[] choice = new string[] { "Accept Side Quest", "Give up Quest", "Change Quest Level", "Show me QuestList" };
		private int index = 0;
		private string currentChoice;
		private float timer = 0f;
		private Animator m_animator;
		private void Awake()
		{
			this.m_name = "$odin.munin";
			this.m_talker = this.gameObject;
			currentChoice = choice[index];
			m_animator= this.GetComponentInChildren<Animator>();
		}
		private void Start()
		{
			gameObject.transform.Rotate(0, -30f, 0);
			this.m_animator.SetTrigger("teleportin");
			this.m_animator.SetTrigger("talk");
			
		}
		private void Update()
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;
			}
		}
		#region Feature
		private void CreatSideQuest()
		{
			if (timer > 0)
			{
				Say("Wait a moment,Hugin will find something for you");
				return;
			}
			if (TaskManager.Root.transform.childCount >= 10)
			{
				Say("You have too many quests,clear some before you want more");
				return;
			}
			TaskManager.CreateRandomTask();
			Say("Wait for Hugin,he will tell you");
			timer = 300f;//add
		}
		private void GiveUpTask()
		{
			if (TaskManager.HasTask())
			{
				//string n = string.Format("Which Quest you want to give up?", TaskManager.Count());
				string n = "Which Quest you want to give up?";
				n = Localization.instance.Localize(n);
				TextInput.instance.RequestText(new TR_Giveup(), n, 3);
			}
		}
		private void ChangeLevel()
		{
			if (TaskManager.Level == TaskManager.MaxLevel)
			{
				TaskManager.Level = 1;
				return;
			}
			TaskManager.Level++;
		}
		
		#endregion Feature
		#region Val

		#endregion Val
		public override bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			switch (index)
			{
				case 0:
					CreatSideQuest();
					break;
				case 1:
					GiveUpTask();
					break;
				case 2:
					ChangeLevel();
					break;
				case 3:
					if (TaskManager.HasTask())
					{
						TaskManager.PrintTaskList();
						Say("Wait for Hugin,he will tell you something");
						break;
					}
					Say("You don't have any Quest");
					break;
			}
			return true;
		}
		public override void SecondaryInteract(Humanoid user)
		{
			index += 1;
			if (index + 1 > choice.Length)
			{
				index = 0;
			}
			currentChoice = choice[index];
		}
		public override string GetHoverText()
		{
			string n = string.Format("<color=lightblue><b>{0}</b></color>", m_name);
			n += string.Format("\n<color=lightblue><b>Current Quest Level:{0}</b></color>", TaskManager.Level);
			n += string.Format("\nYou have <color=lightblue><b>{0}</b></color> Tasks", TaskManager.Count());
			n += "\n[<color=yellow><b>$KEY_Use</b></color>]" + currentChoice;
			n += String.Format("\n<color=yellow><b>[{0}]</b></color>Switch Choice", Plugin.KS_SecondInteractkey.Value.MainKey.ToString());
			return Localization.instance.Localize(n);
		}
		public override string GetHoverName()
		{
			return Localization.instance.Localize(this.m_name);
		}
		public override bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}
		#region TextGUI
		private class TR_Giveup : TextReceiver
		{
			public string GetText()
			{
				return "";
			}
			public void SetText(string text)
			{
				int num;
				if (int.TryParse(text, out num))
				{
					if (!TaskManager.GiveUpTask(num))
					{
						DBG.InfoCT("You don't have Quest " + num);
						return;
					}
				}
				DBG.InfoCT("Wrong Input");
				return;
			}
		}
		#endregion TextGUI

	}
}