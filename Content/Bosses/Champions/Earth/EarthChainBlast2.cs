using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Earth
{
    public class EarthChainBlast2 : MoonLordSunBlast
    {
        public override string Texture => "Terraria/Images/Projectile_687";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chain Blast");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.Burning, 300);
                target.AddBuff(ModContent.BuffType<LethargicBuff>(), 300);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Color.White;
            if (Projectile.ai[1] > 3)
                color = Color.Lerp(new Color(255, 255, 255, 0), new Color(255, 95, 46, 50), Math.Min(2, 7 - Projectile.ai[1]) / 4);

            else
                color = Color.Lerp(new Color(255, 95, 46, 50), new Color(150, 35, 0, 100), (3 - Projectile.ai[1]) / 3);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color,
                Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}

