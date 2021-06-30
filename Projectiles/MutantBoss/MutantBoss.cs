using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantBoss : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/MutantBoss/MutantBoss";

        public string trailTexture => "NPCs/Eternals/MutantSoul";
        public int npcType => mod.NPCType("MutantBoss");
        public bool auraTrail;

        const int auraFrames = 19;
        //const int lightningFrames = 20;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 50;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            if (projectile.hide)
                drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            int ai1 = (int)projectile.ai[1];
            if (projectile.ai[1] >= 0f && projectile.ai[1] < Main.maxNPCs &&
                Main.npc[ai1].active && Main.npc[ai1].type == npcType)
            {
                projectile.Center = Main.npc[ai1].Center;
                projectile.alpha = Main.npc[ai1].alpha;
                projectile.direction = projectile.spriteDirection = Main.npc[ai1].direction;
                projectile.timeLeft = 30;
                auraTrail = DisplayAura(Main.npc[ai1]);

                /*switch((int)Main.npc[ai1].ai[0]) //draw behind whenever holding a weapon
                {
                    case 0:
                    case 4:
                    case 5:
                    case 6:
                    case 13:
                    case 14:
                    case 15:
                    case 21:
                    case 22:
                    case 23:
                    case 25:
                    case 36:
                    case 41:
                        projectile.hide = true;
                        break;

                    default:
                        projectile.hide = false;
                        break;
                }*/

                projectile.hide =
                    Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearAim>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearDash>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSpearSpin>()] > 0
                    || Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MutantSlimeRain>()] > 0;

                if (!Main.dedServ)
                    projectile.frame = (int)(Main.npc[ai1].frame.Y / (float)(Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]));

                if (Main.npc[ai1].frameCounter == 0)
                {
                    if (++projectile.localAI[0] >= auraFrames)
                        projectile.localAI[0] = 0;
                    //if (++projectile.localAI[1] >= lightningFrames) projectile.localAI[1] = 0;
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    projectile.Kill();
                return;
            }
        }

        public override void Kill(int timeLeft)
        {
            /*Main.NewText("i die now");
            if (Main.netMode == NetmodeID.Server)
                NetMessage.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("i die now aaaaaa"), Color.LimeGreen);*/
        }

        public bool DisplayAura(NPC npc)
        {
            return npc.ai[0] < 0 || npc.ai[0] > 9;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D texture2D14 = mod.GetTexture(trailTexture);
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Texture2D aura = mod.GetTexture("NPCs/MutantBoss/MutantAura");
            int auraFrameHeight = aura.Height / auraFrames;
            int auraY = auraFrameHeight * (int)projectile.localAI[0];
            Rectangle auraRectangle = new Rectangle(0, auraY, aura.Width, auraFrameHeight);

            /*Texture2D lightning = mod.GetTexture("NPCs/MutantBoss/MutantLightning");
            int lightningFrameHeight = lightning.Height / lightningFrames;
            int lightningY = lightningFrameHeight * (int)projectile.localAI[0];
            Rectangle lightningRectangle = new Rectangle(0, lightningY, lightning.Width, lightningFrameHeight);*/

            Color color26 = projectile.GetAlpha(projectile.hide && Main.netMode == NetmodeID.MultiplayerClient ? Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16) : lightColor);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (auraTrail)
            {
                float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.9f;
                scale *= projectile.scale;

                Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, scale, effects, 0f);

                for (float i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.5f)
                {
                    Color color27 = Color.White * projectile.Opacity * 0.75f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                    if (max0 < 0)
                        continue;
                    Vector2 value4 = projectile.oldPos[max0];
                    float num165 = projectile.oldRot[max0];
                    Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], 1 - i % 1);
                    center += projectile.Size / 2;
                    Main.spriteBatch.Draw(texture2D14, center - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
                }

                Main.spriteBatch.Draw(aura, -16 * Vector2.UnitY + projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(auraRectangle), Color.White * projectile.Opacity, projectile.rotation, auraRectangle.Size() / 2f, scale, effects, 0f);
            }
            else
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);

            //if (auraTrail) Main.spriteBatch.Draw(lightning, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(lightningRectangle), Color.White * projectile.Opacity, projectile.rotation, lightningRectangle.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}