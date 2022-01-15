using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class CrystalSkull : ModProjectile
    {
        public override string Texture => "Terraria/NPC_289";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Crystal Skull");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 50;
            projectile.height = 50;
            projectile.timeLeft *= 5;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.scale = 0.5f;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        int clickTimer;

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoPlayer>().CrystalSkullMinion)
                projectile.timeLeft = 2;

            if (projectile.damage == 0)
                projectile.damage = (int)(30f * player.minionDamage);

            Vector2 vector2_1 = new Vector2(0f, -60f); //movement code
            Vector2 vector2_2 = player.MountedCenter + vector2_1;
            float num1 = Vector2.Distance(projectile.Center, vector2_2);
            if (num1 > 1000) //teleport when out of range
                projectile.Center = player.Center + vector2_1;
            Vector2 vector2_3 = vector2_2 - projectile.Center;
            float num2 = 4f;
            if (num1 < num2)
                projectile.velocity *= 0.25f;
            if (vector2_3 != Vector2.Zero)
            {
                if (vector2_3.Length() < num2)
                    projectile.velocity = vector2_3;
                else
                    projectile.velocity = vector2_3 * 0.1f;
            }

            const float rotationModifier = 0.08f;
            const float chargeTime = 180f;
            if (projectile.localAI[1] > 0)
            {
                projectile.localAI[1]--;
                if (projectile.owner == Main.myPlayer)
                    projectile.netUpdate = true;
            }

            if (projectile.owner == Main.myPlayer)
            {
                projectile.ai[0] = Main.MouseWorld.X;
                projectile.ai[1] = Main.MouseWorld.Y;
            }
            projectile.rotation = projectile.rotation.AngleLerp(
                (new Vector2(projectile.ai[0], projectile.ai[1]) - projectile.Center).ToRotation(), rotationModifier);

            projectile.frame = 1;
            
            if (projectile.localAI[0] < 0) //attacking
            {
                projectile.localAI[0]++;
                projectile.frame = 4;

                if (projectile.localAI[0] % 5 == 0)
                {
                    Main.PlaySound(SoundID.NPCDeath52, projectile.Center);
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projectile.Center, 12f * projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(4)),
                            ModContent.ProjectileType<ShadowflamesFriendly>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
            else if (player.controlUseItem || (clickTimer > 0 && projectile.localAI[0] <= chargeTime * 2f))
            {
                clickTimer--;

                projectile.localAI[0]++;
                
                if (projectile.localAI[0] == chargeTime * 2f)
                {
                    if (projectile.owner == Main.myPlayer)
                        projectile.netUpdate = true;
                    const int num226 = 64; //dusts indicate charged up
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX.RotatedBy(projectile.rotation) * 6f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                        Vector2 vector7 = vector6 - projectile.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }
                }

                if (projectile.localAI[0] > chargeTime * 2f)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].scale += 0.5f;
                    d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 27, projectile.velocity.X * 0.4f, projectile.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 1.5f;
                }
            }
            else if (projectile.owner == Main.myPlayer)
            {
                if (projectile.localAI[0] > chargeTime * 2f)
                {
                    projectile.localAI[0] = -120;
                    projectile.netUpdate = true;
                }
                else
                {
                    projectile.localAI[0] = 0;
                }
            }

            if (player.controlUseItem || player.itemTime > 0 || player.itemAnimation > 0 || player.reuseDelay > 0)
                clickTimer = 20;

            projectile.spriteDirection = System.Math.Abs(MathHelper.WrapAngle(projectile.rotation)) > MathHelper.PiOver2 ? -1 : 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = projectile.GetAlpha(lightColor);
            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = projectile.rotation;
            if (projectile.spriteDirection < 0)
                rotation += MathHelper.Pi;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}