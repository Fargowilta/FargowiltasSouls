using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class BeeFlower : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 15;
            Projectile.penetrate = 1;
            Projectile.light = 1;
        }
        public override bool? CanDamage() => Projectile.frame == Main.projFrames[Projectile.type] - 1; //only damage when fully grown
        public override void AI()
        {
            if (Projectile.frame < Main.projFrames[Projectile.type] - 1) //petalinate
            {
                if (++Projectile.frameCounter % 60 == 0)
                {
                    Projectile.frame++;
                }
            }
            else
            {
                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox))
                {
                    Main.LocalPlayer.AddBuff(BuffID.Honey, 60 * 15);
                    BeeSwarm();

                    Projectile.Kill();
                }
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BeeSwarm();
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.LiquidsHoneyWater, Projectile.Center);
        }

        public void BeeSwarm()
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (pos - Projectile.Center) / 12,
                    Main.LocalPlayer.beeType(), Main.LocalPlayer.beeDamage((int)(Projectile.damage * 0.75f)), Main.LocalPlayer.beeKB(Projectile.knockBack), Main.LocalPlayer.whoAmI);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].DamageType = Projectile.DamageType;
            }
        }
    }
}