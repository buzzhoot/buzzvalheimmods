using UnityEngine;

namespace OdinPlus
{

	public class OdinNPC : MonoBehaviour, Hoverable, Interactable,OdinInteractable
	{
		public string m_name;
		public Transform m_head;
		public GameObject m_talker;
		public virtual void Say(string text)
		{
			text=Localization.instance.Localize(text);
			var tname=Localization.instance.Localize(m_name);
			Chat.instance.SetNpcText(m_talker, Vector3.up * 1.5f, 60f, 5, tname, text, false);
		}
		public virtual bool Interact(Humanoid user, bool hold)
		{
			if (hold)
			{
				return false;
			}
			return true;
		}
		public virtual void SecondaryInteract (Humanoid user)
		{

		}
		public virtual string GetHoverText()
		{
			string n = string.Format("<color=lightblue><b>{0}}</b></color>", m_name);
			n += string.Format("\n<color=green><b>Credits:{0}</b></color>", OdinData.Credits);
			return Localization.instance.Localize(n);
		}
		public virtual string GetHoverName()
		{
			return Localization.instance.Localize(m_name);
		}
		public virtual bool UseItem(Humanoid user, ItemDrop.ItemData item)
		{
			return false;
		}

	}
}