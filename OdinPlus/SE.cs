using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OdinPlus
{
    class SE
    {
        public static StatusEffect Test;
        public static void init()
        {
            Sprite odinicon=ObjectDB.instance.GetItemPrefab("HelmetOdin").GetComponent<ItemDrop>().m_itemData.m_shared.m_icons[0];
            var mt=ObjectDB.instance.GetItemPrefab("MeadTasty");
            var id=mt.GetComponent<ItemDrop>().m_itemData;
            Test = new SE_TrollHelper();
            Test.m_icon = odinicon;
            Test.m_name = "OdinTest";
            Test.m_ttl = 60;
            id.m_shared.m_consumeStatusEffect = Test;
        }
    }
}
