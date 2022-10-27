using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Fused : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fused");
            Description.SetDefault("You're going out with a bang");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "导火线");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你和爆炸有个约会");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Fused = true;

            if (player.buffTime[buffIndex] == 2)
            {
                player.immune = false;
                player.immuneTime = 0;
                int damage = (int)(Math.Max(player.statLife, player.statLifeMax) * 2.0 / 3.0);
                player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.FargowiltasSouls.DeathMessage.Fused", player.name)), damage, 0, false, false, true);
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Masomode.FusedExplosion>(), damage, 12f, Main.myPlayer);
            }
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}