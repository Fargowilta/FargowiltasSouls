﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using System.IO;
using Mono.Cecil;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{

    public class SlimeSlingingSlasherProj : ModProjectile
    {
        public bool Swinging = false;
        public bool SwingHit = false;
        public bool FirstSwing = true;
        public float SlashOpacity = 0f;
        public override string Texture => "FargowiltasSouls/Content/Items/Weapons/SwarmDrops/SlimeSlingingSlasher";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 63;
            Projectile.height = 63;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.light = 2f;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
        }
        public ref float ItemTime => ref Projectile.ai[0];
        public ref float FreezeTime => ref Projectile.ai[1];
        public ref float SwingRotation => ref Projectile.ai[2];
        public ref float ProjectileCheck => ref Projectile.localAI[0];
        public override bool? CanDamage() => Swinging ? base.CanDamage() : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SwingHit = true;

            target.AddBuff(BuffID.Slimed, 120);
            SoundEngine.PlaySound(SoundID.Item17);
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Main.rand.NextVector2FromRectangle(target.Hitbox), Vector2.Zero, ModContent.ProjectileType<Slimesplosion>(), damageDone, 1f, Projectile.owner);

            FreezeTime = 4;
        }
        public override void AI()
        {
            void SlimeProjs(Player player)
            {
                for (int i = -7; i < 7; i++)
                {
                    int j = i * Projectile.direction;
                    Vector2 vel = (Projectile.rotation + MathHelper.PiOver2 * j / 10f).ToRotationVector2() * 20f;
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), player.RotatedRelativePoint(player.MountedCenter, true) + Vector2.Normalize(vel) * Projectile.height * (1f), vel, ModContent.ProjectileType<SlimeBallHoming>(), Projectile.originalDamage, Projectile.knockBack, player.whoAmI);
                }
                ProjectileCheck = 1;
            }
            Player player = Main.player[Projectile.owner];
            const int SwingTime = 60;
            float progress = ItemTime / SwingTime;
            progress %= 1;

            if (FreezeTime > 0)
                FreezeTime--;
            float increment = player.GetAttackSpeed(DamageClass.Melee) + player.FargoSouls().AttackSpeed - 1f;
            ItemTime += increment * (FreezeTime <= 0 ? 1f : 0.25f);

            const float swingDuration = 0.2f;
            const float pauseDuration = 0.15f;

            const float prepEnd = 1 - (2 * swingDuration + pauseDuration);
            const float firstSwingEnd = prepEnd + swingDuration;
            const float pauseEnd = firstSwingEnd + pauseDuration;

            float maxAngle = MathHelper.PiOver2 * 1.5f;
            bool flip = false;

            if (progress < prepEnd)
            {
                SwingRotation = MathHelper.Lerp(SwingRotation, -maxAngle, 0.2f);
                Swinging = false;

                float animProgress = progress / prepEnd;
                if (animProgress < 0.2f && !FirstSwing)
                    flip = true;
                Projectile.ResetLocalNPCHitImmunity();
            }
            else if (progress < firstSwingEnd)
            {
                float swingProgress = (progress - prepEnd) / swingDuration;
                if (swingProgress > 0.3f)
                {
                    if (!Swinging)
                    {
                        Swinging = true;
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                        
                        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                        {
                            Projectile.oldPos[i] = Vector2.Zero;
                        }
                        ProjectileCheck = 0;
                    }
                }
                if (swingProgress > 0.6f && ProjectileCheck == 0)
                    SlimeProjs(player);
                SwingRotation = MathHelper.SmoothStep(-maxAngle, maxAngle, swingProgress);
            }
            else if (progress < pauseEnd)
            {
                float animProgress = (progress - firstSwingEnd) / pauseDuration;
                SwingRotation = maxAngle;
                if (animProgress > 0.5f)
                    flip = true;
                Swinging = false;
                Projectile.ResetLocalNPCHitImmunity();
            }
            else
            {
                float swingProgress = (progress - pauseEnd) / swingDuration;
                if (swingProgress > 0.4f)
                {
                    if (!Swinging)
                    {
                        Swinging = true;
                        Projectile.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                        for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                        {
                            Projectile.oldPos[i] = Vector2.Zero;
                        }
                        ProjectileCheck = 0;
                    }
                }
                if (swingProgress > 0.6f && ProjectileCheck == 0)
                    SlimeProjs(player);
                SwingRotation = MathHelper.SmoothStep(maxAngle, -maxAngle * 0.8f, swingProgress);
                flip = true;
                FirstSwing = false;
            }

            bool canStop = (progress > firstSwingEnd && progress < pauseEnd) || (progress < prepEnd && !FirstSwing);
            if (!player.channel && canStop)
            {
                Projectile.Kill();
                return;
            }

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
            }

            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle; // + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.rotation += SwingRotation * Projectile.direction;

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            float offset = 32f * Projectile.scale;
            Projectile.Center = playerRotatedPoint + Projectile.rotation.ToRotationVector2() * offset;

            player.ChangeDir(Projectile.direction);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            if (flip)
                Projectile.spriteDirection *= -1;

            Projectile.timeLeft = 2;

            //player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + MathHelper.PiOver2 / 2f;
            if (effects != SpriteEffects.None)
                rotation += MathHelper.PiOver2;

            if (Swinging)
            {
                SlashOpacity = MathHelper.Lerp(SlashOpacity, 0.5f, 0.1f);
            }
            else
            {
                if (SlashOpacity > 0f)
                    SlashOpacity -= 0.1f;
                else
                    SlashOpacity = 0f;
            }

            Texture2D slashTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/BossWeapons/SlimeKingSlasherProjSlash").Value;
            Vector2 slashOrigin = slashTexture.Size() / 2f;
            Rectangle rectangle2 = slashTexture.Bounds;
            Main.EntitySpriteDraw(slashTexture, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle2), Projectile.GetAlpha(lightColor) * SlashOpacity, rotation, slashOrigin, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(texture, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}