using FargowiltasSouls.Buffs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantScythe1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Sickle");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 0;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 1;

            Projectile.hide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVarInt(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadVarInt();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            targetHitbox.Y = targetHitbox.Center.Y;
            targetHitbox.Height = Math.Min(targetHitbox.Width, targetHitbox.Height);
            targetHitbox.Y -= targetHitbox.Height / 2;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            /*if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }*/
            if (Projectile.rotation == 0)
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            float modifier = (180f - Projectile.timeLeft + 90) / 180f; //2f - Projectile.timeLeft / 240f;
            if (modifier < 0)
                modifier = 0;
            if (modifier > 1)
                modifier = 1;
            Projectile.rotation += 0.1f + 0.7f * modifier;
            //Projectile.alpha = (int)(127f * (1f - modifier));

            if (Projectile.timeLeft < 180) //240)
            {
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity = Projectile.ai[1].ToRotationVector2();
                    Projectile.netUpdate = true;
                }
                Projectile.velocity *= 1f + Projectile.ai[0];
            }
            /*for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(Projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(Projectile.Center, 0, 0, 229, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = Projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void PostAI()
        {
            /*if (Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD == 6)
                Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 60;
            else if (Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD == 7)
                Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 5;*/
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            target.AddBuff(BuffID.Bleeding, 600);
        }
    }
}