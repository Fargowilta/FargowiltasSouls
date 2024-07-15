using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeRunespearHitbox : ModProjectile
    {
        public const int Length = 260;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1;
            Projectile.timeLeft = 6000;

            Projectile.Opacity = 0;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) //line collision, needed because of the speed they move at when creating the arena, to form a solid wall
        {
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Length, Projectile.width, ref collisionPoint))
            {
                return true;
            }
            return false;
        }
        public bool HitboxActive = false;
        public override void AI()
        {
            NPC lifelight = Main.npc[(int)Projectile.ai[0]];
            if (!lifelight.TypeAlive<LifeChallenger>())
            {
                Projectile.Kill();
            }
            LifeChallenger lifelightMNPC = lifelight.As<LifeChallenger>();

            Projectile.Center = lifelight.Center + lifelightMNPC.LockVector1;
            Projectile.rotation = lifelightMNPC.LockVector2.ToRotation();

            if (Projectile.timeLeft > (int)Projectile.ai[1])
                Projectile.timeLeft = (int)Projectile.ai[1];

            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.07f);

            HitboxActive = lifelightMNPC.HitPlayer;

        }
        public override bool? CanDamage()
        {
            return HitboxActive ? base.CanDamage() : false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Projectile.scale = 2;
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * Length;
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, drawPos: drawPos, rotation: Projectile.rotation + MathHelper.PiOver4);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }

    }
}
