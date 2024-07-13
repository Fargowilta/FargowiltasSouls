﻿using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class SisypheanBoulder : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_99";
        private bool launched = false;
        private int bounceCount = 0;
        public override void SetDefaults() 
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.channel && !player.CCed && !player.noItems && !launched)
            {
                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);

                float rot = MathHelper.Pi / 6 * player.direction;
                Vector2 holdOffset = new Vector2(0f, -20f).RotatedBy(rot);
                Projectile.Center = player.Center + holdOffset;

                Projectile.direction = Main.MouseWorld.DirectionTo(player.Center).X < 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
                Projectile.spriteDirection = Projectile.direction;

                Projectile.timeLeft++;
                Projectile.friendly = false;
            }
            else
            {
                if (!launched) //On mouse release, calculate velocity
                {
                    float angle = Projectile.Center.AngleTo(Main.MouseWorld);
                    Projectile.velocity = new Vector2(20f, 0f).RotatedBy(angle);
                    launched = true;
                    Projectile.hide = false;
                    Projectile.tileCollide = true;
                    Projectile.friendly = true;
                    SoundEngine.PlaySound(SoundID.Item1 with {Pitch = -0.8f}, Projectile.Center);
                }
                else
                {
                    Projectile.velocity.Y += 0.75f;
                    Projectile.rotation += 0.2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bounceCount++ >= 10)
            {
                Projectile.Kill();
            }
            else {

                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
                }

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X * 0.9f;
                }

                for (int i = 0; i < 10; i++) {
                    Dust.NewDust(Projectile.Center, 10, 10, DustID.Stone);
                }
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDust(Projectile.Center, 25, 25, DustID.Stone);
            }
            int shrapnelCount = 8;
            int damage = Projectile.damage / 3;
            for (int i = 0; i < shrapnelCount; i++)
            {
                float direction = Main.rand.NextFloatDirection();
                if (Main.myPlayer == Projectile.owner) {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0f, -14f).RotatedBy(direction), ModContent.ProjectileType<SisypheanShrapnel>(), damage, 0.25f);
                    ScreenShakeSystem.StartShake(3, shakeStrengthDissipationIncrement: 0.1f);
                } 
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation;
            SpriteEffects flip = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), lightColor, rot, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, flip);
            return false;
        }
    }
}
