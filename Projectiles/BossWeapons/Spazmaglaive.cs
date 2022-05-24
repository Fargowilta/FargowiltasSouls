using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class Spazmaglaive : ModProjectile
    {
        bool empowered = false;
        bool hitSomething = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spazmaglaive");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(empowered);
            writer.Write(hitSomething);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            empowered = reader.ReadBoolean();
            hitSomething = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.light = 0.4f;
            Projectile.tileCollide = false;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == ModContent.ProjectileType<Retiglaive>())
            {
                empowered = true;
                Projectile.ai[0] = 0;
            }
            else if (Projectile.ai[0] == ModContent.ProjectileType<Spazmaglaive>())
            {
                Projectile.ai[0] = 0;
            }
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
            }
            Projectile.rotation += Projectile.direction * -0.4f;
            Projectile.ai[0]++;

            const int maxTime = 45;
            Vector2 DistanceOffset = new Vector2(950 * (float)Math.Sin(Projectile.ai[0] * Math.PI / maxTime), 0).RotatedBy(Projectile.velocity.ToRotation());
            DistanceOffset = DistanceOffset.RotatedBy(Projectile.ai[1] - (Projectile.ai[1] * Projectile.ai[0] / (maxTime / 2)));
            Projectile.Center = Main.player[Projectile.owner].Center + DistanceOffset;
            if (Projectile.ai[0] > maxTime)
                Projectile.Kill();

            if (empowered && Projectile.ai[0] == maxTime / 2 && Projectile.owner == Main.myPlayer) //star spray on the rebound
            {
                Vector2 baseVel = Main.rand.NextVector2CircularEdge(1, 1);
                const int max = 24;
                for (int i = 0; i < max; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item105 with { Pitch = -0.3f }, Projectile.Center);
                    Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / max);
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newvel / 2, ModContent.ProjectileType<DarkStarFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (p < Main.maxProjectiles)
                    {
                        Main.projectile[p].DamageType = DamageClass.Melee;
                        Main.projectile[p].timeLeft = 30;
                        Main.projectile[p].netUpdate = true;
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(new Vector2(targetHitbox.Center.X, targetHitbox.Center.Y)) < 150; //big circular hitbox because otherwise it misses too often
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);

            if (!hitSomething)
            {
                hitSomething = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                    Vector2 baseVel = Main.rand.NextVector2CircularEdge(1, 1);
                    float ai0 = 78;//empowered ? 120 : 78;
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 5);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, newvel, ModContent.ProjectileType<SpazmaglaiveExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0, target.whoAmI);
                    }
                    /*if (empowered)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item105 with { Pitch = -0.3f }, Projectile.Center);
                            Vector2 newvel = baseVel.RotatedBy(i * MathHelper.TwoPi / 12);
                            int p = Projectile.NewProjectile(target.Center, newvel/2, ModContent.ProjectileType<DarkStarFriendly>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, target.whoAmI);
                            if(p < 1000)
                            {
                                Main.Projectile[p].magic = false;
                                Main.Projectile[p].melee = true;
                                Main.Projectile[p].timeLeft = 30;
                                Main.Projectile[p].netUpdate = true;
                            }
                        }
                    }*/
                }
                Projectile.netUpdate = true;
            }
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

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.33f)
            {
                int max0 = Math.Max((int)i - 1, 0);
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], (1 - i % 1));
                if (i < 4)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(texture2D13, center + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, Projectile.oldRot[(int)i], origin2, Projectile.scale, SpriteEffects.None, 0);
                }
                if (empowered)
                {
                    Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/BossWeapons/HentaiSpearSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Color glowcolor = new Color(142, 250, 176);
                    glowcolor = Color.Lerp(glowcolor, Color.Transparent, 0.6f);
                    float glowscale = Projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Main.EntitySpriteDraw(glow, center + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, glowcolor, 0, glow.Size() / 2, glowscale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}