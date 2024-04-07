using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Spirit
{
    public class SpiritHand : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Spirit/SpiritChampionHand";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;

            CooldownSlot = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch,
                        Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                    Main.dust[d].velocity *= 6f;
                }
            }

            if (++Projectile.localAI[0] > 30 && Projectile.localAI[0] < 120)
            {
                Projectile.velocity *= Projectile.ai[0];
            }

            if (Projectile.localAI[0] > 60 && Projectile.localAI[0] < 180)
            {
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<SpiritChampion>()))
                {
                    float rotation = Projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center - Projectile.Center;
                    float targetAngle = vel.ToRotation();
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, Projectile.ai[1]));
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.spriteDirection < 0)
                Projectile.rotation += (float)Math.PI;

            Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 0, default, 2f)].noGravity = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<InfestedBuff>(), 360);
                target.AddBuff(ModContent.BuffType<ClippedWingsBuff>(), 180);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}