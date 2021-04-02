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
		public static string[] Weapons = { "AtgeirBlackmetal", "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilve", "SledgeIron", "SledgeStagbreaker", "SpearBronze", "SpearElderbark", "SpearFlint", "SpearWolfFang", "SwordBlackmetal", "SwordBronze", "SwordCheat", "SwordIron", "SwordIronFire", "SwordSilver", "AtgeirBlackmetal", "AtgeirBronze", "AtgeirIron", "Battleaxe", "KnifeBlackMetal", "KnifeChitin", "KnifeCopper", "KnifeFlint", "MaceBronze", "MaceIron", "MaceNeedle", "MaceSilver" };
		public static string[] Armor = { "ArmorBronzeChest", "ArmorBronzeLegs", "ArmorIronChest", "ArmorIronLegs", "ArmorLeatherChest", "ArmorLeatherLegs", "ArmorPaddedCuirass", "ArmorPaddedGreaves", "ArmorRagsChest", "ArmorRagsLegs", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs", "ArmorWolfChest", "ArmorWolfLegs", "CapeDeerHide", "CapeLinen", "CapeLox", "CapeTest", "CapeTrollHide", "CapeWolf", "HelmetBronze", "HelmetDrake", "HelmetDverger", "HelmetIron", "HelmetLeather","HelmetPadded", "HelmetTrollLeather", "HelmetYule" };
		public static string[] Shield = { "ShieldBanded", "ShieldBlackmetal", "ShieldBlackmetalTower", "ShieldBronzeBuckler", "ShieldIronSquare", "ShieldIronTower", "ShieldKnight", "ShieldSerpentscale", "ShieldSilver", "ShieldWood", "ShieldWoodTower" };

		public static void Init()
		{

			HackValHuman();
			HumanNpc();
			HumanMobA();
			HumanMobB();
			HumanSpawner();
			HackSpawner();

			//HackCamp();


			OdinPlus.OdinPostRegister(PrefabList);
			isInit = true;
		}
		public static void HackValHuman()
		{
			var go = Instantiate(Game.instance.m_playerPrefab, OdinPlus.PrefabParent.transform);

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
			DestroyImmediate(hum);
			//DestroyImmediate(go.GetComponent<ZSyncAnimation>());
			//DestroyImmediate(go.GetComponent<ZSyncTransform>());
			DestroyImmediate(go.GetComponentInChildren<CharacterAnimEvent>());
			//hum.m_health = 1000;
			//hum.m_faction = Character.Faction.Boss;

			//EXC
			go.AddComponent<StaticPhysics>();
			go.GetComponentInChildren<Animator>().SetBool("wakeup",false);//+
			//+Delay EXC
			//var exc_prb = Tutorial.instance.m_ravenPrefab.transform.Find("Munin").gameObject;
			//var exc = Instantiate(exc_prb.GetComponentInChildren<Raven>().m_exclamation, Vector3.up*2, Quaternion.identity, go.transform);
			//exc.name="excOBJ";
			//exc.transform.localScale=Vector3.one*0.5f;

			var hnpc = go.AddComponent<HumanNPC>();
			hnpc.m_shoulderItem=new string[]{"CapeTrollHide","CapeDeerHide"};
			hnpc.m_chestItem=new string[]{"ArmorTrollLeatherLegs","ArmorLeatherChest"};
			hnpc.m_legItem=new string[]{"ArmorTrollLeatherLegs", "ArmorLeatherLegs"};
			

			//ADD exc

			go.name = "HumanNPC";
			PrefabList.Add(go.name, go.gameObject);
		}
		public static void HumanMobA()
		{
			var go = Instantiate(BasicHuman, OdinPlus.PrefabParent.transform);

			var vis = go.GetComponent<VisEquipment>();
			var hum = go.GetComponent<Humanoid>();
			vis.m_isPlayer = false;

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
			Debug.Log(a.Length);
			foreach (var item in a)
			{
				if (item.name.StartsWith("Spawner_Goblin"))
				{
					var c = Instantiate(PrefabList["SpawnHuman"], item.transform.parent);
					c.transform.localPosition = item.transform.localPosition;
					item.m_creaturePrefab = PrefabList["HumanMobB"];
					c.name = "SpawnHuman";
					Debug.Log("hack campe");
				}
			}
			//var a =  ZNetScene.instance.GetPrefab("Spawner_Goblin").GetComponent<CreatureSpawner>();
			//a.m_creaturePrefab=HumanTest;
		}
	}
}