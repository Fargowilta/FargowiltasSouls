using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class EarthPalladOrb : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Souls/PalladOrb";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Palladium Life Orb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.timeLeft = 1200;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 2f;
            projectile.extraUpdates = 3;

            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                Main.PlaySound(SoundID.Item, projectile.Center, 14);
            }

            if (projectile.timeLeft % (projectile.extraUpdates + 1) == 0 && ++projectile.localAI[1] > 30)
            {
                if (projectile.localAI[1] < 90) //accelerate
                {
                    projectile.velocity *= 1.035f;
                }

                if (projectile.localAI[1] > 60 && projectile.localAI[1] < 150
                    && FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.EarthChampion>())
                    && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
                {
                    float rotation = projectile.velocity.ToRotation();
                    Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center
                        + Main.player[Main.npc[EModeGlobalNPC.championBoss].target].velocity * 10f - projectile.Center;
                    float targetAngle = vel.ToRotation();
                    projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, 0.03f));
                }
            }

            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].velocity *= 3;
            int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 1f);
            Main.dust[index3].velocity *= 2f;
            Main.dust[index3].noGravity = true;

            projectile.rotation += 0.4f;

            if (++projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                projectile.timeLeft = 0;
                projectile.position = projectile.Center;
                projectile.width = 500;
                projectile.height = 500;
                projectile.Center = projectile.position;
                projectile.penetrate = -1;
                projectile.Damage();
            }

            //if (!Main.dedServ && Main.LocalPlayer.active) Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;
            
            Main.PlaySound(SoundID.Item, projectile.Center, 14);

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 21f * projectile.scale;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.NextBool() ? 174 : 259, 0f, 0f, 100, new Color(), 2.5f);
                Main.dust[index3].velocity *= 12f;
                Main.dust[index3].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Purified>(), 300);
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Lethargic>(), 300);
            }

            if (projectile.timeLeft > 0)
                projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}