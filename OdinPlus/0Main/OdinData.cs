using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace OdinPlus
{
    class OdinData
    {
        public static int score;
        public static Dictionary<string, int> ItemSellValue = new Dictionary<string, int>();
        public static void init()
		{
			if (Plugin.CFG_ItemSellValue.Value == "") { return; }
			string[] l1 = Plugin.CFG_ItemSellValue.Value.Split(new char[] { ';' });
			for (int i = 0; i < l1.Length; i++)
			{
				string[] c = l1[i].Split(new char[] { ':' });
				try
				{
					ItemSellValue.Add(c[0], int.Parse(c[1]));
				}
				catch (Exception e)
				{

					DBG.blogWarning("CFG Error,Check Your ItemSellValue");
					DBG.blogWarning(e);
				}
			}

		}
        public static void AddScore(int s, Transform m_head)
        {
            score += s;
            //Transform m_head = Traverse.Create(Player.m_localPlayer).Field("m_head").GetValue<Transform>();
            Player.m_localPlayer.m_skillLevelupEffects.Create(m_head.position, m_head.rotation, m_head, 1f);
        }
        public static bool RemoveScore(int s)
        {
            if (score-s<0)
            {
                return false;
            }
            score -= s;
            return true;
        }
        public static void saveOdinData(string name)
        {
            string file = Path.Combine(Application.persistentDataPath,(name + ".odinplus"));            
            if (File.Exists(@file))
            {
                //add Backup
            }
            FileStream fileStream = new FileStream(@file, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(OdinData.score);
            binaryWriter.Flush();
            binaryWriter.Close();
            fileStream.Close();
        }
        public static void loadOdinData(string name)
        {
            string file = Path.Combine(Application.persistentDataPath, (name + ".odinplus"));
            if (File.Exists(@file))
            {
                FileStream fileStream = new FileStream(@file, FileMode.Open);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                score = binaryReader.ReadInt32();
                DBG.blogWarning("OdinDataLoaded:"+score);
                fileStream.Close();
                return;
            }
            else
            {
                DBG.blogWarning("Profile not exists");
            }
        }
    }
}
