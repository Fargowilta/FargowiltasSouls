using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class TerraLightningOrb2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_465";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.scale = 0.5f;
            CooldownSlot = 1;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) <= Projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.position = Projectile.Center;

            Projectile.width = (int)(Projectile.width / Projectile.scale);
            Projectile.height = (int)(Projectile.height / Projectile.scale);

            Projectile.scale = reader.ReadSingle();

            Projectile.width = (int)(Projectile.width * Projectile.scale);
            Projectile.height = (int)(Projectile.height * Projectile.scale);

            Projectile.Center = Projectile.position;
        }
        bool firsttick = false;
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (!firsttick)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy(2 * (float)Math.PI / 8 * i);
                    Vector2 vel = Vector2.Normalize(dir);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<TerraLightningOrbDeathray>(),
                        Projectile.damage, 0, Main.myPlayer, dir.ToRotation(), Projectile.whoAmI);
                }
                Projectile.rotation = Projectile.localAI[0];
                firsttick = true;
            }

            if (Projectile.localAI[0] > 0) //rotate fast, then slow down over time
            {
                Projectile.rotation += Projectile.localAI[1] * (6 - Projectile.scale) * 0.012f;
            }

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.TerraChampion>());
            if (npc != null)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                Projectile.velocity = 4f * Projectile.DirectionTo(Main.player[npc.target].Center);

                if (++Projectile.ai[1] > 60) //grow
                {
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;

                    Projectile.position = Projectile.Center;

                    Projectile.width = (int)(Projectile.width / Projectile.scale);
                    Projectile.height = (int)(Projectile.height / Projectile.scale);

                    Projectile.scale++;

                    Projectile.width = (int)(Projectile.width * Projectile.scale);
                    Projectile.height = (int)(Projectile.height * Projectile.scale);

                    Projectile.Center = Projectile.position;

                    MakeDust();
                    SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                }
            }
            else
            {
                if (Projectile.timeLeft < 2)
                    Projectile.timeLeft = 2;

                Projectile.alpha += 10;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool(3))
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }
        }

        private void MakeDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * Projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * Projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            for (int i = 0; i < 80; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 10f * Projectile.scale;
                vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * 6.28318548f / 80) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 92, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

            MakeDust();

            if (!Main.dedServ)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

            if (Projectile.alpha == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Thunder") { Volume = 0.8f, Pitch = 0.5f }, Projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedBy((2 * (float)Math.PI / 8 * i) + Projectile.rotation);
                    float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                    Vector2 vel = Vector2.Normalize(dir) * 54f;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vel, ModContent.ProjectileType<HostileLightning>(),
                        Projectile.damage, 0, Main.myPlayer, dir.ToRotation(), ai1New / 2);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}