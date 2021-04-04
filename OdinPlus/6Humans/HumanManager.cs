using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;
namespace OdinPlus
{

	public class HumanManager : MonoBehaviour
	{

		public static bool isInit = false;
		public static Dictionary<string, GameObject> PrefabList = new Dictionary<string, GameObject>();
		public static GameObject BasicHuman;
		public static string[] Weapons = { "AtgeirBlackmetal", "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilve",
		 "SledgeIron", "SledgeStagbreaker", "SpearBronze", "SpearElderbark", "SpearFlint", "SpearWolfFang", "SwordBlackmetal", "SwordBronze","SwordIron", "SwordSilver", "AtgeirBlackmetal",
		 "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilver" };
		public static string[] Armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "ArmorLeatherChest", "ArmorLeatherLegs", "ArmorPaddedCuirass", "ArmorPaddedGreaves", "ArmorRagsChest",
		 "ArmorRagsLegs", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs", "ArmorWolfChest", "ArmorWolfLegs", "CapeDeerHide", "CapeLinen", "CapeLox", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake",
		 "HelmetIron", "HelmetLeather", "HelmetPadded", "HelmetTrollLeather", "HelmetYule" };
		public static string[] Shield = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
		private class humanData
		{
			public string presetNAME = "MidEnemy1";
			public string prefab = "Goblin";
			public bool isFriend = false;
			public float m_randomMoveInterval = 30;
			public float health = 200;
			public float speed = 7;
			public string[] weapons = { "SwordBronze", "SwordIron", "AtgeirBronze", "AtgeirIron", "SpearBronze" };
			public string[] sheild = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
			public string[] armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake", "HelmetIron" };
		}
		private static List<humanData> presets = new List<humanData>
		{
			new humanData(),
			new humanData(){presetNAME="LowEnemey1",health=300,
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			armor = new string[]{"ArmorTrollLeatherLegs","ArmorTrollLeatherChest","CapeTrollHide"}
			},
			new humanData(){presetNAME="Fighter1",health=500,
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			armor = new string[]{""},
			},
			new humanData(){presetNAME="DumbNPC",health=300,
			m_randomMoveInterval=30000,
			weapons = new string[]{""},
			isFriend=true,
			armor = new string[]{""},
			sheild=new string[]{"ShieldWood", "ShieldWoodTower"}
			},
			new humanData(){presetNAME="GuardNPC",health=300,
			weapons = new string[]{"SwordBronze", "SwordIron"},
			m_randomMoveInterval=30,
			isFriend=true,
			armor = new string[]{""},
			sheild=new string[]{""}
			}
		};
		public static Dictionary<string, GameObject> HumanPreset = new Dictionary<string, GameObject>();
		public static void Init()
		{

			HackValHuman();
			CreaterPresets();
			HumanNpc();
			HumanMobA();
			HumanMobB();
			HumanSpawner();

			//HackSpawner();
			//HackCamp();


			OdinPlus.OdinPostRegister(PrefabList);
			isInit = true;
		}
		private static void CreaterPresets()
		{
			foreach (var item in presets)
			{
				CreatePreset(item);
			}
		}
		private static void CreatePreset(humanData dat)
		{
			var go = new GameObject();
			go.transform.SetParent(OdinPlus.PrefabParent.transform);
			var hum = go.AddComponentcc<Humanoid>(BasicHuman.GetComponent<Humanoid>());
			var mai = go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab(dat.prefab).GetComponent<MonsterAI>());
			mai.m_alertedEffects.m_effectPrefabs = new EffectList.EffectData[0];
			mai.m_idleSound.m_effectPrefabs = new EffectList.EffectData[0];
			//hum.m_runSpeed = dat.speed;
			hum.m_health = dat.health;
			hum.m_defaultItems = new GameObject[0];
			hum.m_randomSets = new Humanoid.ItemSet[0];
			hum.m_unarmedWeapon = null;
			hum.m_randomArmor = RandomVis(dat.armor);
			hum.m_randomWeapon = RandomVis(dat.weapons);
			hum.m_randomShield = RandomVis(dat.sheild);

			mai.m_attackPlayerObjects = !dat.isFriend;
			mai.m_randomMoveInterval = dat.m_randomMoveInterval;

			go.name = dat.presetNAME;
			HumanPreset.Add(dat.presetNAME, go);
		}
		public static GameObject GetPreset(string prname)
		{
			return HumanPreset[prname];
		}
		public static void AddPreset(GameObject go, string prname)
		{
			go.AddComponentcc<Humanoid>(HumanPreset[prname].GetComponent<Humanoid>());
			go.AddComponentcc<MonsterAI>(HumanPreset[prname].GetComponent<MonsterAI>());
		}
		private static void HackValHuman()
		{
			var go = Instantiate(Game.instance.m_playerPrefab, OdinPlus.PrefabParent.transform);
			go.GetComponent<ZNetView>().m_persistent = true;
			DestroyImmediate(go.GetComponent<PlayerController>());
			DestroyImmediate(go.GetComponent<Talker>());
			DestroyImmediate(go.GetComponent<Skills>());

			var oply = go.GetComponent<Player>();
			var vis = go.GetComponent<VisEquipment>();
			var hum = go.AddComponent<Humanoid>();

			//vis.m_isPlayer = false;

			hum.CopySonComponet<Humanoid, Player>(oply);

			DestroyImmediate(go.GetComponent<Player>());

			BasicHuman = go;

			go.name = "BasicHuman";

		}
		public static void HumanNpc()
		{
			var go = Instantiate(BasicHuman, OdinPlus.PrefabParent.transform);

			var vis = go.GetComponent<VisEquipment>();
			var hum = go.GetComponent<Humanoid>();
			var mai = go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab("Goblin").GetComponent<MonsterAI>());

			hum.m_defaultItems = new GameObject[0];
			//hum.m_randomSets = new Humanoid.ItemSet[1]{new Humanoid.ItemSet(){m_items}}
			hum.m_unarmedWeapon = null;
			//hum.m_randomArmor = RandomVis(Armor);
			hum.m_randomWeapon = RandomVis(Weapons);
			hum.m_randomShield = RandomVis(Shield);



			//go.GetComponentInChildren<Animator>().SetBool("wakeup", false);
			//+Delay EXC
			//var exc_prb = Tutorial.instance.m_ravenPrefab.transform.Find("Munin").gameObject;
			//var exc = Instantiate(exc_prb.GetComponentInChildren<Raven>().m_exclamation, Vector3.up*2, Quaternion.identity, go.transform);
			//exc.name="excOBJ";
			//exc.transform.localScale=Vector3.one*0.5f;

			var hnpc = go.AddComponent<HumanNPC>();
			hnpc.m_shoulderItem = new string[] { "CapeTrollHide", "CapeDeerHide" };
			hnpc.m_chestItem = new string[] { "ArmorTrollLeatherChest", "ArmorLeatherChest" };
			hnpc.m_legItem = new string[] { "ArmorTrollLeatherLegs", "ArmorLeatherLegs", "HelmetTrollLeather" };


			//ADD exc

			go.name = "HumanNPC";
			PrefabList.Add(go.name, go.gameObject);
		}
		public static void HumanMobA()
		{
			var go = Instantiate(BasicHuman, OdinPlus.PrefabParent.transform);

			var vis = go.GetComponent<VisEquipment>();
			var hum = go.GetComponent<Humanoid>();
			//vis.m_isPlayer = false;

			hum.m_health = 1000;
			hum.m_faction = Character.Faction.Players;

			var mai = go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab("Goblin").GetComponent<MonsterAI>());
			var tame = go.AddComponent<Tameable>();

			hum.m_defaultItems = new GameObject[0];
			hum.m_randomSets = new Humanoid.ItemSet[0];
			hum.m_unarmedWeapon = null;
			hum.m_randomArmor = RandomVis(Armor);
			hum.m_randomWeapon = RandomVis(Weapons);
			hum.m_randomShield = RandomVis(Shield);
			go.name = "HumanMobA";
			PrefabList.Add(go.name, go.gameObject);
		}
		public static void HumanMobB()
		{
			var go = Instantiate(BasicHuman, OdinPlus.PrefabParent.transform);

			var vis = go.GetComponent<VisEquipment>();
			var hum = go.GetComponent<Humanoid>();
			vis.m_isPlayer = false;

			hum.m_health = 1000;
			hum.m_faction = Character.Faction.PlainsMonsters;

			var mai = go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab("Goblin").GetComponent<MonsterAI>());
			var tame = go.AddComponent<Tameable>();

			hum.m_defaultItems = new GameObject[0];
			hum.m_unarmedWeapon = null;
			hum.m_randomSets = new Humanoid.ItemSet[0];
			hum.m_randomArmor = RandomVis(Armor);
			hum.m_randomWeapon = RandomVis(Weapons);
			hum.m_randomShield = RandomVis(Shield);

			go.name = "HumanMobB";
			PrefabList.Add(go.name, go.gameObject);
		}
		public static GameObject[] RandomVis(string[] list)
		{
			if (list.Length == 0)
			{
				return new GameObject[0];
			}
			GameObject[] items = new GameObject[list.Length];
			int i = 0;
			foreach (var item in list)
			{
				items[i] = ZNetScene.instance.GetPrefab(item);
				i++;
			}

			return items;
		}
		public static void HumanSpawner()
		{
			var go = Instantiate(ZNetScene.instance.GetPrefab("Spawner_Goblin"), OdinPlus.PrefabParent.transform);
			var a = go.GetComponent<CreatureSpawner>();
			go.name = "SpawnHuman";
			a.m_creaturePrefab = PrefabList["HumanMobA"];
			PrefabList.Add(go.name, go);
		}
		public static void HackSpawner()
		{
			var a = ZNetScene.instance.GetPrefab("Spawner_Goblin").GetComponent<CreatureSpawner>();
			a.m_creaturePrefab = PrefabList["HumanMobB"];
		}

		public static void HackCamp()
		{
			var list = DungeonDB.GetRooms();
			var go = list[0].m_room.transform.parent;
			var a = go.GetComponentsInChildren<CreatureSpawner>(true);
			//Debug.Log(a.Length);
			foreach (var item in a)
			{
				if (item.name.StartsWith("Spawner_Goblin"))
				{
					var c = Instantiate(PrefabList["SpawnHuman"], item.transform.parent);
					c.transform.localPosition = item.transform.localPosition;
					item.m_creaturePrefab = PrefabList["HumanMobB"];
					c.name = "SpawnHuman";
					//Debug.Log("hack campe");
				}
			}
			//var a =  ZNetScene.instance.GetPrefab("Spawner_Goblin").GetComponent<CreatureSpawner>();
			//a.m_creaturePrefab=HumanTest;
		}
	}
}