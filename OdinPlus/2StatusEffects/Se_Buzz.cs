namespace OdinPlus
{
	class Se_Buzz : StatusEffect
	{
		public float speedModifier = 2;
		private float OriginalSpeed;
		public override void Setup(Character character)
		{
			base.Setup(character);
			OriginalSpeed = character.m_runSpeed;
			character.m_runSpeed = character.m_runSpeed * speedModifier;
		}
		public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
		}
		public override void OnDestroy() {
			m_character.m_runSpeed=OriginalSpeed;
		}
		public override void Stop()
		{
			m_character.m_runSpeed=OriginalSpeed;
			base.Stop();
		}
	}
}