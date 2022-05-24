using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class SparklingDevi : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Eternals/DevianttSoul";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deviantt");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 115;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            Projectile.scale = 1;

            Player player = Main.player[Projectile.owner];

            int target = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 2000);

            if (++Projectile.ai[0] == 50) //spawn axe
            {
                Projectile.netUpdate = true;
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 offset = new Vector2(0, -275).RotatedBy(Math.PI / 4 * Projectile.spriteDirection);
                    FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<SparklingLoveBig>(),
                        Projectile.originalDamage, Projectile.knockBack, Projectile.owner, 0f, Projectile.identity);
                }
            }
            else if (Projectile.ai[0] < 100)
            {
                Vector2 targetPos;

                if (target != -1 && Main.npc[target].CanBeChasedBy(Projectile))
                {
                    targetPos = Main.npc[target].Center;
                    Projectile.direction = Projectile.spriteDirection = Projectile.Center.X > targetPos.X ? 1 : -1;
                    targetPos.X += 500 * Projectile.direction;
                    targetPos.Y -= 200;
                }
                else
                {
                    Projectile.direction = Projectile.spriteDirection = -Main.player[Projectile.owner].direction;
                    targetPos = Main.player[Projectile.owner].Center + new Vector2(100 * Projectile.direction, -100);
                }

                if (Projectile.Distance(targetPos) > 50)
                    Movement(targetPos, 1f);
            }
            else if (Projectile.ai[0] == 99 || Projectile.ai[0] == 100)
            {
                Projectile.netUpdate = true;

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 targetPos;

                    if (target != -1 && Main.npc[target].CanBeChasedBy(Projectile))
                    {
                        targetPos = Main.npc[target].Center + Main.npc[target].velocity * 10;
                    }
                    else
                    {
                        targetPos = Main.MouseWorld;
                    }

                    Projectile.direction = Projectile.spriteDirection = Projectile.Center.X > targetPos.X ? 1 : -1;

                    targetPos.X += 360 * Projectile.direction;

                    if (Projectile.ai[0] == 100)
                    {
                        Projectile.velocity = (targetPos - Projectile.Center) / Projectile.timeLeft;

                        Projectile.position += Projectile.velocity; //makes sure the offset is right
                    }
                }
            }

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            int num812 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                86, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1.5f);
            Main.dust[num812].noGravity = true;
        }

        private void Movement(Vector2 targetPos, float speedModifier)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > 24)
                Projectile.velocity.X = 24 * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > 24)
                Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 300);
            target.immune[Projectile.owner] = 1;
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150) * Projectile.Opacity * 0.75f;
        }
    }
}