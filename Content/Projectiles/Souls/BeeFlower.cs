using FargowiltasSouls.Content.Items;
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
            Projectile.scale = 1;
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
                    if (Projectile.ai[2] == 1) // life force
                    {
                        Main.LocalPlayer.FargoSouls().LifeBeetleDuration = 60 * 10;
                        Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                        Main.LocalPlayer.FargoSouls().HealPlayer(10);
                    }

                    FargoGlobalItem.OnRetrievePickup(Main.LocalPlayer);

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
            int damage = (int)(Projectile.damage * 0.75f);
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (pos - Projectile.Center) / 12,
                    Main.LocalPlayer.beeType(), Main.LocalPlayer.beeDamage(damage), Main.LocalPlayer.beeKB(Projectile.knockBack), Main.LocalPlayer.whoAmI);
                if (p != Main.maxProjectiles)
                {
                    Main.projectile[p].DamageType = Projectile.DamageType;
                    if (Projectile.ai[2] == 1)
                        Main.projectile[p].extraUpdates = 10;
                }
                    
            }
        }
    }
}