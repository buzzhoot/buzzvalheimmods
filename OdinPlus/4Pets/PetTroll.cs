using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
	class PetTroll : MonoBehaviour
	{
		private Tameable tame;
		void Awake()
		{
			PetManager.TrollIns = this.gameObject;
			tame = this.GetComponent<Tameable>();
			tame.m_commandable = true;
			tame.m_fedDuration = 300;
			tame.Tame();
			Traverse.Create(tame).Method("ResetFeedingTimer").GetValue();
			Character character = this.GetComponent<Character>();
			character.m_onDeath = (Action)Delegate.Combine(new Action(this.OnDestroyed),character.m_onDeath);
		}
		void Update()
		{
			if (this.GetComponent<Tameable>().IsHungry())
			{
				ZNetScene.instance.Destroy(this.gameObject);
			}
			FocreAttack();
		}
		void OnDestroyed()
		{
			PetManager.Indicator.SetActive(false);
			PetManager.TrollIns = null;
			DBG.InfoCT(Localization.instance.Localize(this.GetComponent<Humanoid>().m_name + " died"));//add trans

		}
		public void FocreAttack()
		{

			if (Plugin.KS_SecondInteractkey.Value.IsDown())
			{
				var __instance = Player.m_localPlayer;
				Vector3 aimDir = __instance.GetAimDir(Vector3.zero);
				Ray ray = new Ray(GameCamera.instance.transform.position, GameCamera.instance.transform.forward);
				int layerMask = Pathfinding.instance.m_layers | Pathfinding.instance.m_waterLayers;
				RaycastHit raycastHit;
				Physics.Raycast(ray, out raycastHit, 500f, layerMask);
				Vector3 point = raycastHit.point;
				if (PetManager.Indicator.activeSelf)
				{
					PetManager.Indicator.SetActive(false);
					// Pet.Indicator.transform.SetParent(PrefabsParent.transform);
					Traverse.Create(this.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(null);
					DBG.InfoCT("Stop pet attack");//trans
					return;
				}
				PetManager.Indicator.SetActive(true);
				// if (__instance.GetHoverCreature() != null)
				// {
				// 	Pet.Indicator.transform.SetParent(__instance.GetHoverCreature().transform);
				// }
				PetManager.Indicator.transform.position = raycastHit.point;
				ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { raycastHit.point, 3, "attack here!", "" });
				Traverse.Create(this.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(PetManager.Indicator.GetComponent<StaticTarget>());
				DBG.InfoCT("Pet force attack");//trans
				return;
			}
		}
	}
}
