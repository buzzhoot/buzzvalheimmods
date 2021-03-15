using UnityEngine;

namespace AdminTool
{
    class FT
    {
        public static void SpawnPrefab(string prefabName, Player player, int amount = 1, int level = 1, bool pickup = false, bool ignoreStackSize = false)
        {
            //Debug.Log("Easy spawner: Trying to spawn " + prefabName);
            GameObject prefab = ZNetScene.instance.GetPrefab(prefabName);
            bool flag = !prefab;
            if (flag)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, prefabName + " does not exist", 0, null);
                Debug.Log("Easy spawner: spawning " + prefabName + " failed");
            }
            else
            {
                bool flag2 = prefab.GetComponent<Character>();
                if (flag2)
                {
                    for (int i = 0; i < amount; i++)
                    {
                        Character component = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position + player.transform.forward * 2f + Vector3.up, Quaternion.identity).GetComponent<Character>();
                        bool flag3 = level > 1;
                        if (flag3)
                        {
                            component.SetLevel(level);
                        }
                    }
                }
                else
                {
                    if (prefab.GetComponent<ItemDrop>())
                    {
                        ItemDrop component2 = prefab.GetComponent<ItemDrop>();
                        bool flag5 = component2.m_itemData.IsEquipable();
                        if (flag5)
                        {
                            if (ignoreStackSize)
                            {
                                component2.m_itemData.m_stack = amount;
                                amount = 1;
                            }
                            component2.m_itemData.m_quality = level;
                            component2.m_itemData.m_durability = component2.m_itemData.GetMaxDurability();
                            for (int j = 0; j < amount; j++)
                            {
                                SpawnItem(pickup, prefab, player);
                            }
                        }
                        else
                        {
                            int num = 1;
                            int num2 = 0;
                            if (ignoreStackSize)
                            {
                                component2.m_itemData.m_stack = amount;
                            }
                            else
                            {
                                int maxStackSize = component2.m_itemData.m_shared.m_maxStackSize;
                                component2.m_itemData.m_stack = maxStackSize;
                                num = amount / maxStackSize;
                                num2 = amount % maxStackSize;
                            }
                            for (int k = 0; k < num; k++)
                            {
                                SpawnItem(pickup, prefab, player);
                            }
                            bool flag6 = num2 != 0;
                            if (flag6)
                            {
                                component2.m_itemData.m_stack = num2;
                                SpawnItem(pickup, prefab, player);
                            }
                        }
                        component2.m_itemData.m_stack = 1;
                        component2.m_itemData.m_quality = 1;
                    }
                    else
                    {
                        for (int l = 0; l < amount; l++)
                        {
                            UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position + player.transform.forward * 2f, Quaternion.identity);
                        }
                    }
                }
                Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "Spawning object " + prefabName, 0, null);
                //Debug.Log("Easy spawner: Spawned " + amount.ToString() + " " + prefabName);
            }
        }
        private static void SpawnItem(bool pickup, GameObject prefab, Player player)
        {
            if (pickup)
            {
                Player.m_localPlayer.PickupPrefab(prefab, 0);
            }
            else
            {
                UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position + player.transform.forward * 2f + Vector3.up, Quaternion.identity);
            }
        }
    }
}
