using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class LunarCultistIceMist : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_464";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Mist");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                fargo.Call("LowRenderProj", Projectile);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
            }

            Projectile.alpha += Projectile.timeLeft > 20 ? -10 : 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;

            if (Projectile.timeLeft % 60 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                Vector2 vel = Vector2.UnitX.RotatedBy(Projectile.rotation);
                vel *= 12f;
                for (int i = 0; i < 6; i++)
                {
                    vel = vel.RotatedBy(2f * (float)Math.PI / 6f);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<LunarCultistIceSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.X, Projectile.velocity.Y);
                }
            }

            Projectile.rotation += (float)Math.PI / 40f;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.75f, 0.9f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * Projectile.Opacity * 0.6f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}