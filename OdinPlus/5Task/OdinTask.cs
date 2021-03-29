using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using UnityEngine;


/* Game.instance.DiscoverClosestLocation("Vendor_BlackForest", Player.m_localPlayer.transform.position, "Merchant", 8);
Minimap.PinData pinData = Enumerable.First<Minimap.PinData>((List<Minimap.PinData>)Traverse.Create(Minimap.instance).Field("m_pins").GetValue(), (Minimap.PinData p) => p.m_type == Minimap.PinType.None && p.m_name == "");
 */


namespace OdinPlus
{

	public class OdinTask : MonoBehaviour
	{
		#region Var
		#region Data
		protected string[] m_tier0 = new string[0];
		protected string[] m_tier1 = new string[0];
		protected string[] m_tier2 = new string[0];
		protected string[] m_tier3 = new string[0];
		protected string[] m_tier4 = new string[0];
		protected string[] m_tier5 = new string[0];
		protected List<string[]> locList = new List<string[]>();
		public string locName;

		#endregion Data
		#region internal
		protected long owner;
		public string playerName;
		protected Action Init;
		protected bool loading = false;
		protected bool singleInit = true;
		#region Real Data
		public TaskManager.TaskType m_type;
		public string Id;
		protected bool m_pause = false;
		protected bool m_isInit = false;
		protected bool m_finished = false;
		protected bool m_isClear = false;
		protected ZoneSystem.LocationInstance location;
		#endregion Real Data

		#endregion internal
		#region in

		public int Key;
		public int Level;
		public bool isMain = false;
		#endregion in
		#region out
		public GameObject Reward;
		#endregion out
		#endregion Var

		#region Mono
		protected virtual void Update()
		{
			if (!m_isInit)
			{
				Init();
				return;
			}
			if (isLoaded() && !IsFinsih())
			{
				CheckTarget();
			}
			if (IsFinsih() && !m_isClear)
			{
				Clear();
			}
		}
		#endregion Mono

		#region Feature
		public virtual void Giveup()
		{
			Clear();
		}
		public virtual void Pause()
		{
			m_pause = !m_pause;
		}
		#endregion Feature

		#region internal Feature
		protected virtual void Begin()
		{
			Key = TaskManager.GameKey;
			locList = new List<string[]> { m_tier0, m_tier1, m_tier2, m_tier3, m_tier4, m_tier5 };
			if (singleInit)
			{
				Init = new Action(InitAll);
			}
			else
			{
				switch (Key)
				{
					case 0:
						Init = new Action(InitTire0);
						break;
					case 1:
						Init = new Action(InitTire1);
						break;
					case 2:
						Init = new Action(InitTire2);
						break;
					case 3:
						Init = new Action(InitTire3);
						break;
					case 4:
						Init = new Action(InitTire4);
						break;
					case 5:
						Init = new Action(InitAll);
						break;
				}
			}
			if (!SetLocation())
			{
				ZRoutedRpc.instance.InvokeRoutedRPC(owner, "RPC_CreateTaskFailed", new object[] { (int)m_type, locName });
				DBG.blogError(string.Format("Cannot Place Task :  {0} {1}", m_type, locName));
				Destroy(gameObject);
				return;
			}
			playerName=Tweakers.GetNameByPeerId(owner);
			ZRoutedRpc.instance.InvokeRoutedRPC(owner, "RPC_CreateTaskSucced", new object[] { Id, locName,location.m_position });
			DBG.blogWarning(string.Format("Placed Task :  {0}, {1},owner:{2} , {3}", m_type, locName,owner,playerName));
		}
		protected virtual bool SetLocation()
		{
			var list = locList[Key];
			int ind = list.Length.RollDice();
			locName = list[ind];
			if (LocationManager.FindClosestLocation(locName, Game.instance.GetPlayerProfile().GetCustomSpawnPoint(), out Id))
			{
				LocationManager.GetLocationInstance(Id, out location);
				gameObject.name = "Task" + Id;
				LocationManager.Remove(Id);
				return true;
			}
			return false;
		}
		protected virtual void InitAll() { }
		protected virtual void InitTire0() { }
		protected virtual void InitTire1() { }
		protected virtual void InitTire2() { }
		protected virtual void InitTire3() { }
		protected virtual void InitTire4() { }
		protected virtual void CheckTarget() { }
		public virtual void Finish()
		{
			//CHECK ONLINE
			if (ZNet.instance.GetPeerByPlayerName(playerName) == null)
			{
				DBG.blogWarning("task been taken by someone elese");
			}
			else
			{
				ZRoutedRpc.instance.InvokeRoutedRPC(ZNet.instance.GetPeerByPlayerName(playerName).m_uid, "RPC_ClientFinish", new object[] { Id });
			}
			m_finished = true;
		}
		public virtual void Clear()
		{
			m_isClear = true;
			Destroy(gameObject);
		}

		#endregion internal Feature

		#region Tool

		public void SetOwner(long sender)
		{
			owner = sender;
		}
		public bool IsOwner(long sender)
		{
			return sender == owner;
		}
		public bool IsFinsih()
		{
			return m_finished;
		}
		public bool IsPause()
		{
			return m_pause;
		}
		public bool isLoaded()
		{
			return IsPlayerInsideArea();
		}
		public bool isInsideArea(Vector3 position)
		{
			if (position.y > 3000f)
			{
				return false;
			}
			return Utils.DistanceXZ(position, location.m_position) < 100;//? 
		}
		public bool IsPlayerInsideArea()
		{
			foreach (ZDO allCharacterZDO in ZNet.instance.GetAllCharacterZDOS())
			{
				if (isInsideArea(allCharacterZDO.GetPosition()))
				{
					return true;
				}
			}
			return false;
		}
		#endregion Tool

		#region save load
		public bool Load(OdinData.TaskDataTable dat)
		{
			loading = true;

			owner = dat.owner;

			playerName=dat.playerName;

			Key = dat.Key;

			Level = dat.Level;

			m_type = dat.m_type;

			if (m_type == TaskManager.TaskType.Search)
			{
				Id = dat.Id;
				gameObject.name = "Task" + Id;
				return true;
			}

			m_isInit = dat.m_isInit;

			m_pause = dat.m_pause;

			m_finished = dat.m_finished;

			m_isClear = dat.m_isClear;

			Id = dat.Id;
			if (LocationManager.GetLocationInstance(Id, out location))
			{
				locName = location.m_location.m_prefabName;
				Init = new Action(InitAll);
				/* 				switch (Key)
								{

									case 0:
										Init = new Action(InitTire0);
										break;
									case 1:
										Init = new Action(InitTire1);
										break;
									case 2:
										Init = new Action(InitTire2);
										break;
									case 3:
										Init = new Action(InitTire3);
										break;
									case 4:
										Init = new Action(InitTire4);
										break;
								} */
				return true;

			}
			//-?
			DestroyImmediate(this.gameObject);
			return false;
		}
		public OdinData.TaskDataTable Save()
		{
			var dat = new OdinData.TaskDataTable()
			{
				owner = this.owner,

				playerName=this.playerName,

				Key = this.Key,

				Level = this.Level,

				m_type = this.m_type,

				m_isInit = this.m_isInit,

				m_pause = this.m_pause,

				m_finished = this.m_finished,

				m_isClear = this.m_isClear,

				Id = this.Id
			};
			return dat;
		}

		#endregion save load

	}
}