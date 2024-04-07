using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    internal class FishStickShark : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_408";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shark");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.MiniSharkron];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MiniSharkron);
            AIType = ProjectileID.MiniSharkron;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            //Projectile.extraUpdates = 1;

            Projectile.tileCollide = false;
            Projectile.minion = false;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ArmorPenetration += 20;
        }

        public override void AI()
        {
            Projectile.position += Projectile.velocity * 0.5f;
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[2]);
            if (npc != null && Projectile.velocity != Vector2.Zero && npc.CanBeChasedBy())
            {
                Projectile.velocity = Projectile.SafeDirectionTo(npc.Center) * Projectile.velocity.Length();
            }
            else
            {
                Projectile.ai[2] = -1; //cancel homing
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 0;
        }

        public override void OnKill(int timeLeft)
        {
            for (int num321 = 0; num321 < 15; num321++)
            {
                int num322 = Dust.NewDust(Projectile.Center - Vector2.One * 10f, 50, 50, DustID.Blood, 0f, -2f, 0, default, 1f);
                Main.dust[num322].velocity /= 2f;
            }
            int num323 = 10;
            if (Main.rand.NextBool(10))
            {
                int num324 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.8f, 584, 1f);
                Main.gore[num324].timeLeft /= num323;
            }
            if (Main.rand.NextBool(10))
            {
                int num324 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.9f, 585, 1f);
                Main.gore[num324].timeLeft /= num323;
            }
            if (Main.rand.NextBool(10))
            {
                int num324 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 1f, 586, 1f);
                Main.gore[num324].timeLeft /= num323;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotationModifier = 0;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + rotationModifier;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation + rotationModifier, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}