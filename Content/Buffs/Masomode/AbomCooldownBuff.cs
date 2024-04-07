
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class AbomCooldownBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Abominable Cooldown");
            // Description.SetDefault("Cannot endure another attack yet");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.FargoSouls().AbominableWandRevived)
                player.buffTime[buffIndex] = 2;
        }
    }
}