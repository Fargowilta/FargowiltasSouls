using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class BloodDrinkerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Drinker");
            // Description.SetDefault("30% increased damage");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.30f;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.FargoSouls().BloodDrinker = true;
        }
    }
}