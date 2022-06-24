using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviRainHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fake Heart");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            CooldownSlot = 1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (Projectile.ai[0] == 0)
            {
                if (npc != null && npc.HasPlayerTarget && Projectile.position.Y >= Main.player[npc.target].position.Y - 350)
                {
                    Projectile.velocity.Normalize();
                    Projectile.ai[0] = 1;
                    Projectile.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                }
            }
            else
            {
                //Projectile.tileCollide = true;

                if (++Projectile.ai[0] < 61)
                {
                    Projectile.velocity *= FargoSoulsWorld.MasochistModeReal ? 1.06f : 1.05f;
                }

                if (npc != null && Projectile.Center.Y > Main.player[npc.target].Center.Y + 280) //break when far below player
                {
                    Projectile.Kill();
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        /*public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            int ai1 = (int)Projectile.ai[1];
            if (Projectile.ai[1] >= 0f && Projectile.ai[1] < 200f &&
                Main.npc[ai1].active && Main.npc[ai1].type == ModContent.NPCType<DeviBoss>())
            {
                fallThrough = Projectile.Center.Y < Main.player[Main.npc[ai1].target].Center.Y + 160;
            }

            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }*/

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            //FargoSoulsUtil.HeartDust(Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 86, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 8f;
            }
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
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}