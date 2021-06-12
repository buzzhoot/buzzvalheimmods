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
		public static Dictionary<string, GameObject> HumanList = new Dictionary<string, GameObject>();
		public static GameObject BasicHuman;
		#region Presets
		public static string[] Weapons = { "AtgeirBlackmetal", "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilve",
		 "SledgeIron", "SledgeStagbreaker", "SpearBronze", "SpearElderbark", "SpearFlint", "SpearWolfFang", "SwordBlackmetal", "SwordBronze","SwordIron", "SwordSilver", "AtgeirBlackmetal",
		 "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilver" };
		public static string[] Armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "ArmorLeatherChest", "ArmorLeatherLegs", "ArmorPaddedCuirass", "ArmorPaddedGreaves", "ArmorRagsChest",
		 "ArmorRagsLegs", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs", "ArmorWolfChest", "ArmorWolfLegs", "CapeDeerHide", "CapeLinen", "CapeLox", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake",
		 "HelmetIron", "HelmetLeather", "HelmetPadded", "HelmetTrollLeather", "HelmetYule" };
		public static string[] Shield = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
		public static string[] Tools = { "AxeIron", "PickAxeIron" };
		private class humanData
		{
			public string presetNAME = "MidEnemy1";
			public string prefab = "Goblin";
			public bool isFriend = false;
			public float m_randomMoveInterval = 30;
			public float m_randomMoveRange = 3;
			public float m_moveMinAngle = 30;
			public float health = 200;
			public float speed = 7;
			public string sets = "Troll";
			public string[] weapons = { "SwordBronze", "SwordIron", "AtgeirBronze", "AtgeirIron", "SpearBronze" };
			public string[] sheild = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };
			//public string[] armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake", "HelmetIron" };
		}
		private static List<humanData> presets = new List<humanData>
		{
			new humanData(),
			new humanData(){presetNAME="LowEnemey1",health=300,
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			},
			new humanData(){presetNAME="Fighter1",health=500,
			sets="Troll0",
			weapons = new string[]{"Club","SpearFlint","KnifeFlint"},
			},
			new humanData(){presetNAME="Fighter2",health=500,
			sets="Brozen",
			},
			new humanData(){presetNAME="DumbNPC",health=300,
			sets="Troll0",
			weapons = new string[]{"SwordBronze", "SwordIron"},
			m_randomMoveRange=0,
			isFriend=true,
			},
			new humanData(){presetNAME="DumbWorker",health=300,
			sets="Troll0",
			m_randomMoveRange=0,
			weapons=Tools,
			sheild=new string[]{""},
			isFriend=true,
			},
			new humanData(){presetNAME="GuardNPC",health=300,
			sets="Padded0",
			m_randomMoveInterval=5,
			m_randomMoveRange=60,
			isFriend=true,
			}
		};
		#endregion Presets
		public static Dictionary<string, GameObject> HumanPreset = new Dictionary<string, GameObject>();
		public static Dictionary<string, string[]> ArmorSets = new Dictionary<string, string[]>
		{
			{"Troll",new string[]{"HelmetTrollLeather","CapeTrollHide","ArmorTrollLeatherChest","ArmorTrollLeatherLegs"}},
			{"Troll0",new string[]{"CapeTrollHide","ArmorTrollLeatherChest","ArmorTrollLeatherLegs"}},
			{"Brozen",new string[]{"ArmorBronzeChest","ArmorBronzeLegs","HelmetBronze","CapeTrollHide"}},
			{"Iron",new string[]{"ArmorIronChest","ArmorIronLegs","HelmetIron","CapeLinen"}},
			{"Silver",new string[]{"ArmorWolfChest","ArmorWolfLegs","HelmetDrake","CapeWolf"}},
			{"Padded",new string[]{"ArmorPaddedCuirass","ArmorPaddedGreaves","HelmetPadded","CapeLinen"}},
			{"Padded0",new string[]{"ArmorPaddedCuirass","ArmorPaddedGreaves","CapeLinen"}}
		};
		public static void Init()
		{
			HackValHuman();
			HumanNpc();
			initSpawner();

			OdinPlus.OdinPostRegister(PrefabList);
			Plugin.posZone = (Action)Delegate.Combine(Plugin.posZone, (Action)PostZone);
			isInit = true;
		}

		#region Npcs
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

			go.AddComponent<HumanVis>();

			hum.CopySonComponet<Humanoid, Player>(oply);

			DestroyImmediate(go.GetComponent<Player>());

			BasicHuman = go;

			go.name = "BasicHuman";

		}
		public static void HumanNpc()
		{
			CreateNPC<HumanFighter>("Fighter1");
			CreateNPC<HumanFighter>("Fighter2");
			CreateNPC<MaterialVillager>("DumbWorker", "MatNPCHuman");
			CreateNPC<HumanMessager>("DumbWorker", "MessageNPCHuman");
			CreateNPC<HumanWorker>("DumbWorker", "WorkerNPCHuman");
			CreateNPC<HumanVillager>("GuardNPC", "GuardVillager");
		}
		public static void CreateNPC<T>(string pname, string goname) where T : Component
		{
			var go = Instantiate(BasicHuman, OdinPlus.PrefabParent.transform);
			CreatePreset(go, pname);
			go.AddComponent<T>();
			go.name = goname;
			PrefabList.Add(go.name, go.gameObject);
			HumanList.Add(go.name, go.gameObject);
			DBG.blogWarning("Create Human" + go.name);
		}
		public static void CreateNPC<T>(string pname) where T : Component
		{
			CreateNPC<T>(pname, pname);
		}
		#endregion Npcs

		#region Tool
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
		public static Humanoid.ItemSet GetSet(string set_name)
		{
			Humanoid.ItemSet result = new Humanoid.ItemSet();
			string[] list = ArmorSets[set_name];
			result.m_name = set_name;
			var sets = RandomVis(list);
			result.m_items = sets;
			return result;
		}
		public static void PostZone()
		{
			var exc_prb = Tutorial.instance.m_ravenPrefab.transform.Find("Munin").gameObject;
			foreach (var item in PrefabList.Values)
			{
				var comp = item.GetComponent<QuestVillager>();
				if (comp)
				{
					var go = comp.gameObject;
					var exc = Instantiate(exc_prb.GetComponentInChildren<Raven>().m_exclamation, Vector3.up *1.3f+go.transform.position, Quaternion.identity, go.transform);
					exc.name = "excOBJ";
					exc.transform.localScale = Vector3.one * 0.5f;
					comp.EXCobj = exc;
				}
			}

			HackingLoc();
		}

		#endregion Tool		

		#region Test
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
			hum.m_randomSets = new Humanoid.ItemSet[1] { GetSet("Silver") };
			hum.m_unarmedWeapon = null;
			//hum.m_randomArmor = RandomVis(Armor);
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
			hum.m_randomSets = new Humanoid.ItemSet[1] { GetSet("Silver") };
			//hum.m_randomArmor = RandomVis(Armor);
			hum.m_randomWeapon = RandomVis(Weapons);
			hum.m_randomShield = RandomVis(Shield);

			go.name = "HumanMobB";
			PrefabList.Add(go.name, go.gameObject);
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

		#endregion Test

		#region Spawner
		public static void initSpawner()
		{
			foreach (var item in HumanList.Keys)
			{
				CreateSpawner(item);
			}
		}
		public static void CreateSpawner(string cname)
		{
			var go = new GameObject(cname + "Spawner");
			go.transform.SetParent(PrefabManager.Root.transform);
			var znv = go.AddComponent<ZNetView>();
			var spn = go.AddComponent<CreatureSpawner>();
			spn.m_creaturePrefab = PrefabList[cname];
			znv.m_persistent = true;

			spn.m_respawnTimeMinuts = 0;
			spn.m_levelupChance = 10;
			spn.m_setPatrolSpawnPoint = true;
			PrefabList.Add(go.name, go);
			DBG.blogWarning("Create Spawner " + go.name);
		}
		#endregion Spawner

		#region OldPreset
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
			hum.m_randomSets = new Humanoid.ItemSet[1] { GetSet(dat.sets) };
			hum.m_unarmedWeapon = null;
			hum.m_randomWeapon = RandomVis(dat.weapons);
			hum.m_randomShield = RandomVis(dat.sheild);

			mai.m_randomMoveInterval = dat.m_randomMoveInterval;
			mai.m_randomMoveRange = dat.m_randomMoveRange;
			mai.m_moveMinAngle = dat.m_moveMinAngle;

			go.name = dat.presetNAME;
			HumanPreset.Add(dat.presetNAME, go);
		}
		private static void CreatePreset(GameObject go, string s)
		{
			var dat = presets.Where(c => c.presetNAME == s).ToArray()[0];
			go.transform.SetParent(OdinPlus.PrefabParent.transform);
			var hum = go.GetComponent<Humanoid>();
			var mai = go.AddComponentcc<MonsterAI>(ZNetScene.instance.GetPrefab(dat.prefab).GetComponent<MonsterAI>());
			mai.m_alertedEffects.m_effectPrefabs = new EffectList.EffectData[0];
			mai.m_idleSound.m_effectPrefabs = new EffectList.EffectData[0];
			//hum.m_runSpeed = dat.speed;
			hum.m_health = dat.health;
			hum.m_defaultItems = new GameObject[0];
			hum.m_randomSets = new Humanoid.ItemSet[1] { GetSet(dat.sets) };
			hum.m_unarmedWeapon = null;
			hum.m_randomWeapon = RandomVis(dat.weapons);
			hum.m_randomShield = RandomVis(dat.sheild);

			mai.m_randomMoveInterval = dat.m_randomMoveInterval;
			mai.m_randomMoveRange = dat.m_randomMoveRange;
			mai.m_moveMinAngle = dat.m_moveMinAngle;
		}
		public static GameObject GetPreset(string prname)
		{
			return HumanPreset[prname];
		}
		public static void AddPreset(GameObject go, string prname)
		{
			go.GetComponent<Humanoid>().CopyOtherComonent(HumanPreset[prname].GetComponent<Humanoid>());
			go.AddComponentcc<MonsterAI>(HumanPreset[prname].GetComponent<MonsterAI>());
		}

		#endregion OldPreset

		#region  HackingLocation	
		public static void HackingLoc()
		{
			HackingFarm();
			HackingRuneStones();
		}
		public static void HackingFarm()
		{
			Transform t = PrefabManager.Root.transform;
			var a = ZoneSystem.instance.m_locations;
			foreach (var item in a)
			{
				if (item.m_prefabName == "WoodFarm1")
				{
					t = item.m_prefab.transform;
					break;
				}
			}
			var guard = ZNetScene.instance.GetPrefab("GuardVillager" + "Spawner");
			var msg = ZNetScene.instance.GetPrefab("MessageNPCHuman" + "Spawner");
			var rsc = ZNetScene.instance.GetPrefab("MatNPCHuman" + "Spawner");
			rsc = Instantiate(rsc, new Vector3(5, 0, 5) + t.position, Quaternion.identity, t);
			msg = Instantiate(msg, new Vector3(5.5f, 0, 5.5f) + t.position, Quaternion.identity, t);
			for (int i = 0; i < 9; i++)
			{
				guard  = Instantiate(guard, new Vector3(10.RollDices(), 0, 10.RollDices()) + t.position, Quaternion.identity, t);
			}
			rsc.name=rsc.name.RemoveClone();
			msg.name=msg.name.RemoveClone();
			guard.name=guard.name.RemoveClone();
			DBG.blogWarning("Hacking Village");
		}
		private static readonly string[] rstones = new string[] { "Runestone_Meadows", "Runestone_Swamps", "Runestone_BlackForest" };
		public static void HackingRuneStones()
		{
			Transform t = PrefabManager.Root.transform;
			var a = ZoneSystem.instance.m_locations;
			foreach (var item in rstones)
			{
				foreach (var item2 in a)
				{
					if (item2.m_prefabName==item)
					{
						t=item2.m_location.gameObject.transform;
					}
				}
				var go = Instantiate(ZNetScene.instance.GetPrefab("Fighter1" + "Spawner"), t.position, Quaternion.identity, t);
				go.name="Fighter1" + "Spawner";
				var rnd = go.AddComponent<RandomSpawn>();
				rnd.m_chanceToSpawn = 90;
				DBG.blogWarning("hacking " + item);
			}

		}
		#endregion  HackingLocation

	}
}