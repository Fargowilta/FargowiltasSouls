using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Nebula
{
    public class NebulaPillarProj : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/CelestialPillar";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Celestial Pillar");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            CooldownSlot = 1;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }
        private int Timer = 0;
        const int Distance = 500;
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                var type = DustID.PinkTorch;
                for (int index = 0; index < 50; ++index)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                    dust.velocity *= 10f;
                    dust.fadeIn = 1f;
                    dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                    if (!Main.rand.NextBool(3))
                    {
                        dust.noGravity = true;
                        dust.velocity *= 3f;
                        dust.scale *= 2f;
                    }
                }
            }

            ref float num = ref Projectile.ai[0];
            ref float fake = ref Projectile.ai[1];
            ref float npcIndex = ref Projectile.ai[2];

            const int StartTime = 60 * 2;
            const int ReactTime = 40;
            const int DeathrayTime = 60 * 2;

            if (!Main.npc[(int)npcIndex].active)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Main.npc[(int)npcIndex].target];
            if (!player.active || player.ghost)
            {
                return;
            }
            //startup

            if (Timer <= StartTime)
            {
                if (Projectile.alpha > 105)
                {
                    Projectile.rotation = (Vector2.UnitY).ToRotation();
                    Projectile.alpha -= 10;
                }
                else
                {
                    Projectile.alpha = 105;
                }
                float rotation = (Timer / 60f) * MathHelper.PiOver2 + (MathHelper.PiOver2 * num);
                Vector2 pos = player.Center + rotation.ToRotationVector2() * Distance;

                Projectile.Center = pos;
                if (Timer == 0)
                {
                    Projectile.rotation = pos.SafeDirectionTo(player.Center).ToRotation();
                }
                RotateTowards(player.Center, 6);
            }
            else if (fake == 1)
            {
                Projectile.Kill();
            }
            if (Timer > StartTime && fake != 1)
            {
                if (Timer < StartTime + ReactTime)
                {
                    float TrackingSpeed = WorldSavingSystem.MasochistModeReal ? 1 : 0.2f;
                    RotateTowards(player.Center, TrackingSpeed);
                }
                if (Timer == StartTime + 1)
                {
                    SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                    Projectile.alpha = 0;
                }
                if (Timer == StartTime + ReactTime)
                {
                    SoundEngine.PlaySound(SoundID.Zombie104 with { Volume = 0.5f }, Projectile.Center);
                    if (FargoSoulsUtil.HostCheck)
                    {
                        float rotation = Projectile.rotation + MathHelper.Pi;
                        Vector2 pos = Projectile.Center + (rotation.ToRotationVector2() * Projectile.height / 2);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), pos, rotation.ToRotationVector2(),
                                ModContent.ProjectileType<NebulaDeathray>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
                if (Timer > StartTime + ReactTime + DeathrayTime)
                {
                    Projectile.Kill();
                }
            }
            Timer++;
            Projectile.frame = 0;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            var type = DustID.PinkTorch;
            for (int index = 0; index < 80; ++index)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 0.0f, 0.0f, 0, new Color(), 1f)];
                dust.velocity *= 10f;
                dust.fadeIn = 1f;
                dust.scale = 1 + Main.rand.NextFloat() + Main.rand.Next(4) * 0.3f;
                if (!Main.rand.NextBool(3))
                {
                    dust.noGravity = true;
                    dust.velocity *= 3f;
                    dust.scale *= 2f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255 - Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
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
        void RotateTowards(Vector2 target, float speed)
        {
            Vector2 PV = Projectile.SafeDirectionTo(target).RotatedBy(MathHelper.Pi); //offset because projectile funny
            Vector2 LV = Projectile.rotation.ToRotationVector2();
            float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
                                                                                                         //change rotation towards target
            Projectile.rotation = Projectile.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
    }
}