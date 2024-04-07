using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Will
{
    public class WillFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_711";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 60;
            Projectile.ignoreWater = true;
            //CooldownSlot = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            bool recolor = WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors;
            int dustID = recolor ? DustID.Shadowflame : DustID.Torch;

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, dustID, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, dustID, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default,
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity.X += 1f;
                Main.gore[gore].velocity.Y += 1f;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<WillChampion>()))
            {
                if (WorldSavingSystem.EternityMode)
                {
                    target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
                    target.AddBuff(ModContent.BuffType<MidasBuff>(), 300);
                }
                target.AddBuff(BuffID.Bleeding, 300);
            }
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy))
            {
                if (WorldSavingSystem.EternityMode)
                {
                    //target.AddBuff(BuffID.OnFire, 600);
                    //target.AddBuff(BuffID.Ichor, 600);
                    target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
                    target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
                    target.AddBuff(BuffID.Burning, 300);
                }
            }
            Projectile.timeLeft = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.betsyBoss, NPCID.DD2Betsy) && WorldSavingSystem.EternityMode && SoulConfig.Instance.BossRecolors)
            {
                texture2D13 = TextureAssets.Projectile[ProjectileID.DD2BetsyFireball].Value;
            }
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Color.White;
            color26 = Projectile.GetAlpha(color26);
            color26.A = (byte)Projectile.alpha;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                float lerpamount = 0;
                if (i > 3 && i < 5)
                    lerpamount = 0.6f;
                if (i >= 5)
                    lerpamount = 0.8f;

                Color color27 = Color.Lerp(Color.White, Color.Purple, lerpamount) * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}