using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomStyxGazer : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/StyxGazer";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Styx Gazer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        const int maxTime = 60;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = maxTime;
            //Projectile.alpha = 250;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;

            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.hide = false; //to avoid edge case tick 1 wackiness

            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
            if (npc != null)
            {
                if (npc.ai[0] == 0) Projectile.extraUpdates = 1;

                if (Projectile.localAI[0] == 0)
                    Projectile.localAI[1] = Projectile.ai[1] / maxTime; //do this first

                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);
                Projectile.ai[1] -= Projectile.localAI[1];
                Projectile.Center = npc.Center + new Vector2(60, 60).RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4) * Projectile.scale;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                /*Vector2 basePos = Projectile.Center - Projectile.velocity * 141 / 2 * Projectile.scale;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(basePos + Projectile.velocity * Main.rand.NextFloat(127) * Projectile.scale, 0, 0, 87, Scale: 3f);
                    Main.dust[d].velocity *= 4.5f;
                    Main.dust[d].noGravity = true;
                }*/

                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }

            /*if (Projectile.timeLeft == maxTime - 20)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Player.FindClosest(Projectile.Center, 0, 0);
                    if (p != -1)
                    {
                        Vector2 vel = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 30f;
                        int max = 8;
                        for (int i = 0; i < max; i++)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(MathHelper.TwoPi / max * i), ModContent.ProjectileType<AbomSickle3>(), Projectile.damage, Projectile.knockBack, Projectile.owner, p);
                        }
                    }
                }
            }*/

            Projectile.Opacity = (float)Math.Min(1, (2 - Projectile.extraUpdates) * Math.Sin(Math.PI * (maxTime - Projectile.timeLeft) / maxTime));

            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(Projectile.direction < 0 ? 135 : 45);
            //Main.NewText(MathHelper.ToDegrees(Projectile.velocity.ToRotation()) + " " + MathHelper.ToDegrees(Projectile.ai[1]));
        }

        public override void Kill(int timeLeft)
        {
            /*Vector2 basePos = Projectile.Center - Projectile.velocity * 141 / 2 * Projectile.scale;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(basePos + Projectile.velocity * Main.rand.NextFloat(127) * Projectile.scale, 0, 0, 87, Scale: 3f);
                Main.dust[d].velocity *= 4.5f;
                Main.dust[d].noGravity = true;
            }*/
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                //target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
                target.AddBuff(ModContent.BuffType<Buffs.Boss.AbomFang>(), 300);
                //target.AddBuff(ModContent.BuffType<Unstable>(), 240);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Berserked>(), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor * Projectile.Opacity;
            color.A = (byte)(255 * Projectile.Opacity);
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Items/Weapons/FinalUpgrades/StyxGazer_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}