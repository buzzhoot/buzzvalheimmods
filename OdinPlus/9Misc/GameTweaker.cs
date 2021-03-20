using System;
using UnityEngine;
namespace OdinPlus
{
	public static class Tweakers
	{
		public static Humanoid ChangeSpeed(this Humanoid humanoid,float speed)
		{
			humanoid.m_speed = speed;
			return humanoid;
		}
	}
}