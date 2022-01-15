using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantDestroyerHead : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Resprites/NPC_134";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Destroyer");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.netImportant = true;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                projectile.GetAlpha(Color.White), projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void AI()
        {
            //keep the head looking right
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
            
            const int homingDelay = 60;
            float desiredFlySpeedInPixelsPerFrame = 10 * projectile.ai[1];
            float amountOfFramesToLerpBy = 25 / projectile.ai[1]; // minimum of 1, please keep in full numbers even though it's a float!

            if (++projectile.localAI[1] > homingDelay)
            {
                int foundTarget = (int)projectile.ai[0];
                Player p = Main.player[foundTarget];
                if (projectile.Distance(p.Center) > 700)
                {
                    desiredFlySpeedInPixelsPerFrame *= 2;
                    amountOfFramesToLerpBy /= 2;
                }
                Vector2 desiredVelocity = projectile.DirectionTo(p.Center) * desiredFlySpeedInPixelsPerFrame;
                projectile.velocity = Vector2.Lerp(projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
            }

            const float IdleAccel = 0.05f;
            foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == projectile.type && p.whoAmI != projectile.whoAmI && p.Distance(projectile.Center) < projectile.width))
            {
                projectile.velocity.X += IdleAccel * (projectile.position.X < p.position.X ? -1 : 1);
                projectile.velocity.Y += IdleAccel * (projectile.position.Y < p.position.Y ? -1 : 1);
                p.velocity.X += IdleAccel * (p.position.X < projectile.position.X ? -1 : 1);
                p.velocity.Y += IdleAccel * (p.position.Y < projectile.position.Y ? -1 : 1);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("LightningRod"), Main.rand.Next(300, 1200));
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 62, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y), projectile.width, projectile.height, 60, -projectile.velocity.X * 0.2f,
                    -projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
            //int g = Gore.NewGore(projectile.Center, projectile.velocity / 2, mod.GetGoreSlot("Gores/DestroyerGun/DestroyerHead"), projectile.scale);
           // Main.gore[g].timeLeft = 20;
            Main.PlaySound(SoundID.NPCKilled, projectile.Center, 14);
        }
    }
}