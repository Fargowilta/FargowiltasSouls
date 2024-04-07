using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Reticle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;

            //CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<CosmosChampion>());
            if (npc == null || npc.ai[0] != 11)
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[npc.target];

            Projectile.velocity = Vector2.Zero;

            if (++Projectile.ai[1] > 45)
            {
                if (Projectile.ai[1] % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);

                    if (FargoSoulsUtil.HostCheck) //rain meteors
                    {
                        Vector2 spawnPos = Projectile.Center;
                        spawnPos.X += Main.rand.Next(-200, 201);
                        spawnPos.Y -= 700;
                        Vector2 vel = Main.rand.NextFloat(10, 15f) * Vector2.Normalize(Projectile.Center - spawnPos);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawnPos, vel, ModContent.ProjectileType<CosmosMeteor>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, 0f, Main.rand.NextFloat(1f, 1.5f));
                    }
                }

                if (Projectile.ai[1] > 90)
                {
                    Projectile.ai[1] = 0;
                    Projectile.localAI[0] = 0;
                    Projectile.netUpdate = true;
                }

                Projectile.rotation = 0;
                Projectile.alpha = 0;
                Projectile.scale = 1;
            }
            else
            {
                Projectile.Center = player.Center;
                Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], player.velocity.X * 30, 0.1f);
                Projectile.position.X += Projectile.localAI[0];

                if (Projectile.ai[1] == 45)
                {
                    Projectile.netUpdate = true;

                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -5);
                }

                float spindown = 1f - Projectile.ai[1] / 45f;
                Projectile.rotation = MathHelper.TwoPi * 1.5f * spindown;
                Projectile.alpha = (int)(255 * spindown);
                Projectile.scale = 1 + 2 * spindown;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}