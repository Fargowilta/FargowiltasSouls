using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Timber
{
    public class TimberTree : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tree");
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 304;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.velocity.Y += 1f;

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (WorldSavingSystem.EternityMode)
                FargoSoulsUtil.NewNPCEasy(Terraria.Entity.InheritSource(Projectile), Projectile.Top - 20 * Vector2.UnitY, ModContent.NPCType<LesserSquirrel>(), velocity: new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-20, -10)));

            if (FargoSoulsUtil.HostCheck)
            {
                Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (player != null)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 spawnPos = Projectile.position;
                        spawnPos.X += Projectile.width / 2f + Main.rand.NextFloat(-40, 40);
                        spawnPos.Y += 40 + Main.rand.NextFloat(-40, 40);

                        const float gravity = 0.2f;
                        float time = 30f;
                        Vector2 distance = player.Center - spawnPos;
                        distance.X = Main.rand.NextFloat(-1.5f, 1.5f);
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        float minimumY = Main.rand.NextFloat(-12f, -9f);
                        if (distance.Y > minimumY)
                            distance.Y = minimumY;
                        distance += Main.rand.NextVector2Square(-0.5f, 0.5f);

                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), spawnPos, distance, ModContent.ProjectileType<TimberAcorn>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}