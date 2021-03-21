using System;
using System.Collections.Generic;
//using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
//using BepInEx.Configuration;


namespace AllTameable
{

	public class ConfigManager : MonoBehaviour
	{
		#region var
		public string CfgName;
		private GUIStyle button;
		private GUIStyle scroll;
		private GUIStyle debugscroll;
		private GUIStyle normalText;
		private GUIStyle debugText;
		private GUIStyle placeholder;
		private Rect SetupWindowRect;
		//private float linesCount;//add
		public object obj;
		private Vector2 scrollPosition = Vector2.zero;
		private Vector2 scrollPosition2 = Vector2.zero;
		public static Transform Root;
		#region GUI Settings
		private float width = 600;
		private float height = 1000;
		public int noramalFontSize = 20;
		public int topicFontSzie = 25;
		public string title;
		public string debugInfo;
		#endregion GUI Settings
		#region util var
		BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance;

		#endregion util var
		#endregion var

		#region Mono
		private void Awake()
		{
			Root = this.gameObject.transform;
			//SM_Root = new GameObject("ConfigManager").transform;
			//SM_Root.SetParent(Plugin.Root.transform);
			//SM_Root.gameObject.SetActive(false);
			SetupWindowRect = new Rect(0, 0, width, height);
		}
		private void OnGUI()
		{
			SetupGUI();
			GUI.backgroundColor = Color.black;
			SetupWindowRect = GUI.Window(9107, SetupWindowRect, new GUI.WindowFunction(SetupWindow), title);

		}
		#endregion Mono

		#region Tool
		private void ConstrutGUI()
		{
			PropertyInfo[] finfos = obj.GetType().GetProperties(bindingFlags);
			foreach (PropertyInfo finfo in finfos)
			{
				if (finfo.PropertyType == typeof(bool))
				{
					DrawBoolToogle(finfo);
					//return;
				}
				if (finfo.PropertyType == typeof(int))
				{
					DrawIntField(finfo);
					//return;
				}
				if (finfo.PropertyType == typeof(float))
				{
					DrawFloatField(finfo);
					//return;
				}
				if (finfo.PropertyType == typeof(string))
				{
					DrawStringField(finfo);
					//return;
				}
			}
		}
		//Gui
		private void DrawStringField(PropertyInfo fieldInfo)
		{
			string result = (string)fieldInfo.GetValue(obj).ToString();
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldInfo.Name, GUILayout.Width((width - 50) * 1 / 3));
			result = GUILayout.TextField(result, GUILayout.Width((width - 50) * 2 / 3));
			GUILayout.EndHorizontal();
			fieldInfo.SetValue(obj, result);
		}
		private void DrawIntField(PropertyInfo fieldInfo)
		{
			string result = (string)fieldInfo.GetValue(obj).ToString();
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldInfo.Name, GUILayout.Width((width - 50) * 1 / 3));
			result = GUILayout.TextField(result, GUILayout.Width((width - 50) * 2 / 3));
			GUILayout.EndHorizontal();
			result = Single.Parse(result).ToString();
			//result = Regex.Replace(result, "[^0-9]", "");
			int a;
			if (int.TryParse(result, out a))
			{
				fieldInfo.SetValue(obj, a);
			}
		}
		private void DrawFloatField(PropertyInfo fieldInfo)
		{
			string result = (string)fieldInfo.GetValue(obj).ToString();
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldInfo.Name, GUILayout.Width((width - 50) * 1 / 3));
			if(fieldInfo.Name=="pregnancyChance"){result = GUILayout.TextField(Single.Parse(result).ToString("0.00"), GUILayout.Width((width - 50) * 2 / 3));}
			else{result = GUILayout.TextField(result, GUILayout.Width((width - 50) * 2 / 3));}
			
			GUILayout.EndHorizontal();
			float a;
			if (float.TryParse(result, out a))
			{
				fieldInfo.SetValue(obj, a);
			}
		}
		private void DrawBoolToogle(PropertyInfo fieldInfo)
		{
			bool result = (bool)fieldInfo.GetValue(obj);
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldInfo.Name, GUILayout.Width((width - 50) * 1 / 3));
			result = GUILayout.Toggle(result, result ? "on" : "off", GUILayout.Width((width - 50) * 1 / 3));
			GUILayout.EndHorizontal();
			fieldInfo.SetValue(obj, result);
		}

		#endregion Tool

		#region Feature
		private void SetupWindow(int windowid)
		{
			GUI.DragWindow(new Rect(0f, 0f, width, 30f));
			GUILayout.BeginArea(new Rect(20f, 80f, width, height - 30));
			GUILayout.Label("", GUILayout.Height(30));
			GUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(30));
			if (GUILayout.Button("Add", button, new GUILayoutOption[0])) { Add(); }
			if (GUILayout.Button("Remove", button, new GUILayoutOption[0])) { Remove(); }
			if (GUILayout.Button("Replace", button, new GUILayoutOption[0])) { Replace(); }
			if (GUILayout.Button("Get", button, new GUILayoutOption[0])) { Get(); }
			GUILayout.EndHorizontal();

			scrollPosition = GUILayout.BeginScrollView(scrollPosition, debugscroll);
			GUILayout.TextArea(debugInfo, debugText, GUILayout.Height(300));
			GUILayout.EndScrollView();

			scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, scroll);
			GUILayout.BeginHorizontal();
			GUILayout.Label("name", GUILayout.Width((width - 50) * 1 / 3));
			CfgName = GUILayout.TextField(CfgName, GUILayout.Width((width - 50) * 2 / 3));
			GUILayout.EndHorizontal();
			ConstrutGUI();

			GUI.EndScrollView();

			GUILayout.EndArea();
		}
		private void SetupGUI()
		{
			button = new GUIStyle(GUI.skin.button);
			button.fontSize = noramalFontSize;
			button.fixedWidth = (width - 100) / 4;
			button.fixedHeight = 30;

			normalText = new GUIStyle(GUI.skin.label);
			normalText.fontSize = noramalFontSize;

			debugText = new GUIStyle(GUI.skin.label);
			debugText.fontSize = noramalFontSize;
			debugText.normal.textColor = Color.yellow;
			debugText.focused.textColor = Color.yellow;
			debugText.richText = true;

			scroll = new GUIStyle(GUI.skin.scrollView);
			scroll.fontSize = noramalFontSize;
			scroll.fixedWidth = width - 20;
			scroll.fixedHeight = height - 20;

			debugscroll = new GUIStyle(GUI.skin.scrollView);
			debugscroll.fontSize = noramalFontSize;
			debugscroll.fixedWidth = width - 20;
			debugscroll.fixedHeight = 120;

			placeholder = new GUIStyle(GUI.skin.box);
		}
		private void Add()
		{
			Plugin.CfgMangerAdd();
		}
		private void Replace()
		{
			Plugin.cfgMangerReplace();
		}
		private void Remove()
		{
			Plugin.CfgMangerRemove();
		}
		private void Get()
		{
			Plugin.CfgMangerGet();
		}
		#endregion Feature

	}
}