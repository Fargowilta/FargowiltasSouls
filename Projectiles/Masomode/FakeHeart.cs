using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FakeHeart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Fake Heart");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            cooldownSlot = 0;
        }

        public override void AI()
        {
            float gravity = .1f;
            float yMax = 7f;

            projectile.velocity.Y += gravity;
            if (projectile.velocity.Y > yMax)
                projectile.velocity.Y = yMax;
            projectile.velocity.X *= .95f;
            if (projectile.velocity.X < .1f && projectile.velocity.X > -.1f)
                projectile.velocity.X = 0f;

            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (projectile.Colliding(projectile.Hitbox, target.Hitbox))
            {
                if (target.GetModPlayer<FargoPlayer>().DevianttHearts)
                {
                    target.statLife += 1;
                    target.HealEffect(1);
                }
                else
                {
                    target.hurtCooldowns[0] = 0;
                    int defense = target.statDefense;
                    float endurance = target.endurance;
                    target.statDefense = 0;
                    target.endurance = 0;
                    target.Hurt(PlayerDeathReason.ByCustomReason(target.name + " felt heartbroken."), projectile.damage, 0, false, false, false, 0);
                    target.statDefense = defense;
                    target.endurance = endurance;

                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, mod.NPCType("DeviBoss")))
                        target.AddBuff(mod.BuffType("Lovestruck"), 240);
                }
  
                projectile.timeLeft = 0;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}