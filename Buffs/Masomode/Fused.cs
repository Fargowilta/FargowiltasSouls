using Microsoft.Xna.Framework;
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
            DisplayName.SetDefault("Fused");
            Description.SetDefault("You're going out with a bang");
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
            DisplayName.AddTranslation(GameCulture.Chinese, "导火线");
            Description.AddTranslation(GameCulture.Chinese, "你带着爆炸外出");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Fused = true;

            if (player.buffTime[buffIndex] == 2)
            {
                player.immune = false;
                player.immuneTime = 0;
                //Projectile.NewProjectile(player.Center, Vector2.Zero, ProjectileID.Explosives, 400, 10f, Main.myPlayer);
                int damage = (player.statLifeMax2 / 2);
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("FuseBomb"), damage / 4, 4f, Main.myPlayer);
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was blown to bits."), damage, 0, false, false, true);
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }
    }
}
