using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class Bloodshed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodshed");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            if (Projectile.ai[0] == 0) //shed by player, buff enemies
            {
                if (Projectile.velocity.Length() < 2)
                {
                    foreach (NPC n in Main.npc.Where(n => n.active && !n.friendly && n.lifeMax > 5 && !n.immortal && n.damage > 0))
                    {
                        if (Projectile.Colliding(Projectile.Hitbox, n.Hitbox))
                        {
                            n.AddBuff(ModContent.BuffType<BloodDrinker>(), 360);
                            Projectile.ai[1] = 1;
                            Projectile.netUpdate = true;
                            Projectile.Kill();
                            return;
                        }
                    }
                }
            }
            else //shed by enemy, buff player
            {
                Projectile.timeLeft -= 3;

                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1 && p != Main.maxPlayers && Main.player[p].active && !Main.player[p].dead && !Main.player[p].ghost)
                {
                    if (Main.player[p].Distance(Projectile.Center) < 360)
                    {
                        Projectile.velocity = Projectile.DirectionTo(Main.player[p].Center) * 9f;

                        if (Projectile.Colliding(Projectile.Hitbox, Main.player[p].Hitbox))
                        {
                            Main.player[p].AddBuff(ModContent.BuffType<BloodDrinker>(), 360);
                            Projectile.ai[1] = 1;
                            Projectile.netUpdate = true;
                            Projectile.Kill();
                            return;
                        }
                    }
                }
            }

            if (Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] > 50)
            {
                Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] -= 1;
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            Lighting.AddLight(Projectile.Center, TorchID.Crimson);
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[1] == 1)
                SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.Center);

            int max = Projectile.ai[1] == 1 ? 20 : 10;
            for (int i = 0; i < max; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
                Main.dust[d].velocity *= Projectile.ai[1] == 1 ? 2.5f : 1.5f;
                Main.dust[d].scale += Projectile.ai[1] == 1 ? 1.5f : 0.5f;
            }
        }

        public override Color? GetAlpha(Color lightColor) => base.GetAlpha(lightColor); //Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}