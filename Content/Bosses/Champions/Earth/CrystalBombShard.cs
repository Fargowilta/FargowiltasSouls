using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class CrystalBombShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_920";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Shard");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.QueenSlimeMinionBlueSpike];
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.QueenSlimeMinionBlueSpike);
            AIType = ProjectileID.QueenSlimeMinionBlueSpike;
            Projectile.scale *= 1.5f;
            Projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Chilled, 180);
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}