using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	class PetHelper : MonoBehaviour
	{
		private Tameable tame;
		void Awake()
		{
			tame = this.GetComponent<Tameable>();
			tame.m_commandable = true;
			tame.m_fedDuration = 300;
			tame.Tame();
			Traverse.Create(tame).Method("ResetFeedingTimer").GetValue();
		}
		void Update()
		{
			if (this.GetComponent<Tameable>().IsHungry())
			{
				ZNetScene.instance.Destroy(this.gameObject);
			}
		}
		void OnDestroy()
		{
			Pet.petIns = null;
			DBG.InfoCT(Localization.instance.Localize(this.GetComponent<Humanoid>().m_name+" died"));//add trans
			
		}
	}
}
