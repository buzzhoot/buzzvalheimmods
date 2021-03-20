using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	public class PetWolf : MonoBehaviour, OdinInteractable
	{
		public Container container;
		private Tameable tame;
		private Inventory m_inventory;
		private Humanoid m_hum;
		private void Awake()
		{
			PetManager.WolfIns= this.gameObject;
			Character character = this.GetComponent<Character>();
			character.m_onDeath = (Action)Delegate.Combine(new Action(this.OnDestroyed), character.m_onDeath);
			tame = this.GetComponent<Tameable>();
			tame.Tame();
			tame.m_fedDuration = 600;
			m_inventory=Traverse.Create(container).Field<Inventory>("m_inventory").Value;
			m_hum=this.GetComponent<Humanoid>();
		}
		public static void Teleport() { }
		private void OnDestroyed()
		{
			//?Action a =  (Action)Traverse.Create(container).Field<Action>("OnDestryod").Value;
			List<ItemDrop.ItemData> allItems = m_inventory.GetAllItems();
			int num = 1;
			foreach (ItemDrop.ItemData item in allItems)
			{
				Vector3 position = base.transform.position + Vector3.up * 0.5f + UnityEngine.Random.insideUnitSphere * 0.3f;
				Quaternion rotation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
				ItemDrop.DropItem(item, 0, position, rotation);
				num++;
			}
		}
		private void Update() {
			var weight =Traverse.Create(m_inventory).Field<float>("m_totalWeight").Value;
			m_hum.ChangeSpeed(300/(weight+0.0001f)*2);//trans
		}
		public void SecondaryInteract()
		{
			container.Interact(Player.m_localPlayer, false);
		}
	}
}