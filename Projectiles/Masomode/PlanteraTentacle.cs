using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PlanteraTentacle : ModProjectile
    {
        public override string Texture => "Terraria/NPC_264";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Planty Tentacle");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.PlanterasTentacle];
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hostile = true;
            //cooldownSlot = 1;

            projectile.extraUpdates = 0;
            projectile.timeLeft = 360 * (projectile.extraUpdates + 1);

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCheck =
                projectile =>
                {
                    float num6 = 0f;
                    if (CanDamage() && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(),
                        new Vector2(projectile.localAI[0], projectile.localAI[1]), projectile.Center, 22f * projectile.scale + Main.LocalPlayer.GetModPlayer<FargoPlayer>().GrazeRadius * 2 + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };
        }

        public override bool CanDamage()
        {
            return counter >= attackTime;
        }

        private int counter;
        private const int attackTime = 150;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.Plantera);
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            projectile.rotation = projectile.DirectionFrom(npc.Center).ToRotation() + MathHelper.Pi;
            projectile.localAI[0] = npc.Center.X;
            projectile.localAI[1] = npc.Center.Y;

            if (projectile.velocity == Vector2.Zero)
            {
                projectile.frame = 0;
                //projectile.timeLeft--;
            }
            else
            {
                if (counter == 0)
                    Main.PlaySound(SoundID.Item5, projectile.Center);

                if (++counter < attackTime)
                {
                    projectile.position += npc.velocity / 3;

                    Vector2 target = npc.Center + (150f + counter * 1.5f) * projectile.ai[1].ToRotationVector2();
                    Vector2 distance = target - projectile.Center;
                    float length = distance.Length();
                    if (length > 10f)
                    {
                        distance /= 8f;
                        projectile.velocity = (projectile.velocity * 23f + distance) / 24f;
                    }
                    else
                    {
                        if (projectile.velocity.Length() < 12f)
                            projectile.velocity *= 1.05f;
                    }
                }
                else if (counter == attackTime)
                {
                    projectile.velocity = 32f * projectile.ai[1].ToRotationVector2();
                    Main.PlaySound(SoundID.Item92, projectile.Center);
                }
                else
                {
                    if (npc.HasPlayerTarget && projectile.Distance(npc.Center) > npc.Distance(Main.player[npc.target].Center))
                    {
                        Tile tile = Framing.GetTileSafely(projectile.Center);
                        if (tile.nactive() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type])
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, -projectile.velocity.X * 0.1f, -projectile.velocity.Y * 0.1f, Scale: 2.5f);
                                Main.dust[d].noGravity = true;
                                Main.dust[d].velocity *= 4f;
                            }

                            projectile.velocity = Vector2.Zero;
                        }
                    }
                }

                if (++projectile.frameCounter > 3 * (projectile.extraUpdates + 1))
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item5, projectile.Center);

            if (projectile.localAI[0] != 0 && projectile.localAI[1] != 0)
            {
                Vector2 planteraCenter = new Vector2(projectile.localAI[0], projectile.localAI[1]);

                int length = (int)projectile.Distance(planteraCenter);
                const int increment = 512;
                for (int i = 0; i < length; i += increment)
                {
                    Gore.NewGore(projectile.Center + projectile.DirectionTo(planteraCenter) * (i + Main.rand.NextFloat(increment)), Vector2.Zero, 
                        mod.GetGoreSlot("Gores/Plantera/Gore_" + (Main.rand.NextBool() ? "386" : "387")), projectile.scale);
                }
            }

            //Gore.NewGore(projectile.Center, Vector2.Zero, mod.GetGoreSlot("Gores/Plantera/Gore_" + (Main.rand.NextBool() ? "388" : "389")), projectile.scale);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(ModContent.BuffType<Infested>(), 180);
            target.AddBuff(ModContent.BuffType<IvyVenom>(), 240);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    new Vector2(projectile.localAI[0], projectile.localAI[1]), projectile.Center);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[0] != 0 && projectile.localAI[1] != 0)
            {
                Texture2D texture = mod.GetTexture("NPCs/Vanilla/Chain26");
                Vector2 position = projectile.Center;
                Vector2 mountedCenter = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = projectile.GetAlpha(color2);
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                    }
            }

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}