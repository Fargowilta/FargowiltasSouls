using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DestroyerLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_658";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.scale = 1.8f;
            Projectile.tileCollide = false;

            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 190 * Projectile.extraUpdates + 1;
        }
		
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (projHitbox.Intersects(targetHitbox))
                return true;
			
			float length = Math.Max(Projectile.width, Projectile.velocity.Length() * 2);
			float dummy = 0f;
			Vector2 offset = length / 2 * Projectile.scale * (Projectile.rotation - MathHelper.ToRadians(135f)).ToRotationVector2();
			Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;
			
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, Projectile.width / 2, ref dummy))
                return true;
			
			return false;
		}

        public override void AI()
        {
            int totalUpdates = Projectile.extraUpdates + 1;

            if (Projectile.ai[0] == 1f)
            {
                if (Projectile.timeLeft > 91 * totalUpdates) //mp sync is important for this, which is why i do it this way
                    Projectile.timeLeft = 91 * totalUpdates;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (Projectile.timeLeft % totalUpdates == 0) //only run once per tick
            {
                if (++Projectile.localAI[1] > 20 && Projectile.localAI[1] < 90)
                    Projectile.velocity *= 1.06f;

                if (Projectile.alpha > 0 && Projectile.timeLeft > 10 * totalUpdates)
                {
                    Projectile.alpha -= 14;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
                else if (Projectile.timeLeft <= 10 * totalUpdates)
                {
                    Projectile.velocity *= 0.7f;
                    Projectile.alpha += 25;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                target.AddBuff(BuffID.Electrified, 60);
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu))
            {
                target.AddBuff(BuffID.Poisoned, 120);
                target.AddBuff(BuffID.Darkness, 120);
                target.AddBuff(BuffID.Bleeding, 120);
                target.AddBuff(BuffID.Slow, 120);
                target.AddBuff(BuffID.Weak, 120);
                target.AddBuff(BuffID.BrokenArmor, 120);
            }
        }

        public override bool? CanDamage() => Projectile.timeLeft > 10 * (Projectile.extraUpdates + 1);

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D hitboxindicator = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantSphereGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 scale = new Vector2(1, 1 + Projectile.velocity.Length() / 5 * (Projectile.extraUpdates + 1));
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, scale, SpriteEffects.None, 0);

            /*if (Projectile.timeLeft > 10 * (Projectile.extraUpdates + 1))
            {
                Main.EntitySpriteDraw(hitboxindicator, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, hitboxindicator.Width, hitboxindicator.Height),
                    new Color(255, 133, 149) * Projectile.Opacity, 0, hitboxindicator.Size() / 2, 0.25f, SpriteEffects.None, 0);
            }*/

            return false;
        }
    }
}