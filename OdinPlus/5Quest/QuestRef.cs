using System.Collections.Generic;

namespace OdinPlus
{
  public class QuestRef
  {
    private static readonly List<string[]> DungeonLoc = new List<string[]>
    {
      new[] {"Crypt3"},
      new[] {"Crypt3", "Crypt2", "Crypt4"},
      new[] {"SunkenCrypt4"},
      new[] {"SunkenCrypt4"},
      new[] {"GoblinCamp2"},
      new[] {"Crypt3", "Crypt2", "Crypt4", "SunkenCrypt4", "GoblinCamp2"}
    };

    private static readonly List<string[]> TreasureLoc = new List<string[]>
    {
      new[] {"WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse6", "WoodHouse7", "WoodHouse8", "WoodHouse9"},
      new[] {"WoodHouse3", "WoodHouse4", "Ruin2", "Ruins1", "ShipSetting01", "Runestone_Boars", "Runestone_Meadows", "Runestone_Greydwarfs", "Runestone_BlackForest"},
      new[]
      {
        "SwampRuin1", "SwampRuin2", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr", 
        "FireHole", "DrakeNest01", "Waymarker02", "AbandonedLogCabin02", "AbandonedLogCabin03", "AbandonedLogCabin04", "MountainGrave01"
      },
      new[] {"DrakeNest01", "Waymarker02", "AbandonedLogCabin02", "AbandonedLogCabin03", "AbandonedLogCabin04", "MountainGrave01", "DrakeLorestone"},
      new[] {"StoneHenge1", "StoneHenge2", "StoneHenge3", "StoneHenge4", "StoneHenge5", "StoneHenge6"},
      new[]
      {
        "WoodHouse11", "WoodHouse6", "WoodHouse3", "WoodHouse4", "WoodHouse6", "WoodHouse7", "WoodHouse8", "WoodHouse9", "WoodHouse3",
        "WoodHouse4", "Ruin2", "Ruins1", "ShipSetting01", "Runestone_Boars", "Runestone_Meadows", "Runestone_Greydwarfs", "Runestone_BlackForest", "SwampRuin1",
        "SwampRuin2", "SwampHut5", "SwampHut1", "SwampHut2", "SwampHut3", "SwampHut4", "Runestone_Draugr", "FireHole", "DrakeNest01", "Waymarker02",
        "AbandonedLogCabin02", "AbandonedLogCabin03", "AbandonedLogCabin04", "MountainGrave01", "DrakeNest01", "Waymarker02", "AbandonedLogCabin02",
        "AbandonedLogCabin03", "AbandonedLogCabin04", "MountainGrave01", "DrakeLorestone", "StoneHenge1", "StoneHenge2", "StoneHenge3", "StoneHenge4", "StoneHenge5", "StoneHenge6"
      }
    };

    private static readonly List<string[]> HuntLoc = new List<string[]>
    {
      new[] {"Runestone_Greydwarfs"},
      new[] {"Ruin1", "Runestone_Greydwarfs"},
      new[] {"Runestone_Draugr"},
      new[] {"Waymarker02"},
      new[] {"Runestone_Plains"},
      new[] {"Runestone_Greydwarfs", "Ruin1", "Runestone_Greydwarfs", "Runestone_Draugr", "Waymarker02", "Runestone_Plains"}
    };

    private static readonly List<string[]> SearchItem = new List<string[]>
    {
      new[] {"LeatherScraps:20", "Mushroom:20", "CookedMeat:20", "Raspberry:20", "Stone:50", "Wood:50", "DeerHide:15", "Resin:20"},
      new[] {"TrollHide:10", "Coins:100", "Resin:50", "BoneFragments:20", "MushroomYellow:20", "Blueberries:20", "HardAntler:1"},
      new[] {"Guck:30", "Ooze:10", "SurtlingCore:20", "ElderBark:50", "Amber:20", "AmberPearl:20", "Resin:100"},
      new[] {"Guck:50", "Ooze:20", "SurtlingCore:30", "ElderBark:50", "DragonEgg:1", "WolfFang:20", "WolfPelt:10", "Resin:100"},
      new[] {"Amber:20", "AmberPearl:20", "Bread:50", "Guck:80", "Ooze:40", "SurtlingCore:30", "ElderBark:80", "DragonEgg:1", "WolfFang:20", "WolfPelt:20", "Resin:100"}
    };

    public static Dictionary<QuestType, List<string[]>> LocDic = new Dictionary<QuestType, List<string[]>>
    {
      {QuestType.Dungeon, DungeonLoc},
      {QuestType.Hunt, HuntLoc}, 
      {QuestType.Search, SearchItem}, 
      {QuestType.Treasure, TreasureLoc}
    };

    public static string[] HunterMonsterList = {"Troll", "Draugr_Elite", "Fenring", "GoblinBrute"};
  }
}


/*Location List 
StoneCircle
StartTemple
Eikthyrnir
GoblinKing
Greydwarf_camp1
Greydwarf_camp2
Greydwarf_camp3
Runestone_Greydwarfs
Grave1
SwampRuin1
SwampRuin2
FireHole
Runestone_Draugr
Castle
Fort1
xmastree
GDKing
Bonemass
Meteorite
Crypt2
Ruin1
Ruin2
Pillar1
Pillar2
StoneHouse1
StoneHouse1_heath
StoneHouse2_heath
StoneHouse5_heath
StoneHouse2
StoneHouse3
StoneHouse4
StoneHouse5
Ruin3
GoblinCamp1
GoblinCamp2
StoneTower1
StoneTower2
StoneTower3
StoneTower4
StoneHenge1
StoneHenge2
StoneHenge3
StoneHenge4
StoneHenge5
StoneHenge6
WoodHouse1
WoodHouse2
WoodHouse3
WoodHouse4
WoodHouse5
WoodHouse6
WoodHouse7
WoodHouse8
WoodHouse9
WoodHouse10
WoodHouse11
WoodHouse12
WoodHouse13
WoodFarm1
WoodVillage1
TrollCave
TrollCave02
SunkenCrypt1
SunkenCrypt2
SunkenCrypt3
SunkenCrypt4
Dolmen01
Dolmen02
Dolmen03
Crypt3
Crypt4
InfestedTree01
SwampHut1
SwampHut2
SwampHut3
SwampHut4
SwampHut5
SwampWell1
StoneTowerRuins04
StoneTowerRuins05
StoneTowerRuins03
StoneTowerRuins07
StoneTowerRuins08
StoneTowerRuins09
StoneTowerRuins10
Vendor_BlackForest
ShipSetting01
Dragonqueen
DrakeNest01
Waymarker01
Waymarker02
AbandonedLogCabin02
AbandonedLogCabin03
AbandonedLogCabin04
MountainGrave01
DrakeLorestone
MountainWell1
MountainCave01
ShipWreck01
ShipWreck02
ShipWreck03
ShipWreck04
Hugintest
Runestone_Meadows
Runestone_Boars
Runestone_Swamps
Runestone_Mountains
Runestone_BlackForest
Runestone_Plains */
