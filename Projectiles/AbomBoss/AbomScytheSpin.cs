using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomScytheSpin : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_274";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            if (Projectile.timeLeft == 390)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            else if (Projectile.timeLeft == 360)
            {
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
            }
            else if (Projectile.timeLeft < 360)
            {
                NPC abom = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                if (abom == null)
                {
                    Projectile.Kill();
                    return;
                }
                Vector2 pivot = abom.Center;
                Projectile.velocity = (pivot - Projectile.Center).RotatedBy(Math.PI / 2 * Projectile.ai[1]);
                Projectile.velocity *= 2 * (float)Math.PI / 360;
            }

            Projectile.spriteDirection = (int)Projectile.ai[1];
            Projectile.rotation += Projectile.spriteDirection * 0.5f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            for (int index1 = 0; index1 < 20; ++index1) //put some dust here ig
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale++;
                Main.dust[index2].velocity *= 4f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) //fire at player
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Vector2 speed = /*15f * */ Projectile.DirectionTo(Main.player[p].Center);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<AbomSickle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFang>(), 300);
                //target.AddBuff(ModContent.BuffType<Unstable>(), 240);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Berserked>(), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
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
            return Color.White;
        }
    }
}