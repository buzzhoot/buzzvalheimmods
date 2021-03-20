using System;
using UnityEngine;

namespace OdinPlus
{
	public class PetWolf : MonoBehaviour,OdinInteractable
	{
		private Container container;
		private Tameable tame;
		private void Awake()
		{
			tame=this.GetComponent<Tameable>();
			tame.Tame();
			tame.m_fedDuration = 600;
			container= this.GetComponentInChildren<Container>();
		}
		public static void Teleport() { }
		private void OnDestroy()
		{

		}
		public void SecondaryInteract()
		{
			container.Interact(Player.m_localPlayer,false);
		}
	}
}