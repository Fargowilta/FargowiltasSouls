using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TimberHook2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_13";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Hook");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4800;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCheck =
                projectile =>
                {
                    float num6 = 0f;
                    if (CanDamage() == true && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(),
                        new Vector2(Projectile.localAI[0], Projectile.localAI[1]), Projectile.Center, 22f * Projectile.scale + Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().GrazeRadius * 2 + Player.defaultHeight, ref num6))
                    {
                        return true;
                    }
                    return false;
                };

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage() => canHurt;

        bool canHurt => Projectile.ai[1] < 0;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampionHead>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            if (--Projectile.ai[1] < 0) //deal damage
            {
                if (Projectile.ai[1] < -120)
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.localAI[0] = npc.Center.X;
                Projectile.localAI[1] = npc.Center.Y;

                const int increment = 150; //dust
                int distance = (int)Projectile.Distance(npc.Center);
                Vector2 direction = Projectile.DirectionTo(npc.Center);
                for (int i = 2; i < distance; i += increment)
                {
                    float offset = i + Main.rand.NextFloat(-increment, increment);
                    if (offset < 0)
                        offset = 0;
                    if (offset > distance)
                        offset = distance;
                    int d = Dust.NewDust(Projectile.Center + direction * offset, 0, 0, DustID.Frost);
                    Main.dust[d].scale = 0.75f;
                    Main.dust[d].noGravity = true;
                }
            }

            if (Projectile.Distance(npc.Center) > 1500 + npc.Distance(Main.player[npc.target].Center))
                Projectile.velocity = Vector2.Zero;

            if (!Projectile.tileCollide && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.tileCollide = true;

            Projectile.rotation = Projectile.DirectionFrom(npc.Center).ToRotation() + (float)Math.PI / 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                new Vector2(Projectile.localAI[0], Projectile.localAI[1]), Projectile.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool flashingZapEffect = canHurt && Projectile.timeLeft % 10 < 5;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampionHead>());
            if (npc != null && TextureAssets.Chain.IsLoaded)
            {
                Texture2D texture = TextureAssets.Chain.Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = npc.Center;
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
                        color2 = flashingZapEffect ? Color.White * Projectile.Opacity : Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                        if (flashingZapEffect)
                        {
                            color2.A = 0;
                            Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                        }
                    }
            }

            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Color color = flashingZapEffect ? Color.White * Projectile.Opacity : Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            if (flashingZapEffect)
            {
                color.A = 0;
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }
            return false;
        }
    }
}