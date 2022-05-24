using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class PlanteraCrystalLeafRing : MutantBoss.MutantCrystalLeaf
    {
        public override string Texture => "Terraria/Images/Projectile_226";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1.5f;
            CooldownSlot = -1;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] == 0)
            {
                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 107 : 157, 0f, 0f, 0, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 5f;
                }
            }

            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);
            Projectile.scale = (Main.mouseTextColor / 200f - 0.35f) * 0.2f + 0.95f;
            Projectile.scale *= 1.5f;

            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[0], ModContent.ProjectileType<MutantBoss.MutantMark2>());
            if (byIdentity != -1)
            {
                Vector2 offset = new Vector2(100, 0).RotatedBy(Projectile.ai[1]);
                Projectile.Center = Main.projectile[byIdentity].Center + offset;

                Projectile.localAI[1] = Math.Max(0, 150 - Main.projectile[byIdentity].ai[1]) / 150; //rampup
                if (Projectile.localAI[1] > 1f) //clamp it for use in draw
                    Projectile.localAI[1] = 1f;
                Projectile.ai[1] += 0.15f * Projectile.localAI[1];
            }

            Projectile.rotation = Projectile.ai[1] + (float)Math.PI / 2f;

            if (Projectile.localAI[0] > 20)
            {
                Projectile.localAI[0] = 1;
                NPC plantera = FargoSoulsUtil.NPCExists(NPC.plantBoss, NPCID.Plantera);
                if (plantera != null && Projectile.Distance(plantera.Center) < 1600f && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, 4f * Projectile.ai[1].ToRotationVector2(), ModContent.ProjectileType<CrystalLeafShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<IvyVenom>(), 240);
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

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * Projectile.localAI[1];
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}