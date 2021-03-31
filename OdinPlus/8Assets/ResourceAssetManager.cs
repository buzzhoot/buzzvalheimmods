using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace OdinPlus
{
	public class ResourceAssetManager : MonoBehaviour
	{

		public static Dictionary<string, Sprite> OdinMeadsIcons = new Dictionary<string, Sprite>();

		public static string[] OdinMeadsName = { "ExpMeadS", "ExpMeadM", "ExpMeadL", "WeightMeadS", "WeightMeadM", "WeightMeadL", "InvisibleMeadS", "InvisibleMeadM", "InvisibleMeadL", "PickaxeMeadS", "PickaxeMeadM", "PickaxeMeadL", "BowsMeadS", "BowsMeadM", "BowsMeadL", "SwordsMeadS", "SwordsMeadM", "SwordsMeadL", "SpeedMeadsL", "AxeMeadS", "AxeMeadM", "AxeMeadL" };

		private void Awake()
		{
			LoadMeadsIcons();
		}
		public static void LoadMeadsIcons()
		{
			foreach (var name in OdinMeadsName)
			{
				AddIcon(name,0);
			}
		}
		public static void AddIcon(string name, int list)
		{
			Sprite a = Util.LoadSpriteFromTexture(Util.LoadTextureRaw(Util.GetResource(Assembly.GetCallingAssembly(), "OdinPlus.Resources." + name + ".png")), 100f);
			OdinMeadsIcons.Add(name, a);
		}
	}
}