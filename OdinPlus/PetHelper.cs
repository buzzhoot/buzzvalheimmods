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
			Pet.petIns = this.gameObject;
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
			FocreAttack();
		}
		void OnDestroy()
		{
			Pet.Indicator.SetActive(false);
			Pet.petIns = null;
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
				if (Pet.Indicator.activeSelf)
				{
					Pet.Indicator.SetActive(false);
					// Pet.Indicator.transform.SetParent(PrefabsParent.transform);
					Traverse.Create(this.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(null);
					DBG.InfoCT("Stop pet attack");//trans
					return;
				}
				Pet.Indicator.SetActive(true);
				// if (__instance.GetHoverCreature() != null)
				// {
				// 	Pet.Indicator.transform.SetParent(__instance.GetHoverCreature().transform);
				// }
				Pet.Indicator.transform.position = raycastHit.point;
				ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { raycastHit.point, 3, "attack here!", "" });
				Traverse.Create(this.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(Pet.Indicator.GetComponent<StaticTarget>());
				DBG.InfoCT("Pet force attack");//trans
				return;
			}
		}
	}
}
