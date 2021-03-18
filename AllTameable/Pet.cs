using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace AllTameable
{
    class Pet
    {
        private static ZNetScene zns;
        private static Tameable wtame;
        private static GameObject PrefabTransform;
        //public static Dictionary<string,float> GrowTime=new Dictionary<string, float>();
        //private static Dictionary<string, GameObject> PetList = new Dictionary<string, GameObject>();

        public static void init(ZNetScene instance)
        {
            zns = instance;
            var ptroll = zns.GetPrefab("Troll");
            wtame = zns.GetPrefab("Wolf").GetComponent<Tameable>();
            var list = Plugin.cfgList;
            foreach (var obj in list)
            {
                string name = obj.Key ;
                if (zns.GetPrefab(name) == null)
                {
                    DBG.blogWarning("Cant find Prefab Check your name : "+name);
                }
                addtame(zns.GetPrefab(name), obj.Value);
            }
        }
        private static void addtame(GameObject go,Plugin.TameTable tb)
        {
            if (go.GetComponent<MonsterAI>() == null)
            {
                DBG.blogWarning(go.name + " can't be added,Remove it in your cfg");
                return;
            }
            var tame = go.AddComponent<Tameable>();     
            tame.m_petEffect = wtame.m_petEffect;
            tame.m_sootheEffect = wtame.m_sootheEffect;
            tame.m_petEffect = wtame.m_petEffect;
            tame.m_commandable = tb.commandable;
            tame.m_tamingTime = tb.tamingTime;
            tame.m_fedDuration = tb.fedDuration;
            var ma = go.GetComponent<MonsterAI>();
            ma.m_consumeRange = tb.consumeRange;
            ma.m_consumeSearchInterval = tb.consumeSearchInterval;
            ma.m_consumeHeal = tb.consumeHeal;
            ma.m_consumeSearchRange = tb.consumeSearchRange;            
            foreach (var itm in tb.consumeItems)
            {
                var a = ObjectDB.instance.GetItemPrefab(itm);
                if (a==null)
                {
                    DBG.blogWarning("Wrong food name :" + itm);
                }
                else
                {
                    DBG.blogInfo("add "+itm+" to "+go.name);
                    ma.m_consumeItems.Add(a.GetComponent<ItemDrop>());
                }
            }
            if (tb.procretion)
            {
                var pc = go.AddComponent<Procreation>();
                pc.m_maxCreatures = tb.maxCreatures*2;
                pc.m_pregnancyChance = tb.pregnancyChance;
                pc.m_pregnancyDuration = tb.pregnancyDuration;
                pc.m_partnerCheckRange = 30;
                pc.m_totalCheckRange = 30;
                pc.m_offspring = go;
            }
            //tame.Tame();
            //go.AddComponent<PetHelper>();
            //go.GetComponent<Rigidbody>().useGravity = false;
            //go.GetComponent<Humanoid>().enabled = false;
            //go.GetComponent<MonsterAI>().enabled = false;
            //go.GetComponent<ZSyncAnimation>().enabled = false;
            //go.GetComponent<ZSyncTransform>().enabled = false;
        }
        public static GameObject spawnMini(string name)
        {
            var pgo = zns.GetPrefab(name);
            pgo.SetActive(false);
            GameObject go = GameObject.Instantiate(zns.GetPrefab(name));
            pgo.SetActive(true);
            go.name = "Mini" + name;
            go.transform.localPosition = new Vector3(1000,1000,1000);
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
            go.SetActive(true);
            ZNetView znv = go.GetComponent<ZNetView>();
            ZDO zdo = go.GetComponent<ZNetView>().GetZDO();
            zns.m_instances.Remove(zdo);
            ZDOMan.instance.DestroyZDO(zdo);

            var gu = go.AddComponent<Growup>();
            gu.m_grownPrefab = zns.GetPrefab(name);
            gu.m_growTime = Plugin.cfgList[name].growTime;
            GameObject.Destroy(go, 3);
            return go;
        }
        //public static GameObject spawnMini(string name)
        //{
        //    var pgo = zns.GetPrefab(name);
        //    GameObject go = GameObject.Instantiate(zns.GetPrefab(name), new Vector3(1000, 1000, 1000),Quaternion.identity, zns.gameObject.transform.parent);
        //    go.name = "Mini" + name;
        //    go.transform.localScale *= 0.5f;
        //    go.GetComponent<Humanoid>().m_name = "Mini " + go.GetComponent<Humanoid>().m_name;
        //    if (go.GetComponent<VisEquipment>() != null)
        //    {
        //        GameObject.DestroyImmediate(go.GetComponent<VisEquipment>());
        //    }
        //    if (go.GetComponent<CharacterDrop>() != null)
        //    {
        //        GameObject.DestroyImmediate(go.GetComponent<CharacterDrop>());
        //    }
        //    go.GetComponent<Humanoid>().m_faction = Character.Faction.Players;
        //    go.GetComponent<Tameable>().Tame();
        //    ZNetView znv = go.GetComponent<ZNetView>();
        //    ZDO zdo = go.GetComponent<ZNetView>().GetZDO();
        //    zns.m_instances.Remove(zdo);
        //    ZDOMan.instance.DestroyZDO(zdo);
        //    var tb = Plugin.cfgList[name];
        //    GameObject.Destroy(go, 5);
        //    return go;
        //}
    }
}
