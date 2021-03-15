using System;
using System.Collections.Generic;

using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
    class Pet : MonoBehaviour
    {
        private static ZNetScene zns;
        private static GameObject TrollPrefab;
        private static Dictionary<string, GameObject> PetList = new Dictionary<string, GameObject>();
        public static GameObject petIns;
        public static GameObject Indicator;
        public static void init(ZNetScene instance)
        {
            zns = instance;
            var ptroll = zns.GetPrefab("Troll");
            var wtame = zns.GetPrefab("Wolf").GetComponent<Tameable>();
            TrollPrefab = GameObject.Instantiate(ptroll);
            TrollPrefab.name = "Troll_pet";
            var tame = TrollPrefab.AddComponent<Tameable>();
            tame.m_petEffect = wtame.m_petEffect;
            tame.m_sootheEffect = wtame.m_sootheEffect;
            tame.m_petEffect = wtame.m_petEffect;
            TrollPrefab.AddComponent<PetHelper>();
            PetList.Add("Troll_pet", TrollPrefab);
            TrollPrefab.GetComponent<Humanoid>().m_randomSets= TrollPrefab.GetComponent<Humanoid>().m_randomSets.Skip(1).ToArray();
            TrollPrefab.transform.position = new Vector3(1000, 1000, 1000);
            SetPrefab(TrollPrefab, false);
            TrollPrefab.transform.SetParent(Plugin.PrefabParent.transform);
        }
        public static bool GetPrefab(string name, out GameObject go)
        {
            if (PetList.ContainsKey(name))
            {
                go = PetList[name];
                return true;
            }
            go = null;
            return false;
        }
        public static void SetPrefab(GameObject go, bool setting)
        {
            go.GetComponent<Rigidbody>().useGravity = setting;
            go.GetComponent<Humanoid>().enabled = setting;
            go.GetComponent<MonsterAI>().enabled = setting;
            go.GetComponent<ZSyncAnimation>().enabled = setting;
            go.GetComponent<ZSyncTransform>().enabled = setting;
            go.GetComponent<FootStep>().enabled = setting;
        }
        public static void Clear()
        {
            foreach (var o in PetList)
            {
                GameObject.Destroy(o.Value);
            }
            petIns = null;
            PetList.Clear();
            DBG.blogInfo("PetList Clear");
        }
        public static void CmdHelper()
        {
            RaycastHit raycastHit;
            if (petIns != null && Input.GetKeyDown(KeyCode.BackQuote) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
                Vector3 point = raycastHit.point;
                Debug.DrawRay(Player.m_localPlayer.transform.position, point, Color.white);
                if (Indicator.activeSelf)
                {
                    Indicator.SetActive(false);
                    Traverse.Create(petIns.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(null);
                    DBG.InfoCT("Stop pet attack");
                    return;
                }
                Indicator.SetActive(true);
                Indicator.transform.position = raycastHit.point;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { raycastHit.point, 3, "attack here!", "" });
                Traverse.Create(petIns.GetComponent<MonsterAI>()).Field("m_targetStatic").SetValue(Indicator.GetComponent<StaticTarget>());
                DBG.InfoCT("Pet force attack");
                return;
            }
        }
        public static void initIndicator()
        {
            Indicator = new GameObject("Indicator");
            DontDestroyOnLoad(Indicator);
            Indicator.AddComponent<StaticTarget>();
            Indicator.AddComponent<CapsuleCollider>();
            Indicator.SetActive(false);
        }
        public static void SummonHelper()
        {
            if (petIns != null)
            {
                DBG.InfoCT("You can have only one helper");
                return;
            }
            var ppfb = ZNetScene.instance.GetPrefab("Troll_pet");
            if (ppfb == null)
            {
                DBG.blogWarning("Pet spawned failed cannot find the prefab");
            }
            petIns = Instantiate(ppfb, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up, Quaternion.identity);
            DBG.InfoCT("You summoned a helper");//trans
        }
    }
}
