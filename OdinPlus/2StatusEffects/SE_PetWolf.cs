
namespace OdinPlus
{
	class SE_PetWolf : StatusEffect
	{
		public override void Setup(Character character)
		{
			base.Setup(character);
            PetManager.SummonWolf();
		}
		public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
		}
	}
}
