using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SlimeRainBall : Bosses.MutantBoss.MutantSlimeBall
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/SlimeBall";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            CooldownSlot = -1;
        }

        public override void AI()
        {
            Projectile.tileCollide = --Projectile.ai[1] < 0;
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, 255, lightColor.A);
        }
    }
}