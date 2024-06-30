using FargowiltasSouls.Content.Buffs.Masomode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class ShadowDash : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 38;
            Projectile.height = 54;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MarkedforDeathBuff>(), 300);
        }
        public override void AI()
        {
            if (Projectile.owner < 0)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active || owner.dead || owner.FargoSouls().ShadowDashTimer <= 0)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            Projectile.timeLeft = 2;
        }
    }
}
