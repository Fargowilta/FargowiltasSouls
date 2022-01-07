using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Fused : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Fused");
            //Description.SetDefault("You're going out with a bang");
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "导火线");
            //Description.AddTranslation(GameCulture.Chinese, "你和爆炸有个约会");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Fused = true;

            if (player.buffTime[buffIndex] == 2)
            {
                player.immune = false;
                player.immuneTime = 0;
                int damage = (int)(Math.Max(player.statLife, player.statLifeMax) * 2.0 / 3.0);
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was blown to bits."), damage, 0, false, false, true);
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("FusedExplosion"), damage, 12f, Main.myPlayer);
            }
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            return true;
        }
    }
}