using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearThrown : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        //throw with 25 velocity, 1000 damage, 10 knockback

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        float scaletimer;
        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 15, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            if (--Projectile.localAI[0] < 0)
            {
                Projectile.localAI[0] = 3;
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 baseVel = Vector2.Normalize(Projectile.velocity).RotatedBy(Math.PI / 2);

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 16f * baseVel,
                        ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, 1f);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = Projectile.DamageType;

                    p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 16f * -baseVel,
                        ModContent.ProjectileType<PhantasmalSphere>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, 1f);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = Projectile.DamageType;

                    /*Vector2 vel = baseVel.RotatedByRandom(Math.PI / 4);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 8f * vel, ModContent.ProjectileType<HentaiSpearArc>(), 
                        Projectile.damage, Projectile.knockBack / 2, Projectile.owner, vel.ToRotation(), Projectile.timeLeft / Projectile.MaxUpdates);
                    vel = -baseVel.RotatedByRandom(Math.PI / 4);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 8f * vel, ModContent.ProjectileType<HentaiSpearArc>(),
                        Projectile.damage, Projectile.knockBack / 2, Projectile.owner, vel.ToRotation(), Projectile.timeLeft / Projectile.MaxUpdates);*/
                }
            }

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity),
                        ModContent.ProjectileType<HentaiSpearDeathray>(), Projectile.damage, Projectile.knockBack,
                        Projectile.owner, 0f, Projectile.velocity.Length() * Projectile.MaxUpdates);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].DamageType = Projectile.DamageType;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            scaletimer++;
        }

        public override bool? CanDamage()
        {
            Projectile.maxPenetrate = 1;
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)), Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, 0f, Projectile.owner);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].DamageType = Projectile.DamageType;
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantEye_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int rect1 = glow.Height / Main.projFrames[Projectile.type];
            int rect2 = rect1 * Projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(51, 255, 191, 0), Color.Transparent, 0.82f);
            Color glowcolor2 = Color.Lerp(new Color(194, 255, 242, 0), Color.Transparent, 0.6f);
            glowcolor = Color.Lerp(glowcolor, glowcolor2, 0.5f + (float)Math.Sin(scaletimer / 7) / 2); //make it shift between the 2 colors
            Vector2 drawCenter = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 28);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the Projectile
            {
                Vector2 drawCenter2 = drawCenter + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the Projectile's velocity to offset it at different angles
                drawCenter2 -= (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20); //then move it backwards
                float scale = Projectile.scale;
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Main.EntitySpriteDraw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0);
            }

            for (int i = ProjectileID.Sets.TrailCacheLength[Projectile.type] - 1; i > 0; i--) //scaling trail
            {

                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Vector2 value4 = Projectile.oldPos[i] - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}