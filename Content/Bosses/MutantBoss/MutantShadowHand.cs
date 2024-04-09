using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantShadowHand : MutantFishron
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantShadowHand_April" :
            "Terraria/Images/Projectile_965";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.InsanityShadowHostile];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = Projectile.height = 50;
            Projectile.alpha = 0;
            Projectile.scale = 1.5f;
        }

        public override bool PreAI() => true;

        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1;
                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost, Projectile.Center);
                p = LumUtils.AnyBosses() ? Main.npc[FargoSoulsGlobalNPC.boss].target : Player.FindClosest(Projectile.Center, 0, 0);
                Projectile.netUpdate = true;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (++Projectile.localAI[0] > 85) //dash
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }
            else //preparing to dash
            {
                int ai0 = p;
                //const float moveSpeed = 1f;
                if (Projectile.localAI[0] == 85) //just about to dash
                {
                    Projectile.velocity = Main.player[ai0].Center - Projectile.Center;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Projectile.type == ModContent.ProjectileType<MutantFishron>() ? 24f : 20f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
                }
                else //regular movement
                {
                    Vector2 vel = Main.player[ai0].Center - Projectile.Center;
                    Projectile.rotation += Projectile.velocity.Length() / 20f;
                    Projectile.rotation = Projectile.rotation.AngleLerp(
                        (Main.player[ai0].Center - Projectile.Center).ToRotation(), Projectile.localAI[0] / 85f * 0.08f);
                    if (vel.X > 0) //projectile is on left side of target
                    {
                        vel.X -= 300;
                        Projectile.direction = Projectile.spriteDirection = 1;
                    }
                    else //projectile is on right side of target
                    {
                        vel.X += 300;
                        Projectile.direction = Projectile.spriteDirection = -1;
                    }
                    Vector2 targetPos = Main.player[ai0].Center + new Vector2(Projectile.ai[0], Projectile.ai[1]);
                    Vector2 distance = (targetPos - Projectile.Center) / 4f;
                    Projectile.velocity = (Projectile.velocity * 19f + distance) / 20f;
                    Projectile.position += Main.player[ai0].velocity / 2f;
                }
            }
        }


        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.FargoSouls().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            }
            target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 900);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Color.Black * Projectile.Opacity;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float drawRotation = 0;
            if (Projectile.spriteDirection < 0)
                drawRotation += (float)Math.PI;

            float opacityMod = Projectile.localAI[0] > 85 ? 0.6f : 0.3f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.LightBlue * Projectile.Opacity * opacityMod;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + drawRotation;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale * 1.2f, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation + drawRotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}