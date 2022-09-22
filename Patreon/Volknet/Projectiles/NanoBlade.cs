using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    class NanoBlade : ModProjectile
    {
        public override void SetStaticDefaults()           //320,47
        {
            DisplayName.SetDefault("Plasma Blade");
            //DisplayName.AddTranslation(GameCulture.Chinese, "等离子能量刃");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.light = 1.1f;
            Projectile.scale = 1.75f;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 0 && owner.channel)
            {
                Projectile.frame = (int)Projectile.localAI[0];
                Rectangle Frame = new Rectangle(0, (int)Projectile.localAI[0] * 47, 340, 47);
                Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 RandomPos = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2();
                Main.EntitySpriteDraw(tex, RandomPos + owner.Center + Projectile.rotation.ToRotationVector2() * (160 * Projectile.scale - 24) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), Frame, new Color(255, 255, 255, 0) * Projectile.Opacity * Main.rand.NextFloat(0.6f, 1f), Projectile.rotation, Frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (320 - 24) * Projectile.scale, 30, ref point);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            target.AddBuff(BuffID.WitheredArmor, 600);
            target.AddBuff(BuffID.BrokenArmor, 600);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float num1 = 0.33f;
            for (int i = 0; i < 9; i++)
            {
                if (Main.rand.NextFloat() >= num1)
                {
                    float f = Main.rand.NextFloat() * MathHelper.TwoPi;
                    float num2 = Main.rand.NextFloat();
                    Dust dust = Dust.NewDustPerfect(target.Center, 157, (f - MathHelper.Pi).ToRotationVector2() * (14 + 8 * num2), 0, default, 1f);  //GreenFx
                    dust.scale = 0.9f;
                    dust.fadeIn = 1.15f + num2 * 0.3f;
                    dust.noGravity = true;
                }
            }
        }
        public override void AI()
        {
            CastLights();
            if (Main.player[Projectile.owner].active)
            {
                Player owner = Main.player[Projectile.owner];
                if (!owner.dead && owner.HeldItem.type == ModContent.ItemType<NanoCore>())
                {
                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 0 && owner.channel)
                    {
                        Projectile.ai[0] = (Projectile.ai[0] + 1) % 30;
                        if (Projectile.ai[0] == 15)
                        {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item15, owner.Center);
                        }
                        Projectile.timeLeft = 2;
                        Projectile.Center = owner.Center;
                        Projectile.rotation = (Main.MouseWorld - owner.Center).ToRotation();

                        Projectile.localAI[0] += 0.5f;
                        if (Projectile.localAI[0] > 3) Projectile.localAI[0] = 3;
                    }
                    else
                    {
                        Projectile.Kill();
                    }

                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Kill();
            }
            Projectile.alpha = 0;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * 320, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
        }

        private void CastLights()
        {
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 320, Projectile.width, DelegateMethods.CastLight);
        }
    }
}