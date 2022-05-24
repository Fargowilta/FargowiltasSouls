using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosNebulaBlaze : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_634";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nebula Blaze");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 120 * 3;
            Projectile.hostile = true;
            Projectile.tileCollide = false;

            Projectile.extraUpdates = 2;
            Projectile.scale = 1.5f;
            CooldownSlot = 1;
        }

        public override void AI() //vanilla code echprimebegone
        {
            if (++Projectile.localAI[1] < 45 * 3
                && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.CosmosChampion>())
                && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center - Projectile.Center;
                float targetAngle = vel.ToRotation();
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, Projectile.ai[0]));
            }

            float num1 = 5f;
            float num2 = 250f;
            float num3 = 6f;
            Vector2 vector2_1 = new Vector2(8f, 10f);
            float num4 = 1.2f;
            Vector3 rgb = new Vector3(0.7f, 0.1f, 0.5f);
            int num5 = 4 * Projectile.MaxUpdates;
            int Type1 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
            int Type2 = (int)byte.MaxValue;
            if ((double)Projectile.ai[1] == 0.0)
            {
                Projectile.ai[1] = 1f;
                Projectile.localAI[0] = (float)-Main.rand.Next(48);
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
            }
            /*else if ((double)Projectile.ai[1] == 1.0 && Projectile.owner == Main.myPlayer)
            {
                
            }
            else if ((double)Projectile.ai[1] > (double)num1)
            {
                
            }*/
            if ((double)Projectile.ai[1] >= 1.0 && (double)Projectile.ai[1] < (double)num1)
            {
                ++Projectile.ai[1];
                if ((double)Projectile.ai[1] == (double)num1)
                    Projectile.ai[1] = 1f;
            }
            Projectile.alpha = Projectile.alpha - 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter = Projectile.frameCounter + 1;
            if (Projectile.frameCounter >= num5)
            {
                Projectile.frame = Projectile.frame + 1;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            //Lighting.AddLight(Projectile.Center, rgb);
            Projectile.rotation = Projectile.velocity.ToRotation();
            ++Projectile.localAI[0];
            if ((double)Projectile.localAI[0] == 48.0)
                Projectile.localAI[0] = 0.0f;
            else if (Projectile.alpha == 0)
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 vector2_2 = Vector2.UnitX * -30f;
                    Vector2 vector2_3 = -Vector2.UnitY.RotatedBy((double)Projectile.localAI[0] * 0.130899697542191 + 3.14159274101257, new Vector2()) * vector2_1 - Projectile.rotation.ToRotationVector2() * 10f;
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, Type2, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = num4;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + vector2_3 + Projectile.velocity * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.Center + Projectile.velocity * 2f * 8f - Main.dust[index2].position) * 2f + Projectile.velocity * 2f;
                    Main.dust[index2].velocity *= Projectile.MaxUpdates / 3;
                }
            }
            if (Main.rand.NextBool(12))
            {
                Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type1, 0.0f, 0.0f, 0, new Color(), 1.2f);
                Main.dust[index2].velocity *= 0.3f;
                Main.dust[index2].velocity *= Projectile.MaxUpdates / 3;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].position = Projectile.Center + vector2_2 * (float)Projectile.width / 2f;
                if (Main.rand.NextBool())
                    Main.dust[index2].fadeIn = 1.4f;
            }
        }

        public override void Kill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            /*int num1 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
            int Type1 = (int)byte.MaxValue;
            int Type2 = (int)byte.MaxValue;
            int num2 = 50;
            float Scale1 = 1.7f;
            float Scale2 = 0.8f;
            float Scale3 = 2f;
            Vector2 vector2 = (Projectile.rotation - 1.570796f).ToRotationVector2() * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = num2;
            Projectile.Center = Projectile.position;
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int Type3 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
                if (Projectile.type == 635)
                    Type3 = Utils.SelectRandom<int>(Main.rand, new int[3]
                    {
              242,
              59,
              88
                    });
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type3, 0.0f, 0.0f, 200, new Color(), Scale1);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 3f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity = dust2.velocity + vector2 * Main.rand.NextFloat();
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type1, 0.0f, 0.0f, 100, new Color(), Scale2);
                Main.dust[index3].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Dust dust3 = Main.dust[index3];
                dust3.velocity = dust3.velocity * 2f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].fadeIn = 1f;
                Main.dust[index3].color = Color.Crimson * 0.5f;
                Dust dust4 = Main.dust[index3];
                dust4.velocity = dust4.velocity + vector2 * Main.rand.NextFloat();
            }
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Type2, 0.0f, 0.0f, 0, new Color(), Scale3);
                Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * (float)Projectile.width / 3f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 0.5f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity = dust2.velocity + vector2 * (float)(0.600000023841858 + 0.600000023841858 * (double)Main.rand.NextFloat());
            }*/
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Berserked>(), 300);
                target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            }
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

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
            {
                Color color27 = Color.HotPink * Projectile.Opacity;
                color27.A = 0;
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