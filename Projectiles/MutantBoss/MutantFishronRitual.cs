using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantFishronRitual : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FishronRitual";

        private const int safeRange = 150;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fish Nuke");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 320;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            cooldownSlot = -1;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCheck =
                projectile =>
                {
                    return CanDamage() && Math.Abs((Main.LocalPlayer.Center - projectile.Center).Length() - safeRange) < Player.defaultHeight + Main.LocalPlayer.GetModPlayer<FargoPlayer>().GrazeRadius;
                };

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool CanDamage()
        {
            return projectile.alpha == 0f && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantBomb>()] > 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((projHitbox.Center.ToVector2() - targetHitbox.Center.ToVector2()).Length() < safeRange)
                return false;

            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;
            
            return Math.Sqrt(dX * dX + dY * dY) <= 1200;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
            if (npc != null && npc.ai[0] == 34)
            {
                projectile.alpha -= 7;
                projectile.timeLeft = 300;
                projectile.Center = npc.Center;
                projectile.position.Y -= 100;
            }
            else
            {
                projectile.alpha += 17;
            }

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
            {
                projectile.alpha = 255;
                projectile.Kill();
                return;
            }
            projectile.scale = 1f - projectile.alpha / 255f;
            projectile.rotation += (float)Math.PI / 70f;

            /*int num1 = (300 - projectile.timeLeft) / 60;
            float num2 = projectile.scale * 0.4f;
            float num3 = Main.rand.Next(1, 3);
            Vector2 vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
            vector2.Normalize();
            int index2 = Dust.NewDust(projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].noLight = true;
            Main.dust[index2].velocity = vector2 * num3;
            if (Main.rand.NextBool())
            {
                Main.dust[index2].velocity *= 2f;
                Main.dust[index2].scale += 0.5f;
            }
            Main.dust[index2].fadeIn = 2f;*/

            Lighting.AddLight(projectile.Center, 0.4f, 0.9f, 1.1f);

            if (projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD > 10)
                projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD = 10;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 100;
                target.AddBuff(mod.BuffType("OceanicMaul"), 5400);
                target.AddBuff(mod.BuffType("MutantFang"), 180);
            }
            target.AddBuff(mod.BuffType("MutantNibble"), 900);
            target.AddBuff(mod.BuffType("CurseoftheMoon"), 900);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}