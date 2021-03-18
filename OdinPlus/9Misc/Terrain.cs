using System;
using System.Collections.Generic;
using UnityEngine;

namespace OdinPlus
{
    #region terrian
    public static class Terrain
    {
        public static bool Flatten(float radiusX, float radiusY, Transform t)
        {
            if (radiusY <= 0f)
            {
                radiusY = radiusX;
            }
            else if (radiusX <= 0f)
            {
                radiusX = radiusY;
            }
            Transform transform = t;
            Vector3 position = t.position;
            position = new Vector3(position.x-radiusY / 2, position.y, position.z-radiusY/2);
            //Log.Message("Attempting To Flatten...");
            GameObject prefab = ZNetScene.instance.GetPrefab("raise");
            if (prefab.GetComponent<Piece>() == null || prefab == null)
            {
                return false;
            }
            TerrainModifier.SetTriggerOnPlaced(true);
            int num = 0;
            while ((float)num < radiusX)
            {
                int num2 = 0;
                while ((float)num2 < radiusY)
                {
                    Terrain.SpawnFloor(position + Vector3.down * 0.5f + transform.forward * (float)num + transform.right * (float)num2, transform.rotation, prefab);
                    num2++;
                }
                num++;
            }
            TerrainModifier.SetTriggerOnPlaced(false);
            return true;
        }
        public static void RemoveFlora(float radius, Vector3 pos)
        {
            Collider[] array = Physics.OverlapBox(pos, new Vector3(radius, radius, radius));
            List<string> list = new List<string>();
            list.Add("tree");
            list.Add("rock");
            list.Add("beech");
            list.Add("log");
            list.Add("bush");
            Collider[] array2 = array;
            if (array.Length == 0) { return; }
            for (int i = 0; i < array2.Length; i++)
            {
                GameObject gameObject = array2[i].gameObject;
                GameObject gameObject2 = gameObject.transform.parent.gameObject;
                GameObject gameObject3 = null;
                string text = gameObject2.name.ToLower();
                string text2 = gameObject.name.ToLower();
                foreach (string value in list)
                {
                    if (text2.Contains(value))
                    {
                        gameObject3 = gameObject;
                        break;
                    }
                    if (!(gameObject2 == null) && text.Contains(value))
                    {
                        gameObject3 = gameObject2;
                        break;
                    }
                }
                if (!(gameObject3 == null))
                {
                    try
                    {
                        ZNetScene.instance.Destroy(gameObject3);
                    }
                    catch (Exception e)
                    {
                        DBG.blogWarning("terrain flora:" + e);
                        return;  
                    }
                }
            }
        }

        public static void ResetTerrain(Vector3 centerLocation, float radius)
        {
            radius = Mathf.Clamp(radius, 2f, 50f);
            try
            {
                foreach (TerrainModifier terrainModifier in TerrainModifier.GetAllInstances())
                {
                    if (terrainModifier != null && Utils.DistanceXZ(Player.m_localPlayer.transform.position, terrainModifier.transform.position) < radius)
                    {
                        ZNetView component = terrainModifier.GetComponent<ZNetView>();
                        if (component != null && component.IsValid())
                        {
                            component.ClaimOwnership();
                            component.Destroy();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public static void SpawnFloor(Vector3 position, Quaternion rotation, GameObject piece)
        {
            try
            {
                UnityEngine.Object.Instantiate<GameObject>(piece, position, rotation);
            }
            catch (Exception e)
            {
                DBG.blogWarning("SpawnFloor Failed:" + e);
            }
        }
    }
    #endregion
}
