using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class TargetingReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Targeting Reticle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 90;
            //CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
            if (npc != null && npc.HasPlayerTarget)
            {
                Projectile.alpha -= 4;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;

                int modifier = Math.Min(60, 90 - Projectile.timeLeft);

                Projectile.scale = 4f - 3f / 60f * modifier; //start big, shrink down

                Projectile.Center = npc.Center;
                Projectile.velocity = Main.player[npc.target].Center - Projectile.Center;
                Projectile.velocity = Projectile.velocity / 60 * modifier; //move from npc to player
                Projectile.rotation = (float)Math.PI * 2 / 30 * modifier * Math.Sign(Projectile.velocity.X);
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}