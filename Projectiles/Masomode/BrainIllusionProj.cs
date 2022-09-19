using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BrainIllusionProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_266";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;

            Projectile.scale += 0.25f;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] == 2f;
        }

        private const int attackDelay = 120;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.BrainofCthulhu);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }

            if (Projectile.frame < 4 || Projectile.frame > 7)
                Projectile.frame = 4;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha = (int)(255f * npc.life / npc.lifeMax);
            }
            else if (Projectile.ai[1] == 1f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 0, 0.02f);

                Projectile.position += 0.5f * (Main.player[npc.target].position - Main.player[npc.target].oldPosition) * (1f - Projectile.localAI[0] / attackDelay);
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 180;

                if (++Projectile.localAI[0] > attackDelay)
                {
                    Projectile.ai[1] = 2f;
                    Projectile.velocity = 18f * Projectile.DirectionTo(Main.player[npc.target].Center);
                    Projectile.netUpdate = true;

                    Projectile.localAI[0] = Main.player[npc.target].Center.X;
                    Projectile.localAI[1] = Main.player[npc.target].Center.Y;
                }
            }
            else
            {
                Projectile.alpha = 0;
                Projectile.velocity *= 1.015f;

                if (Projectile.Distance(new Vector2(Projectile.localAI[0], Projectile.localAI[1])) < Projectile.velocity.Length() + 1f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            /*if (Projectile.ai[1] == 2f)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1);

                for (int i = 0; i < 25; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 5);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 2f;
                }
            }*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].Value; //Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.npcFrameCount[NPCID.BrainofCthulhu]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color = Projectile.GetAlpha(lightColor);

            if (CanDamage() == true)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Vector2 warningShake;
            if (Projectile.ai[1] == 1f)
            {
                float radius = 16f * Projectile.localAI[0] / attackDelay;
                warningShake = Main.rand.NextVector2Circular(radius, radius);
            }
            else
            {
                warningShake = Vector2.Zero;
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + warningShake - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}