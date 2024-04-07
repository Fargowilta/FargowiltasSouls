using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantReticle2 : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/TargetingReticle";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Reticle");
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
            Projectile.timeLeft = 60;
            //CooldownSlot = 1;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
                && !Main.npc[EModeGlobalNPC.mutantBoss].dontTakeDamage)
            {
                int modifier = 60 - Projectile.timeLeft;

                Projectile.scale = 4f - 3f / 60f * modifier; //start big, shrink down

                Projectile.rotation = (float)Math.PI * 2 / 30 * modifier;
            }
            else
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft < 10)
                Projectile.alpha += 25;

            else
            {
                Projectile.alpha -= 4;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;
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