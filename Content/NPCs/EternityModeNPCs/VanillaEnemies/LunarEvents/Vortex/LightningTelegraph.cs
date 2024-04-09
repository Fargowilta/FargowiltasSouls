using FargowiltasSouls.Content.Bosses.Champions.Will;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex
{
    public class LightningTelegraph : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Meteor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 80;
            Projectile.scale = 1;
            AIType = 0;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {

        }
        private static readonly SoundStyle LightningSound = new("FargowiltasSouls/Assets/Sounds/LightningStrike");
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 13) //if its the top one
            {
                SoundEngine.PlaySound(LightningSound with { MaxInstances = 4, Volume = 0.2f }, Main.LocalPlayer.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 dir = Vector2.UnitY;
                    Vector2 vel = Vector2.Normalize(dir);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel * 6, ModContent.ProjectileType<VortexLightningDeathray>(),
                        Projectile.damage, 0, Main.myPlayer, dir.ToRotation(), 1);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Color color = Color.Red;
            color.A = 0;
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}