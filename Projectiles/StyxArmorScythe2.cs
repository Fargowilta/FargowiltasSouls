using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class StyxArmorScythe2 : StyxArmorScythe
    {
        public override string Texture => "FargowiltasSouls/Projectiles/StyxArmorScythe";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.penetrate = 1;

            Projectile.usesLocalNPCImmunity = false;
            Projectile.localNPCHitCooldown = 0;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = false;
        }

        public override void AI()
        {
            const int baseDamage = 100;

            if (Projectile.velocity == Vector2.Zero || Projectile.velocity.HasNaNs())
                Projectile.velocity = -Vector2.UnitY;

            Player player = Main.player[Projectile.owner];
            Projectile.damage = (int)(baseDamage * player.ownedProjectileCounts[Projectile.type] * player.GetDamage(DamageClass.Magic).Additive);
            if (++Projectile.ai[0] > 10)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000);
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 0)
            {
                if (Projectile.velocity.Length() < 24)
                    Projectile.velocity *= 1.06f;
            }

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1]);
            if (npc != null)
            {
                double num4 = (npc.Center - Projectile.Center).ToRotation() - Projectile.velocity.ToRotation();
                if (num4 > Math.PI)
                    num4 -= 2.0 * Math.PI;
                if (num4 < -1.0 * Math.PI)
                    num4 += 2.0 * Math.PI;
                Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.2f);
            }
            else
            {
                Projectile.ai[1] = -1f;
                Projectile.netUpdate = true;
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation += Projectile.spriteDirection * 1f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 70, Scale: 3f);
                Main.dust[d].velocity *= 12f;
                Main.dust[d].noGravity = true;
            }

            Projectile.timeLeft = 0;
            Projectile.penetrate = -1;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 200;
            Projectile.Center = Projectile.position;

            if (timeLeft > 0)
            {
                Projectile.Damage();
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 0, 0) * Projectile.Opacity; //yellow
        }
    }
}