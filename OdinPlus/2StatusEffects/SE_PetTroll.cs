
namespace OdinPlus
{
	class SE_PetTroll : StatusEffect
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
