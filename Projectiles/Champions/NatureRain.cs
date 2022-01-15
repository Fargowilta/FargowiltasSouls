using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class NatureRain : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_239";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Nature Rain");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RainNimbus);
            aiType = ProjectileID.RainNimbus;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.5f, 0.75f, 1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 300);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}