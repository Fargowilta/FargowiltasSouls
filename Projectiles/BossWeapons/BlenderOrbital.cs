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
    internal class BlenderOrbital : ModProjectile
    {
        public int Counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Blender");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;

            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;

            Projectile.aiStyle = -1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        int soundtimer;
        int dieTimer;
        public override void AI()
        {
            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.localAI[0], ModContent.ProjectileType<BlenderYoyoProj>());
            if (byIdentity == -1)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    if (++dieTimer > 30)
                        Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile proj = Main.projectile[byIdentity];

                //rotation mumbo jumbo
                float distanceFromPlayer = 150 + 150 * (1 - (float)Math.Cos(Projectile.localAI[1])); // + Projectile.ai[0] * 32;

                Vector2 offset = new Vector2(distanceFromPlayer, 0f).RotatedBy(Projectile.ai[1]);

                Vector2 oldCenter = Projectile.Center - offset;
                Vector2 desiredPos = proj.Center + offset;
                Projectile.position += Vector2.Lerp(oldCenter, proj.Center, 0.05f) - oldCenter;
                Projectile.position += (Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition) / 2;
                Projectile.Center = Vector2.Lerp(Projectile.Center, desiredPos, 0.05f);

                float rotation = MathHelper.Pi / 20 / Projectile.MaxUpdates * 0.75f;
                Projectile.ai[1] += rotation;
                if (Projectile.ai[1] > MathHelper.Pi)
                {
                    Projectile.ai[1] -= MathHelper.TwoPi;
                    Projectile.netUpdate = true;
                }

                Projectile.localAI[1] += MathHelper.TwoPi / 120 / Projectile.MaxUpdates; // * (0.5f + 1.5f * Projectile.ai[0] / 5);
                if (Projectile.localAI[1] > MathHelper.Pi)
                    Projectile.localAI[1] -= MathHelper.TwoPi;

                Projectile.damage = (int)(proj.damage * 0.5);
                Projectile.knockBack = proj.knockBack;
            }

            Projectile.timeLeft++;
            Projectile.rotation += 0.25f / Projectile.MaxUpdates;

            if (soundtimer > 0)
                soundtimer--;

            dieTimer = 0;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[Projectile.owner] = 6;

            /*Vector2 velocity = Vector2.Normalize(Projectile.Center - target.Center) * 10;
            int proj2 = ModContent.ProjectileType<BlenderProj3>();
            Projectile.NewProjectile(Projectile.Center, velocity, proj2, Projectile.damage, Projectile.knockBack, Main.myPlayer);*/

            if (soundtimer == 0)
            {
                soundtimer = 15;
                SoundEngine.PlaySound(SoundID.Item22 with { Volume = 1.5f, Pitch = 1f }, Projectile.Center);
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