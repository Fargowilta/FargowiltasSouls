using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
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
            DisplayName.SetDefault("Fish Nuke");
        }

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 320;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            CooldownSlot = -1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCheck =
                projectile =>
                {
                    return CanDamage() == true && Math.Abs((Main.LocalPlayer.Center - Projectile.Center).Length() - safeRange) < Player.defaultHeight + Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().GrazeRadius;
                };

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0f && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantBomb>()] > 0;
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
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
            if (npc != null && npc.ai[0] == 34)
            {
                Projectile.alpha -= 7;
                Projectile.timeLeft = 300;
                Projectile.Center = npc.Center;
                Projectile.position.Y -= 100;
            }
            else
            {
                Projectile.alpha += 17;
            }

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
            {
                Projectile.alpha = 255;
                Projectile.Kill();
                return;
            }
            Projectile.scale = 1f - Projectile.alpha / 255f;
            Projectile.rotation += (float)Math.PI / 70f;

            /*int num1 = (300 - Projectile.timeLeft) / 60;
            float num2 = Projectile.scale * 0.4f;
            float num3 = Main.rand.Next(1, 3);
            Vector2 vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
            vector2.Normalize();
            int index2 = Dust.NewDust(Projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].noLight = true;
            Main.dust[index2].velocity = vector2 * num3;
            if (Main.rand.NextBool())
            {
                Main.dust[index2].velocity *= 2f;
                Main.dust[index2].scale += 0.5f;
            }
            Main.dust[index2].fadeIn = 2f;*/

            Lighting.AddLight(Projectile.Center, 0.4f, 0.9f, 1.1f);

            if (Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD > 10)
                Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 10;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}