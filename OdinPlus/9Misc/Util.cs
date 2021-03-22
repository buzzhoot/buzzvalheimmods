using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace OdinPlus
{
	public static class Util
	{

		#region LoadResource
		public static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
		private static Sprite LoadCustomTexture(String image)
		{
			Sprite cartography;
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string filepath = Path.Combine(path, image);
			Texture2D temp = LoadTexture(filepath);
			cartography = Sprite.Create(temp, new Rect(0, 0, 32, 32), Vector2.zero);
			return cartography;
		}

		private static Texture2D LoadTexture(string filepath)
		{
			bool flag = cachedTextures.ContainsKey(filepath);
			Texture2D result;
			if (flag)
			{
				result = cachedTextures[filepath];
			}
			else
			{
				Texture2D texture2D = new Texture2D(0, 0);
				texture2D.LoadRawTextureData(File.ReadAllBytes(filepath));
				result = texture2D;
			}
			return result;
		}

		public static byte[] GetResource(Assembly asm, string ResourceName)
		{
			Stream manifestResourceStream = asm.GetManifestResourceStream(ResourceName);
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
			return array;
		}
		#endregion LoadResource
		#region Reflection
public static object InvokePrivate(object instance, string name, object[] args = null)
		{
			MethodInfo method = instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);

			if (method == null)
			{
				DBG.blogWarning("Method " + name + " does not exist on type: " + instance.GetType());
				return null;
			}

			return method.Invoke(instance, args);
		}

		public static T GetPrivateField<T>(object instance, string name)
		{
			FieldInfo var = instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

			if (var == null)
			{
				DBG.blogWarning("Variable " + name + " does not exist on type: " + instance.GetType());
				return default(T);
			}

			return (T)var.GetValue(instance);
		}

		public static void SetPrivateField(object instance, string name, object value)
		{
			FieldInfo var = instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);

			if (var == null)
			{
				DBG.blogWarning("Variable " + name + " does not exist on type: " + instance.GetType());
				return;
			}

			var.SetValue(instance, value);
		}

		public static List<T> RemoveList<TU, T>(this List<T> instance, Dictionary<TU, T> other)
		{
			foreach (var item in other.Values)
			{
				if (instance.Contains(item))
				{
					instance.Remove(item);
				}
			}
			return instance;
		}
		public static Dictionary<T, TU> RemoveList<T, TU>(this Dictionary<T, TU> instance, Dictionary<T, TU> other)
		{
			foreach (var item in other.Keys)
			{
				if (instance.ContainsKey(item))
				{
					instance.Remove(item);
				}
			}
			return instance;
		}
		#endregion Reflection
		
		public static Vector3 GetRandomLocation(this Vector3 pos)
		{
			float seed = Time.time;
			UnityEngine.Random.InitState((int)seed);
			return pos+new Vector3(UnityEngine.Random.value,0,UnityEngine.Random.value)*50;
		}
	}
}

