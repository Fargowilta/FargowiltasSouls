using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class DestroyerHead2 : ModProjectile
    {
        public override string Texture => "Terraria/NPC_134";

        public float modifier;
        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Destroyer Head");
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 0;
            projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.Write(modifier);

            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            modifier = reader.ReadSingle();

            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles,
           List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D glow = mod.GetTexture("Projectiles/Minions/DestroyerHead2_glow");
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                color25, projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(glow, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                Color.White, projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    projectile.netUpdate = true;
                }
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;

                float minionSlotsUsed = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && !Main.projectile[i].hostile && Main.projectile[i].owner == projectile.owner && Main.projectile[i].minionSlots > 0)
                        minionSlotsUsed += Main.projectile[i].minionSlots;
                }

                float modifier = Main.player[projectile.owner].maxMinions - minionSlotsUsed;
                if (modifier < 0)
                    modifier = 0;
                if (modifier > 5)
                    modifier = 5;

                if (projectile.owner == Main.myPlayer)
                {
                    projectile.netUpdate = true;

                    int current = projectile.whoAmI;
                    for (int i = 0; i <= modifier * 3; i++)
                        current = Projectile.NewProjectile(projectile.Center, projectile.velocity, mod.ProjectileType("DestroyerBody2"), projectile.damage, projectile.knockBack, projectile.owner, Main.projectile[current].identity);
                    int previous = current;
                    current = Projectile.NewProjectile(projectile.Center, projectile.velocity, mod.ProjectileType("DestroyerTail2"), projectile.damage, projectile.knockBack, projectile.owner, Main.projectile[current].identity);
                    Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                    Main.projectile[previous].netUpdate = true;
                }
            }

            //keep the head looking right
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;

            const int aislotHomingCooldown = 0;
            const int homingDelay = 30;
            float desiredFlySpeedInPixelsPerFrame = 40 + modifier * 4;
            float amountOfFramesToLerpBy = 30 + 30 - modifier * 6; // minimum of 1, please keep in full numbers even though it's a float!

            projectile.ai[aislotHomingCooldown]++;
            if (projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                if (projectile.Distance(mousePos) > 50)
                {
                    Vector2 desiredVelocity = projectile.DirectionTo(mousePos) * desiredFlySpeedInPixelsPerFrame;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            target.AddBuff(mod.BuffType("LightningRod"), Main.rand.Next(300, 1200));
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 60, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
            Main.PlaySound(SoundID.NPCKilled, projectile.Center, 14);
        }
    }
}