using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderYoyoProj : ModProjectile
    {
        public bool yoyosSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Blender");
            // Vanilla values range from 3f(Wood) to 16f(Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;
            // Vanilla values range from 130f(Wood) to 400f(Terrarian), and defaults to 200f
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 750f;
            // Vanilla values range from 9f(Wood) to 17.5f(Terrarian), and defaults to 10f
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 25f;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Kraken);
            Projectile.width = 46;
            Projectile.height = 46;
            //yoyo ai
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;

            Projectile.extraUpdates = 1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        int soundtimer;
        public override void AI()
        {
            if (!yoyosSpawned && Projectile.owner == Main.myPlayer)
            {
                float localAI1 = 0;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].friendly && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == ModContent.ProjectileType<BlenderOrbital>())
                    {
                        localAI1 = Main.projectile[i].localAI[1] + MathHelper.Pi;
                        break;
                    }
                }

                int maxYoyos = 5;
                for (int i = 0; i < maxYoyos; i++)
                {
                    float radians = (360f / (float)maxYoyos) * i * (float)(Math.PI / 180);
                    Projectile yoyo = FargoSoulsUtil.NewProjectileDirectSafe(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlenderOrbital>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i, radians);
                    if (yoyo != null)
                    {
                        yoyo.localAI[0] = Projectile.identity;
                        yoyo.localAI[1] = localAI1;
                    }
                }

                yoyosSpawned = true;
            }

            if (soundtimer > 0)
                soundtimer--;

            if (Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.Blender>())
            {
                Projectile.damage = Main.player[Projectile.owner].GetWeaponDamage(Main.player[Projectile.owner].HeldItem);
                Projectile.knockBack = Main.player[Projectile.owner].GetWeaponKnockback(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.knockBack);
            }
        }

        public override void PostAI()
        {
            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 107 : 157);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 0.2f;
            Main.dust[d].velocity += Projectile.velocity * 0.8f;
            Main.dust[d].scale = 1.5f;
        }

        int hitcounter;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[Projectile.owner] = 6;

            if (Projectile.owner == Main.myPlayer)
            {
                Player player = Main.player[Projectile.owner];
                hitcounter++;
                if (player.ownedProjectileCounts[ProjectileID.BlackCounterweight] < 5)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Main.rand.NextVector2Circular(10, 10), ProjectileID.BlackCounterweight, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                if (hitcounter % 5 == 0)
                {
                    Vector2 velocity = Vector2.UnitY;
                    velocity = velocity.RotatedByRandom(Math.PI / 4);
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 newvel = velocity.RotatedBy(i * Math.PI / 4);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newvel * 8, ModContent.ProjectileType<BlenderPetal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }

            if (soundtimer == 0)
            {
                soundtimer = 15;
                SoundEngine.PlaySound(SoundID.Item22 with { Volume = 1.5f, Pitch = 1f }, Projectile.Center);
            }
        }

        //reduce tile hitbox
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 30;
            height = 30;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = Color.LightGreen * Projectile.Opacity * 0.5f;
                color27.A = 100;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}