using FargowiltasSouls.Buffs.Masomode;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs
{
    public class BerserkerInstall : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Berserker Install");
            Description.SetDefault("'Get them! Berserker!'");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public static void DebuffPlayerStats(Player player)
        {
            player.endurance -= 0.30f;
            player.statDefense -= 30;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            DebuffPlayerStats(player);

            player.GetModPlayer<FargoSoulsPlayer>().Berserked = true;

            player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed += 0.50f;
            player.GetDamage(DamageClass.Generic) += 0.20f;
            player.GetCritChance(DamageClass.Generic) += 20;
            player.moveSpeed += 0.20f;

            player.hasMagiluminescence = true;
            player.noKnockback = true;

            if (!player.controlLeft && !player.controlRight)
            {
                if (player.velocity.X > 0)
                    player.controlRight = true;
                else if (player.velocity.X < 0)
                    player.controlLeft = true;
                else if (player.direction > 0)
                    player.controlRight = true;
                else
                    player.controlLeft = true;
            }

            if (player.buffTime[buffIndex] > 2)
                player.GetModPlayer<FargoSoulsPlayer>().NoMomentum = true;

            if (player.buffTime[buffIndex] == 2)
            {
                int stunDuration = 150; //2.5sec
                player.AddBuff(ModContent.BuffType<BerserkerInstallCD>(), stunDuration);
                player.AddBuff(ModContent.BuffType<Stunned>(), stunDuration);
            }
        }
    }
}