using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviDevilScythe : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_44";


        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.alpha = 100;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }


        public override void AI()
        {
            Projectile.velocity *= .96f;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? -1 : 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }

            Projectile.rotation += Projectile.localAI[0];

            Lighting.AddLight(Projectile.position, .3f, .1f, .3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 240);
            target.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), 240);
            target.AddBuff(ModContent.BuffType<GuiltyBuff>(), 240);
            target.AddBuff(ModContent.BuffType<LovestruckBuff>(), 240);
            target.AddBuff(ModContent.BuffType<RottingBuff>(), 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}