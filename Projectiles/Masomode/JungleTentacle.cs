using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class JungleTentacle : ModProjectile
    {
        public override string Texture => "Terraria/NPC_264";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Jungle Tentacle");
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
            projectile.timeLeft = 240 * (projectile.extraUpdates + 1);
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.BigMimicJungle);
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
                projectile.velocity *= 1.005f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;

                if (npc.HasPlayerTarget && projectile.Distance(npc.Center) > npc.Distance(Main.player[npc.target].Center))
                {
                    Tile tile = Framing.GetTileSafely(projectile.Center);
                    if (tile.nactive() && Main.tileSolid[tile.type])
                        projectile.velocity = Vector2.Zero;
                }

                if (++projectile.frameCounter > 3 * (projectile.extraUpdates + 1))
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame >= Main.projFrames[projectile.type])
                        projectile.frame = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.IvyVenom>(), 240);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.localAI[0] != 0 && projectile.localAI[1] != 0)
            {
                Texture2D texture = mod.GetTexture("NPCs/Vanilla/Chain27");
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