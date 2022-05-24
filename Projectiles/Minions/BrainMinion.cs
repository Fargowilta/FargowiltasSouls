using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class BrainMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain Proj");
            Main.projFrames[Projectile.type] = 11;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            //ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 70;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            //Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.dead) modPlayer.BrainMinion = false;
            if (modPlayer.BrainMinion) Projectile.timeLeft = 2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 11;
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center), 0.05f);

            Projectile.ai[0]++;
            Projectile.alpha = (int)(Math.Cos(Projectile.ai[0] * MathHelper.TwoPi / 180) * 122.5 + 122.5);
            if (Projectile.ai[0] == 180)
            {

                Projectile.Center = player.Center + Main.rand.NextVector2CircularEdge(300, 300);
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 8;
                Projectile.netUpdate = true;
                Projectile.ai[0] = 0;
            }
        }
    }
}