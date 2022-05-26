using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class RazorbladeTyphoonFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Razorblade Typhoon");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.alpha = 100;
            Projectile.penetrate = -1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = Projectile.timeLeft;
                /*switch ((int)Projectile.ai[1])
                {
                    case 1: Projectile.DamageType = DamageClass.Melee; break;
                    case 2: Projectile.DamageType = DamageClass.Ranged; break;
                    case 3: Projectile.DamageType = DamageClass.Magic; break;
                    case 4: Projectile.minion = true; break;
                    case 5: Projectile.thrown = true; break;
                    case 6: Projectile.DamageType = DamageClass.Ranged; Projectile.timeLeft -= 420; break;
                    default: break;
                }*/
                Projectile.ai[1] = Projectile.velocity.Length();
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] / (2 * Math.PI * Projectile.ai[0] * ++Projectile.localAI[0]));

            if (Main.rand.NextBool(10))
            {
                //vanilla typhoon dust (ech)
                Vector2 vector2_1 = Projectile.velocity;
                vector2_1.Normalize();
                vector2_1.X *= Projectile.width;
                vector2_1.Y *= Projectile.height;
                vector2_1 /= 2;
                vector2_1 = vector2_1.RotatedBy(-1 * Math.PI / 6);
                vector2_1 += Projectile.Center;
                Vector2 vector2_2 = (Main.rand.NextFloat() * (float)Math.PI - (float)Math.PI / 2f).ToRotationVector2();
                vector2_2 *= Main.rand.Next(3, 8);
                int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity /= 4f;
                Main.dust[index2].velocity -= Projectile.velocity;
            }

            Projectile.rotation += 0.2f;// * (Projectile.velocity.X > 0f ? 1f : -1f);
            Projectile.frame++;
            if (Projectile.frame > 2)
                Projectile.frame = 0;

            Projectile.alpha = (int)(255f * (1f - Projectile.timeLeft / Projectile.localAI[1]));
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //target.immune[Projectile.owner] = 1;
            /*target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);*/
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void Kill(int timeLeft)
        {
            /*int num1 = 36;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                Vector2 vector2_1 = (Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + Projectile.Center;
                Vector2 vector2_2 = vector2_1 - Projectile.Center;
                int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity = vector2_2;
            }*/
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

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 250, 200) * Projectile.Opacity;
        }
    }
}