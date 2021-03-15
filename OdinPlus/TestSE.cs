using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class SE_TEST : StatusEffect
{
    // Token: 0x060003AC RID: 940 RVA: 0x0001EC64 File Offset: 0x0001CE64
    public override void Setup(Character character)
    {
        base.Setup(character);
    }

    // Token: 0x060003AD RID: 941 RVA: 0x0001F52C File Offset: 0x0001D72C
    public override void Stop()
    {
        base.Stop();
        Player player = this.m_character as Player;
        if (!player)
        {
            return;
        }
        if (this.m_moreHealth > 0f)
        {
            player.SetMaxHealth(this.m_character.GetMaxHealth() + this.m_moreHealth, true);
            player.SetHealth(this.m_character.GetMaxHealth());
        }
        if (this.m_moreStamina > 0f)
        {
            player.SetMaxStamina(this.m_character.GetMaxStamina() + this.m_moreStamina, true);
        }
        this.m_upgradeEffect.Create(this.m_character.transform.position, Quaternion.identity, null, 1f);
    }

    // Token: 0x0400039B RID: 923
    [Header("Health")]
    public float m_moreHealth;

    // Token: 0x0400039C RID: 924
    [Header("Stamina")]
    public float m_moreStamina;

    // Token: 0x0400039D RID: 925
    public EffectList m_upgradeEffect = new EffectList();
}
