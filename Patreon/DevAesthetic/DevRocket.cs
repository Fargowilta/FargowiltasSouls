using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.DevAesthetic
{
    class DevRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dev Rocket");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.VortexBeaterRocket);
            Projectile.aiStyle = -1;
            //AIType = ProjectileID.VortexBeaterRocket;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = 2;

            Projectile.timeLeft = 75 * (Projectile.extraUpdates + 1);
        }

        private Color color;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Projectile.velocity.Length() * (Main.rand.NextBool() ? 1 : -1);
                color = new Color(50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5, 50 * Main.rand.Next(6) + 5);
                Projectile.ai[0] = Main.rand.Next(-30, 30);
                Projectile.ai[1] = -1;
            }

            Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (++Projectile.ai[0] > 30)
            {
                Projectile.ai[0] = 20;

                if (Projectile.timeLeft > 45 * Projectile.MaxUpdates)
                    Projectile.timeLeft = 45 * Projectile.MaxUpdates;

                if (Projectile.ai[1] == -1)
                {
                    Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, true);
                    Projectile.netUpdate = true;
                }
            }

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1]);
            if (npc != null && npc.CanBeChasedBy())
            {
                Projectile.position += npc.velocity / 5;

                Vector2 targetPos = npc.Center;
                float offset = 120 * Projectile.timeLeft / (30 * Projectile.MaxUpdates) * 2;
                if (Projectile.Distance(targetPos) > offset)
                    targetPos += Projectile.DirectionTo(npc.Center).RotatedBy(MathHelper.PiOver2) * offset * Math.Sign(Projectile.localAI[0]);
                Vector2 targetSpeed = Projectile.DirectionTo(targetPos) * Math.Abs(Projectile.localAI[0]);
                const int factor = 8;
                Projectile.velocity = (Projectile.velocity * (factor - 1) + targetSpeed) / factor;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[1] == 1)
            {
                Projectile[] projs = FargoSoulsUtil.XWay(Main.rand.Next(3, 7), Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.type, 6, Projectile.damage, Projectile.knockBack);
                foreach (Projectile proj in projs)
                {
                    if (proj != null)
                        proj.localAI[1] = 2;
                }
            }

            Color dustColor = color;
            dustColor.A = 100;
            for (int i = 0; i < 2; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 76, Projectile.velocity.X, Projectile.velocity.Y, 100, dustColor, 2f);
                Main.dust[d].velocity *= 2f;
                Main.dust[d].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.timeLeft = 0;
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = 1;
            target.immune[Projectile.owner] = 3;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return color * Projectile.Opacity;
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
                Color color27 = color26 * 0.75f;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                float scale = Projectile.scale;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
