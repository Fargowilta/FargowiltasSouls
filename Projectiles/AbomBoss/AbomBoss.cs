using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomBoss : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/AbomBoss/AbomBoss";
        public int npcType => mod.NPCType("AbomBoss");

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 50;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1], npcType);
            if (npc != null)
            {
                projectile.Center = npc.Center;
                projectile.alpha = npc.alpha;
                projectile.direction = projectile.spriteDirection = npc.direction;
                projectile.timeLeft = 30;
                projectile.localAI[0] = npc.localAI[3]; //in p2?

                if (!Main.dedServ)
                    projectile.frame = (int)(npc.frame.Y / (float)(Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]));
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    projectile.Kill();
                return;
            }
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            bool glowAura = projectile.localAI[0] > 1 && FargoSoulsWorld.EternityMode;

            if (glowAura)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

                Terraria.Graphics.Shaders.ArmorShaderData shader = Terraria.Graphics.Shaders.GameShaders.Armor.GetShaderFromItemId(ItemID.PurpleDye);
                shader.Apply(projectile, new Terraria.DataStructures.DrawData?());
            }

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27;
                if (glowAura)
                {
                    color27 = Color.White;
                    color27 *= 0.5f + 0.5f * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    color27.A = 0;
                }
                else
                {
                    color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                }
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            if (glowAura)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}