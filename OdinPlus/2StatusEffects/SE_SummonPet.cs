
namespace OdinPlus
{
	class SE_SumonPet : StatusEffect
	{
		public string PetName;
		public override void Setup(Character character)
		{
			base.Setup(character);
            PetManager.SummonPet(PetName);
		}
		public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
		}
	}
}
