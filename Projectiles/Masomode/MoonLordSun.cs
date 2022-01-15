using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordSun : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sun");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 410;
            projectile.height = 410;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            
            projectile.extraUpdates = 0;
            cooldownSlot = 1;

            //projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCheck = projectile => CanDamage() && projectile.Distance(Main.LocalPlayer.Center) < Math.Min(projectile.width, projectile.height) / 2 + Player.defaultHeight + Main.LocalPlayer.GetModPlayer<FargoPlayer>().GrazeRadius && Collision.CanHit(projectile.Center, 0, 0, Main.LocalPlayer.Center, 0, 0);
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
            projectile.penetrate = -1;

            projectile.scale = 0.75f;
            projectile.alpha = 255;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0f, 0f, 0, Color.White, 6f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 4f;

            NPC core = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.MoonLordCore);
            NPC socket = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.MoonLordHand);
            if (socket == null || core == null || socket.ai[3] != core.whoAmI || core.ai[0] == 2f)
            {
                projectile.Kill();
                return;
            }

            if (++projectile.localAI[0] < 120) //prepare to throw down
            {
                //if (projectile.localAI[0] == 1) Dusts();

                //use hand's x pos but core's y pos
                /*Vector2 targetPos = new Vector2(Main.npc[ai1].Center.X, Main.npc[ai0].Center.Y - 300);
                projectile.velocity = (targetPos - projectile.Center) / 30;
                Main.npc[ai1].Center = projectile.Center + projectile.velocity; //glue hand to me until thrown*/

                projectile.Center = socket.Center;
                projectile.position.Y -= 250f * Math.Min(1f, projectile.localAI[0] / 85);

                projectile.alpha -= 3;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            else if (projectile.localAI[0] == 120) //launch at player
            {
                //d = (v+0)*t/2
                //v = d*2/t
                projectile.velocity = (Main.player[core.target].Center - projectile.Center) * 2f / 90f;
                //0 = v+a*t
                //a = -v/t
                projectile.localAI[1] = projectile.velocity.Length() / 90f;

                projectile.timeLeft = 91;
                projectile.netUpdate = true;
            }
            else
            {
                projectile.velocity = Vector2.Normalize(projectile.velocity) * (projectile.velocity.Length() - projectile.localAI[1]);
                projectile.alpha = 0;
            }
        }

        /*public void Dusts()
        {
            Main.PlaySound(SoundID.NPCKilled, projectile.Center, 6);
            for (int index1 = 0; index1 < 15; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
            }
            for (int index1 = 0; index1 < 50; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 0, new Color(), 2.5f);
                Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 1f;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Main.dust[index3].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 1f;
                Main.dust[index3].noGravity = true;
            }

            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 50; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 100; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 100; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                if (Main.rand.NextBool(3))
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                Main.dust[d].position = projectile.Center;
            }
        }*/

        public override void Kill(int timeLeft)
        {
            NPC core = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.MoonLordCore);
            NPC socket = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.MoonLordHand);
            if (socket == null || core == null || socket.ai[3] != core.whoAmI || core.ai[0] == 2f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(), 0, 0f, projectile.owner);
            }
            else
            {
                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                if (Main.netMode != NetmodeID.MultiplayerClient) //chain explosions
                {
                    //perpendicular
                    /*Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                        projectile.damage, projectile.knockBack, projectile.owner, projectile.velocity.ToRotation() + MathHelper.PiOver2, 5);
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                        projectile.damage, projectile.knockBack, projectile.owner, projectile.velocity.ToRotation() - MathHelper.PiOver2, 5);*/

                    const int max = 4; //spread
                    for (int i = 0; i < max; i++)
                    {
                        Vector2 offset = projectile.width / 2 * Vector2.UnitX.RotatedBy(Math.PI * 2 / max * i);
                        Projectile.NewProjectile(projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2, projectile.height / 2), Vector2.Zero, ModContent.ProjectileType<MoonLordSunBlast>(),
                            projectile.damage, projectile.knockBack, projectile.owner, MathHelper.WrapAngle(offset.ToRotation()), 32);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Burning, 120);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(255, Main.DiscoG + 105, Main.DiscoB / 2 + 105) * projectile.Opacity;
            Color glow2 = new Color(255, Main.DiscoG + 25, Main.DiscoB / 2 + 25) * projectile.Opacity;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}