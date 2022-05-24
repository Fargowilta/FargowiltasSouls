using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class BlenderProj3 : ModProjectile
    {
        public int Counter = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Blender");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 19;
            Projectile.height = 19;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 0.75f;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.Pi;
            Counter++;

            const int aislotHomingCooldown = 0;
            const int homingDelay = 30;
            const float desiredFlySpeedInPixelsPerFrame = 30;
            const float amountOfFramesToLerpBy = 30; // minimum of 1, please keep in full numbers even though it's a float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                NPC n = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 1000));
                if (n != null)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(n.Center) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            int proj2 = ModContent.ProjectileType<BlenderSpray>(); //374;

            /*if (Projectile.owner == Main.myPlayer)
            {
                if (Main.rand.NextBool())
                {
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0f, 10f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 10f, 0f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 0f, -10f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -10f, 0f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
                else
                {
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 8f, 8f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -8f, -8f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, 8f, -8f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, -8f, 8f, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }*/

            for (int i = 0; i < 2; i++)
            {
                if (!Main.dedServ)
                    Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + (float)Main.rand.Next(Projectile.width),
                    Projectile.position.Y + (float)Main.rand.Next(Projectile.height)),
                    Projectile.velocity,
                    ModContent.Find<ModGore>(Mod.Name, $"Tentacle{i}").Type,
                    Projectile.scale);
            }

            Vector2 velocity = Vector2.UnitY;
            velocity = velocity.RotatedByRandom(Math.PI / 4);
            for (int i = 0; i < 8; i++)
            {
                Vector2 newvel = velocity.RotatedBy(i * Math.PI / 4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newvel * 4, ModContent.ProjectileType<BlenderPetal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[Projectile.owner] = 6;
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

            SpriteEffects effects = SpriteEffects.None;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}