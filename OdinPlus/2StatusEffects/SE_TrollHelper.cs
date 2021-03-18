using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OdinPlus
{
	class SE_TrollHelper : StatusEffect
	{
		public override void Setup(Character character)
		{
			base.Setup(character);
            PetManager.SummonTroll("Troll");
		}
		public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
		}
	}
}
