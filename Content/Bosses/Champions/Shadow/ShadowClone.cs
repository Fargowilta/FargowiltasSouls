using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Shadow
{
    public class ShadowClone : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Shadow/ShadowChampion";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Champion of Shadow");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];

            Projectile.direction = Projectile.spriteDirection = Projectile.Center.X < player.Center.X ? 1 : -1;

            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default, 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 4f;
            }
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 0, default, 5f);
                Main.dust[d].noGravity = true;
            }

            if (--Projectile.ai[1] > 0)
            {
                Vector2 targetPos = player.Center + Projectile.DirectionFrom(player.Center) * 400f;
                if (Projectile.Distance(targetPos) > 50)
                    Movement(targetPos, 0.3f, 24f);

                Projectile.position += player.velocity * 0.9f;
            }
            else if (Projectile.ai[1] == 0)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.position += player.velocity / 4f;
                Projectile.netUpdate = true;
                Projectile.localAI[0] = Projectile.SafeDirectionTo(player.Center).ToRotation();
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.SafeDirectionTo(player.Center), ModContent.ProjectileType<ShadowDeathraySmall>(), 0, 0f, Main.myPlayer);
                }
            }
            else if (Projectile.ai[1] == -30)
            {
                Projectile.velocity = 45f * Vector2.UnitX.RotatedBy(Projectile.localAI[0]);
            }

            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.frame = 0;
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f, bool fastY = false)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += fastY ? speedModifier * 2 : speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= fastY ? speedModifier * 2 : speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > cap)
                Projectile.velocity.X = cap * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > cap)
                Projectile.velocity.Y = cap * Math.Sign(Projectile.velocity.Y);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Darkness, 300);
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
                target.AddBuff(BuffID.Blackout, 300);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Black;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp/*.PointWrap*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.VoidDye);
            shader.Apply(Projectile, new Terraria.DataStructures.DrawData?());

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture2D14 = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/Champions/Shadow/ShadowChampion_Trail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * 0.25f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D14, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}