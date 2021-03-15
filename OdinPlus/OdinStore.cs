using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OdinPlus
{
    class OdinStore:MonoBehaviour,Hoverable,Interactable
    {
        #region Var
        public static OdinStore m_instance;
        #endregion,
        #region Mono
        public  void Awake()
        {
             m_instance = this;
        }
        #endregion
        #region Features
        #endregion
        #region Util
        #endregion
        #region Valheim
        public string GetHoverText()
        {
            string n = "<color=blue><b>Odin's Pot</b></color>";
            string s = string.Format("\n<color=green><b>Score:{0}</b></color>", OdinScore.score);
            string a = "\n[<color=yellow><b>E</b></color>] Raise Your Skill:[<color=green><b>{0}</b></color>]";
            return (n + s + a );
        }
        public string GetHoverName()
        {
            return "<color=blue><b>Odin's Pot</b></color>";
        }
        public bool Interact(Humanoid user, bool hold)
        {
            if (hold)
            {
                return false;
            }
            OdinTrader.Say("Want something magic,Warrior?");
            //end of interact
            return true;
        }
        public bool UseItem(Humanoid user, ItemDrop.ItemData item)
        {
            return false;
        }
        #endregion
    }
}
