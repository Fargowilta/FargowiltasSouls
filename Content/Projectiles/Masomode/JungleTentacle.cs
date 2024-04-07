using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class JungleTentacle : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_264";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jungle Tentacle");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.PlanterasTentacle];
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            //CooldownSlot = 1;

            Projectile.extraUpdates = 0;
            Projectile.timeLeft = 240 * (Projectile.extraUpdates + 1);
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.BigMimicJungle);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.rotation = Projectile.DirectionFrom(npc.Center).ToRotation() + MathHelper.Pi;
            Projectile.localAI[0] = npc.Center.X;
            Projectile.localAI[1] = npc.Center.Y;

            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.frame = 0;
                //Projectile.timeLeft--;
            }
            else
            {
                Projectile.velocity *= 1.005f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

                if (npc.HasPlayerTarget && Projectile.Distance(npc.Center) > npc.Distance(Main.player[npc.target].Center))
                {
                    Tile tile = Framing.GetTileSafely(Projectile.Center);
                    if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType])
                        Projectile.velocity = Vector2.Zero;
                }

                if (++Projectile.frameCounter > 3 * (Projectile.extraUpdates + 1))
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.IvyVenomBuff>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] != 0 && Projectile.localAI[1] != 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Chain27", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
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
                        color2 = Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                    }
            }

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}