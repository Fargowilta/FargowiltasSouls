using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class BerserkerInstallCDBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Berserker Cooldown");
            // Description.SetDefault("You cannot go berserk yet");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //avoid double dipping when they overlap
            if (!player.HasBuff(ModContent.BuffType<BerserkerInstallBuff>()))
                BerserkerInstallBuff.DebuffPlayerStats(player);
        }
    }
}