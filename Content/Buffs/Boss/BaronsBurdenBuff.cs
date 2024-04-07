using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class BaronsBurdenBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baron's Burden");
            // Description.SetDefault("Gravity heavily increased outside water\nBreathe water instead of air\nGet back in!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gills = true;
            player.ignoreWater = true;
            if (!player.wet)
            {
                player.velocity.Y += 0.4f;
                player.velocity.X *= 0.9f;
                if (player.statLife > 10)
                {
                    player.lifeRegen = -1 * 90;
                }
            }
        }
    }
}