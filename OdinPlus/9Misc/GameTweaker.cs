using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
namespace OdinPlus
{
	public static class Tweakers
	{
		public static List<string> TaskHints = new List<string>();
		public static void addHints(string text)
		{
			var m_knownTexts = Traverse.Create(Player.m_localPlayer).Field<Dictionary<string, string>>("m_knownTexts").Value;
			TaskHints.Add(text);
			while (TaskHints.Count > 50)
			{
				TaskHints.RemoveAt(0);
			}
			m_knownTexts["Quest Hints"] = string.Join("\n", TaskHints.ToArray());
		}

		public static Humanoid ChangeSpeed(this Humanoid humanoid, float speed)
		{
			humanoid.m_speed = speed;
			return humanoid;
		}
		public static void TaskHintHugin(string messageName, string messageText)
		{
			Tutorial.TutorialText tutorialText = new Tutorial.TutorialText
			{
				m_label = "Quest Hints",
				m_name = messageName + DateTimeOffset.Now.Millisecond + DateTimeOffset.Now.Day + DateTimeOffset.Now.Hour + DateTimeOffset.Now.Minute,
				m_text = messageText,
				m_topic = "Quest Hints"
			};
			HuginSays(tutorialText.m_name, tutorialText.m_topic, tutorialText.m_text, tutorialText.m_label);
			addHints(messageName + "\n" + tutorialText.m_text + "\n");
		}
		public static void TaskTopicHugin(string messageName, string messageText)
		{
			Tutorial.TutorialText tutorialText = new Tutorial.TutorialText
			{
				m_label = "Quest List",
				m_name = messageName + DateTimeOffset.Now.Millisecond + DateTimeOffset.Now.Day + DateTimeOffset.Now.Hour + DateTimeOffset.Now.Minute,
				m_text = messageText,
				m_topic = "Quest List"
			};
			HuginSays(tutorialText.m_name, tutorialText.m_topic, tutorialText.m_text, tutorialText.m_label);
			var m_knownTexts = Traverse.Create(Player.m_localPlayer).Field<Dictionary<string, string>>("m_knownTexts").Value;
			if (m_knownTexts.ContainsKey(tutorialText.m_topic))
			{
				m_knownTexts[tutorialText.m_topic] = tutorialText.m_text;
				return;
			}
			m_knownTexts.Add(tutorialText.m_topic, tutorialText.m_text);
			return;

		}
		public static void HuginSays(string key, string topic, string text, string label)
		{
			//Traverse.Create(Tutorial.instance).Method("SpawnRaven", new object[]{ key,topic,text,label}).GetValue();
			if (!Raven.IsInstantiated())
			{
				UnityEngine.Object.Instantiate<GameObject>(Tutorial.instance.m_ravenPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			}
			Raven.AddTempText(key, topic, text, label, false);
		}
		public static string GetTransName(this string str)
		{
			return ObjectDB.instance.GetItemPrefab(str).GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
		}
		public static string GetLocal(this string str)
		{
			return Localization.instance.Localize(str);
		}
		public static string DepakVector2i(Vector2i v2i)
		{
			return v2i.x.ToString() + "_" + v2i.y.ToString();
		}
		public static Vector2i Pak(string str)
		{
			Vector2i val;
			string[] a = str.Split(new char[] { '_' });
			val.x = int.Parse(a[0]);
			val.y = int.Parse(a[1]);
			return val;
		}
		public static bool HasObject(string name, Vector3 pos, float range = 10)
		{
			Collider[] array = Physics.OverlapBox(pos, new Vector3(range, range, range));
			if (array == null)
			{
				return false;
			}
			foreach (var item in array)
			{
				var s = item.gameObject;
				if (s.name == name)
				{
					return true;
				}
				var f = s.transform.parent;
				if (f == null) { break; }
				if (f.name == name)
				{
					return true;
				}
				var g = f.transform.parent;
				if (g == null) { break; }
				if (g.name == name)
				{
					return true;
				}
			}
			return false;
		}
		public static ItemDrop.ItemData GetItemData(string name)
		{
			return ObjectDB.instance.GetItemPrefab(name).GetComponent<ItemDrop>().m_itemData;
		}
		public static GameObject ValSpawn(string name, Vector3 pos, bool removeClone = false)
		{
			var a = GameObject.Instantiate(ZNetScene.instance.GetPrefab(name), pos, Quaternion.identity);
			if (removeClone)
			{
				if (a.name.Contains("(Clone)"))
				{
					a.name = a.name.Substring(a.name.Length - 6, 7);
				}
			}
			return a;
		}
		#region Distance

		public static bool isInsideArea(Vector3 position, Vector3 m_position, float m_range)
		{
			if (position.y > 3000f)
			{
				return false;
			}
			return Utils.DistanceXZ(position, m_position) < m_range;
		}
		public static string GetNameByPeerId(long uid)
		{
			var peers = Traverse.Create(ZNet.instance).Field<List<ZNetPeer>>("m_peers").Value;
			foreach (var peer in peers)
			{
				if (peer.m_uid == uid)
				{
					return peer.m_playerName;
				}
			}
			DBG.blogWarning("Cant find player name");
			return null;
		}
		#endregion Distance
		#region Vector2i

		public static string Pak(this Vector2i v2i)
		{
			return v2i.x.ToString() + "_" + v2i.y.ToString();
		}
		public static Vector2i ToV2I(this string str)
		{
			Vector2i val;
			string[] a = str.Split(new char[] { '_' });
			val.x = int.Parse(a[0]);
			val.y = int.Parse(a[1]);
			return val;
		}
		#endregion Vector2i
	}

}