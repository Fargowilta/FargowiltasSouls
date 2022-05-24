using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HellSkull2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_585";

        public float targetRotation;
        /*public int targetID = -1;
        public int searchTimer = 30;*/

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Skull");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.ClothiersCurse];
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120; //600;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            const int period = 60;

            if (Projectile.localAI[0] == 0.0)
            {
                Projectile.localAI[0] = 1f;
                Projectile.localAI[1] = 50;
                targetRotation = Projectile.velocity.ToRotation();

                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                for (int i = 0; i < 3; ++i)
                {
                    int index2 = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale),
                        27, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity = Projectile.DirectionTo(Main.dust[index2].position);
                    Main.dust[index2].velocity *= -5f;
                    Main.dust[index2].velocity += Projectile.velocity / 2f;
                    Main.dust[index2].noLight = true;
                }
            }

            float speed = Projectile.velocity.Length();
            float rotation = targetRotation + (float)Math.PI / 4 * (float)Math.Sin(2 * (float)Math.PI * Projectile.localAI[1] / period) * Projectile.ai[1];
            if (++Projectile.localAI[1] > period)
                Projectile.localAI[1] = 0;
            Projectile.velocity = speed * rotation.ToRotationVector2();

            if (Projectile.alpha > 0)
                Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (++Projectile.frameCounter >= 12)
                Projectile.frameCounter = 0;
            Projectile.frame = Projectile.frameCounter / 2;
            if (Projectile.frame > 3)
                Projectile.frame = 6 - Projectile.frame;

            Lighting.AddLight(Projectile.Center, NPCID.Sets.MagicAuraColor[54].ToVector3());

            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.direction < 0)
                Projectile.rotation += (float)Math.PI;

            /*if (targetID == -1) //no target atm
            {
                if (searchTimer <= 0)
                {
                    searchTimer = 30;

                    int possibleTarget = -1;
                    float closestDistance = 1000f;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.CanBeChasedBy())
                        {
                            float distance = Vector2.Distance(Projectile.Center, npc.Center);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                possibleTarget = i;
                            }
                        }
                    }

                    if (possibleTarget != -1)
                    {
                        targetID = possibleTarget;
                        Projectile.netUpdate = true;
                    }
                }
                searchTimer--;
            }
            else //currently have target
            {
                NPC npc = Main.npc[targetID];

                if (npc.CanBeChasedBy()) //target is still valid
                {
                    if (Projectile.Distance(npc.Center) > npc.width + npc.height)
                        targetRotation = (npc.Center - Projectile.Center).ToRotation();
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    Projectile.netUpdate = true;
                }
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(ModContent.BuffType<Buffs.Souls.HellFire>(), 300);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 0.5f, Pitch = 0.2f }, Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Shadowflame, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = new Color(212, 148, 255) * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                    color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}