
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class PrismaRegaliaProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prisma Regalia");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 244;
            Projectile.height = 244;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 150;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 HitboxSize = new(88 * Projectile.scale, 88 * Projectile.scale);
            Vector2 HitboxCenter = Projectile.Center + (Projectile.velocity * (Projectile.Size.Length() / 2f - HitboxSize.Length()/ 2f));
            hitbox = new Rectangle((int)(HitboxCenter.X - HitboxSize.X / 2f), (int)(HitboxCenter.Y - HitboxSize.Y / 2f), (int)(HitboxSize.X), (int)(HitboxSize.Y));
        }
        public int SwingDirection = 1;
        public float Extension = 0;
        int OrigAnimMax = 30;
        bool Charged;
        Vector2 ChargeVector = Vector2.Zero;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            if (Projectile.localAI[0] == 0)
            {
                OrigAnimMax = player.itemAnimationMax;
                Projectile.localAI[0] = 1;
            }
            float HoldoutRangeMax = (float)Projectile.Size.Length() / 2; //since sprite is diagonal
            float HoldoutRangeMin = (float)-Projectile.Size.Length() / 6;

            if (player.channel)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld);
                Projectile.Center = player.MountedCenter + Projectile.velocity * HoldoutRangeMin;
                Projectile.friendly = false;
                if (Projectile.ai[0] < 60 * 3)
                    Projectile.ai[0] ++;
                if (Projectile.ai[0] == 60 * 3 - 1)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center + (Projectile.velocity * Projectile.Size.Length()/2));
                }
            }
            else
            {
                Projectile.friendly = true;
                if (Projectile.ai[0] > -1) //check once
                {
                    Projectile.damage = (int)(Projectile.damage * (1 + (Projectile.ai[0] / 60f))); //modify this to change damage charge
                    Charged = Projectile.ai[0] == 60 * 3;
                    Projectile.ai[0] = -1;
                    
                }
                if (Charged) //charge until spear will hit mouse
                {
                    //this charge is super janky and unfinished. please do this better than me. the rest is pretty polished though. there's also no sound or dust effects here.

                    if (Projectile.ai[0] > -2) //check once
                    {
                        Vector2 ChargePos = Main.MouseWorld - (Projectile.velocity * HoldoutRangeMax);
                        ChargeVector = player.DirectionTo(ChargePos) * player.Distance(ChargePos) / 15;
                        Projectile.ai[0] = -2;
                    }

                    player.velocity = ChargeVector;
                    player.controlLeft = false;
                    player.controlRight = false;
                    player.controlJump = false;
                    player.controlDown = false;
                    player.controlUseItem = false;
                    player.controlUseTile = false;
                    player.controlHook = false;
                    player.controlMount = false;

                    if (Projectile.ai[0] < -18) //one frame before player reaches target, stop
                    {
                        player.velocity = Vector2.Zero;
                        if (Projectile.timeLeft > 10)
                        {
                            Projectile.timeLeft = 10;
                        }
                        Projectile.Center = player.MountedCenter + (Projectile.velocity * HoldoutRangeMax);
                    }
                    else
                    {
                        Projectile.Center = player.MountedCenter;
                    }
                    Projectile.ai[0]--;
                    
                }
                else //normal swing
                {
                    int duration = (int)(OrigAnimMax / 1.5f);
                    int WaitTime = OrigAnimMax / 5;


                    if (Projectile.ai[1] == 0)
                        SwingDirection = Main.rand.NextBool(2) ? 1 : -1;
                    float Swing = 13; //higher value = less swing
                    Projectile.localNPCHitCooldown = OrigAnimMax;   //only hit once per swing
                                                                    //projectile.ai[1] is time from spawn
                                                                    //Extension is between 0 and 1
                    if (Projectile.timeLeft > OrigAnimMax)
                    {
                        Projectile.timeLeft = OrigAnimMax;
                    }
                    if (Projectile.ai[1] <= duration / 2)
                    {
                        Extension = Projectile.ai[1] / (duration / 2);
                        Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
                    }
                    else if (Projectile.ai[1] <= duration / 2 + WaitTime)
                    {
                        Extension = 1;
                        Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * OrigAnimMax)); //i know how wacky this looks
                    }
                    else
                    {
                        Projectile.friendly = false; //no hit on backswing
                        Extension = (duration + WaitTime - Projectile.ai[1]) / (duration / 2);
                        Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
                    }

                    if (Projectile.ai[1] == duration / 2)
                        SoundEngine.PlaySound(SoundID.Item1, player.Center);

                    Projectile.ai[1]++;
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity); //store direction
                    Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, Extension);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(-45f) + (float)Math.PI;
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(-135f) + (float)Math.PI;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i ++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.rotation;//Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}