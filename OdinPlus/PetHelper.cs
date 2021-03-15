using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace OdinPlus
{
    class PetHelper:MonoBehaviour
    {
        private Tameable tame;
        //public Transform cmdt;
        void Awake()
        {
            //gameObject.AddComponent<ZNetView>();
            var d =this.GetComponent<CharacterDrop>();
            Destroy(d);
            tame = this.GetComponent<Tameable>();
            tame.m_commandable = true;
            tame.m_tamingTime = 0;
            tame.m_fedDuration = 300;
            //ZNetView nview;
            //if (TryGetComponent<ZNetView>(out nview)){ }
            //else{gameObject.AddComponent<ZNetView>(); }
            Pet.SetPrefab(this.gameObject, true);
            tame.Tame();
            Traverse.Create(tame).Method("ResetFeedingTimer").GetValue();
        }
        void Update()
        {

        }
        void OnDestroy()
        {
            Pet.petIns = null;
            DBG.InfoCT("Pet died");
        }
    }
}
