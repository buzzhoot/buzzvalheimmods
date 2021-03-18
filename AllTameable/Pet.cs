using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace AllTameable
{
	class PetManager:MonoBehaviour
	{
		private static ZNetScene zns;
		private static Tameable wtame;
		public static GameObject Root;
		public static bool isInit;

		private void Awake() {
            Root=new GameObject("MiniOnes");
            Root.transform.SetParent(Plugin.prefabManager.Root.transform);
        }
        public static void Init()
		{
            zns = ZNetScene.instance;
			wtame = zns.GetPrefab("Wolf").GetComponent<Tameable>();
			var list = Plugin.cfgList;
			foreach (var obj in list)
			{
				string name = obj.Key;
				if (zns.GetPrefab(name) == null)
				{
					DBG.blogWarning("Cant find Prefab Check your name : " + name);
				}
				AddTame(zns.GetPrefab(name), obj.Value);
			}
			isInit = true;
		}
		private static void AddTame(GameObject go, Plugin.TameTable tb)
		{
			if (go.GetComponent<MonsterAI>() == null)
			{
				DBG.blogWarning(go.name + " can't be added,Remove it in your cfg");
				return;
			}
			Tameable tame;
			if (!go.TryGetComponent<Tameable>(out tame))
			{
				tame = go.AddComponent<Tameable>();
			}
			var ma = go.GetComponent<MonsterAI>();

			tame.m_petEffect = wtame.m_petEffect;
			tame.m_sootheEffect = wtame.m_sootheEffect;
			tame.m_petEffect = wtame.m_petEffect;
			tame.m_commandable = tb.commandable;
			tame.m_tamingTime = tb.tamingTime;
			tame.m_fedDuration = tb.fedDuration;
			
			ma.m_consumeRange = tb.consumeRange;
			ma.m_consumeSearchInterval = tb.consumeSearchInterval;
			ma.m_consumeHeal = tb.consumeHeal;
			ma.m_consumeSearchRange = tb.consumeSearchRange;

			foreach (var itm in tb.consumeItems)
			{
				var a = ObjectDB.instance.GetItemPrefab(itm);
				if (a == null)
				{
					DBG.blogWarning("Wrong food name :" + itm);
				}
				else
				{
					DBG.blogInfo("add " + itm + " to " + go.name);
					ma.m_consumeItems.Add(a.GetComponent<ItemDrop>());
				}
			}

			if (tb.procretion)
			{
				var pc = go.AddComponent<Procreation>();
				pc.m_maxCreatures = tb.maxCreatures * 2;
				pc.m_pregnancyChance = tb.pregnancyChance;
				pc.m_pregnancyDuration = tb.pregnancyDuration;
				pc.m_partnerCheckRange = 30;
				pc.m_totalCheckRange = 30;
				pc.m_offspring = SpawnMini(go);
			}
		}
		private static GameObject SpawnMini(GameObject prefab)
		{
			var pgo = prefab;
			var name=prefab.name;
			GameObject go = GameObject.Instantiate(zns.GetPrefab(name),Root.transform);
			go.name = "Mini" + name;
			go.transform.localScale *= 0.5f;
			go.GetComponent<Humanoid>().m_name = "Mini " + go.GetComponent<Humanoid>().m_name;
			if (go.GetComponent<MonsterAI>() != null)
			{
				GameObject.DestroyImmediate(go.GetComponent<MonsterAI>());
			}
			if (go.GetComponent<VisEquipment>() != null)
			{
				GameObject.DestroyImmediate(go.GetComponent<VisEquipment>());
			}
			if (go.GetComponent<CharacterDrop>() != null)
			{
				GameObject.DestroyImmediate(go.GetComponent<CharacterDrop>());
			}
			if (go.GetComponent<Tameable>() != null)
			{
				GameObject.DestroyImmediate(go.GetComponent<Tameable>());
			}
			if (go.GetComponent<Procreation>() != null)
			{
				GameObject.DestroyImmediate(go.GetComponent<Procreation>());
			}
			var mai = pgo.GetComponent<MonsterAI>();
			var aai = go.AddComponent<AnimalAI>();
			aai.CopyBroComponet<AnimalAI, MonsterAI>(mai);

			var gu = go.AddComponent<Growup>();
			gu.m_grownPrefab = zns.GetPrefab(name);
			gu.m_growTime = Plugin.cfgList[name].growTime;
			
            PrefabManager.PostRegister(go);

            return go;
		}
        public  void Clear()
        {

        }

	}
}
