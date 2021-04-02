using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
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
		public static Texture2D LoadTextureRaw(byte[] file)
		{
			bool flag = Enumerable.Count<byte>(file) > 0;
			if (flag)
			{
				Texture2D texture2D = new Texture2D(2, 2);
				bool flag2 = ImageConversion.LoadImage(texture2D, file);
				if (flag2)
				{
					return texture2D;
				}
			}
			return null;
		}
		public static Sprite LoadSpriteFromTexture(Texture2D SpriteTexture, float PixelsPerUnit = 100f)
		{
			bool flag = SpriteTexture;
			Sprite result;
			if (flag)
			{
				result = Sprite.Create(SpriteTexture, new Rect(0f, 0f, (float)SpriteTexture.width, (float)SpriteTexture.height), new Vector2(0f, 0f), PixelsPerUnit);
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static Sprite LoadResouceIcon(string name)
		{
			return Util.LoadSpriteFromTexture(Util.LoadTextureRaw(Util.GetResource(Assembly.GetCallingAssembly(), "OdinPlus.Resources." + name + ".png")), 100f);
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

		#region  game
#region RollDice 

		public static float RollDice(this float val)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000));
			return val * UnityEngine.Random.value;
		}
		public static int RollDice(this int val)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000));
			return Mathf.FloorToInt(UnityEngine.Random.Range(0, val - 0.0001f));
		}
		public static int RollDice(this int val, int max)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000));
			return (int)(UnityEngine.Random.Range(val, max));
		}
		public static float RollDice(this float val, float max)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000));
			return (UnityEngine.Random.Range(val, max));
		}
		public static int seed = 0;
		public static float RollDices(this float val)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000)+seed);
			return val * UnityEngine.Random.value;
		}
		public static int RollDices(this int val)
		{
			UnityEngine.Random.InitState((int)((Time.time + val) * 1000)+seed);
			return Mathf.FloorToInt(UnityEngine.Random.Range(0, val - 0.0001f));
		}
		//100d6 大失败！！！！！

#endregion

		
		public static string GetRandomElement(this string[] array)
		{
			return array[array.Length.RollDices()];
		}
		public static Vector3 GetRandomLocation(this Vector3 pos, float range)
		{
			float seed = Time.time;
			UnityEngine.Random.InitState((int)seed);
			return pos + new Vector3(UnityEngine.Random.value, 0, UnityEngine.Random.value) * range;
		}
		public static GameObject FindObject(this GameObject parent, string name)
		{
			Component[] trs = parent.GetComponentsInChildren(typeof(Transform), true);
			foreach (Transform t in trs)
			{
				if (t.name == name)
				{
					return t.gameObject;
				}
			}
			return null;
		}

		#endregion  game

		#region HUMAN
		public static T CopyBroComponet<T, TU>(this Component comp, TU other) where T : Component
		{
			Type btype = comp.GetType().BaseType;
			IEnumerable<FieldInfo> finfos = btype.GetFields(bindingFlags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
				//DBG.blogWarning(finfo+" , "+finfo.GetType() + " , " +finfo.Name);
			}
			return comp as T;
		}
		public static T CopySonComponet<T, TU>(this Component comp, TU other) where T : Component
		{
			IEnumerable<FieldInfo> finfos = comp.GetType().GetFields(bindingFlags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
				//DBG.blogWarning(finfo+" , "+finfo.GetType() + " , " +finfo.Name);
			}
			return comp as T;
		}
		private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;

		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			Type type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match

			List<Type> derivedTypes = new List<Type>();
			Type derived = type.BaseType;
			while (derived != null)
			{
				if (derived == typeof(MonoBehaviour))
				{
					break;
				}
				derivedTypes.Add(derived);
				derived = derived.BaseType;
			}

			IEnumerable<PropertyInfo> pinfos = type.GetProperties(bindingFlags);

			foreach (Type derivedType in derivedTypes)
			{
				pinfos = pinfos.Concat(derivedType.GetProperties(bindingFlags));
			}

			pinfos = from property in pinfos
					 where !(type == typeof(Rigidbody) && property.Name == "inertiaTensor") // Special case for Rigidbodies inertiaTensor which isn't catched for some reason.
					 where !property.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
					 select property;
			foreach (var pinfo in pinfos)
			{
				if (pinfo.CanWrite)
				{
					if (pinfos.Any(e => e.Name == $"shared{char.ToUpper(pinfo.Name[0])}{pinfo.Name.Substring(1)}"))
					{
						continue;
					}
					try
					{
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}

			IEnumerable<FieldInfo> finfos = type.GetFields(bindingFlags);

			foreach (var finfo in finfos)
			{

				foreach (Type derivedType in derivedTypes)
				{
					if (finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}"))
					{
						continue;
					}
					finfos = finfos.Concat(derivedType.GetFields(bindingFlags));
				}
			}

			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}

			finfos = from field in finfos
					 where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
					 select field;
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}

			return comp as T;
		}

		public static T AddComponentcc<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent(toAdd.GetType()).GetCopyOf(toAdd) as T;
		}

		#endregion HUMAN
	}
	#region ZNET
	public static class ZNetExtensions
	{

		//Great work from jules!
		public enum ZNetInstanceType
		{
			Local,
			Client,
			Server
		}

		public static bool IsLocalInstance(this ZNet znet)
		{
			return znet.IsServer() && !znet.IsDedicated();
		}

		public static bool IsClientInstance(this ZNet znet)
		{
			return !znet.IsServer() && !znet.IsDedicated();
		}

		public static bool IsServerInstance(this ZNet znet)
		{
			return znet.IsServer() && znet.IsDedicated();
		}

		public static ZNetInstanceType GetInstanceType(this ZNet znet)
		{
			if (znet.IsLocalInstance())
			{
				return ZNetInstanceType.Local;
			}

			if (znet.IsClientInstance())
			{
				return ZNetInstanceType.Client;
			}

			return ZNetInstanceType.Server;
		}
	}

	#endregion ZNET


}

