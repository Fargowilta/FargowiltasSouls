using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Deathrays
{
    public abstract class AbomSpecialDeathray : BaseDeathray
    {
        public AbomSpecialDeathray(int maxTime) : base(maxTime, "AbomDeathray") { }

        const int sheetMax = 11;

        public override void AI()
        {
            if (Projectile.localAI[0] % Projectile.MaxUpdates == 0)
            {
                if (++Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= sheetMax)
                        Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = Mod.Assets.Request<Texture2D>($"Projectiles/Deathrays/Abom/AbomDeathray_{Projectile.frame}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = Mod.Assets.Request<Texture2D>($"Projectiles/Deathrays/Abom/AbomDeathray2_{Projectile.frame}", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = Mod.Assets.Request<Texture2D>($"Projectiles/Deathrays/{texture}3", AssetRequestMode.ImmediateLoad).Value;
            float num223 = Projectile.localAI[1];
            Color color44 = new Color(255, 255, 255, 0) * 0.95f;
            Texture2D arg_ABD8_1 = texture2D19;
            Vector2 arg_ABD8_2 = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.EntitySpriteDraw(arg_ABD8_1, arg_ABD8_2, sourceRectangle2, color44, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture2D20.Width, 30);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }

                    Main.EntitySpriteDraw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 30;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Texture2D arg_AE2D_1 = texture2D21;
            Vector2 arg_AE2D_2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            Main.EntitySpriteDraw(arg_AE2D_1, arg_AE2D_2, sourceRectangle2, color44, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}