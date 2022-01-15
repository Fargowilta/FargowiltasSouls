using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class EarthChainBlast2 : Masomode.MoonLordSunBlast
    {
        public override string Texture => "Terraria/Projectile_687";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Chain Blast");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Burning, 300);
                target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            if(projectile.ai[1] > 3)
                color = Color.Lerp(new Color(255, 255, 255, 0), new Color(255, 95, 46, 50), Math.Min(2, 7 - projectile.ai[1]) / 4);

            else
                color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (3-projectile.ai[1]) / 3);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, 
                projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}

