using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Timber
{
    public class TimberPalmTree : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Minions/PalmTreeSentry";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Palm Tree");
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<TimberChampion>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];

            Projectile.timeLeft = 2;

            if (Projectile.ai[1] == 0)
            {
                Projectile.tileCollide = true;

                if (Projectile.Distance(player.Center) > 1200)
                {
                    Projectile.ai[1] = 1;
                    Projectile.netUpdate = true;
                    return;
                }

                if (Projectile.velocity.Y == 0 && --Projectile.localAI[1] < 0)
                {
                    Projectile.localAI[1] = 120f;
                    if (FargoSoulsUtil.HostCheck)
                    {
                        const float gravity = 0.2f;
                        float time = 90f;
                        Vector2 distance = player.Center - Projectile.Center;
                        distance.X /= time;
                        distance.Y = distance.Y / time - 0.5f * gravity * time;
                        for (int i = 0; i < 10; i++)
                        {
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, distance + Main.rand.NextVector2Square(-0.5f, 0.5f) * 2,
                                ModContent.ProjectileType<TimberAcorn>(), Projectile.damage, 0f, Main.myPlayer);
                        }
                    }
                }

                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
                }
            }
            else //chase to get back in range
            {
                Projectile.tileCollide = false;
                Projectile.localAI[1] = 0;

                if (Projectile.Distance(player.Center) < 500)
                {
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                    return;
                }

                Vector2 targetPos = player.Center;
                const float speedModifier = 0.35f;
                const float cap = 20f;

                if (Projectile.Center.X < targetPos.X)
                {
                    Projectile.velocity.X += speedModifier;
                    if (Projectile.velocity.X < 0)
                        Projectile.velocity.X += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.X -= speedModifier;
                    if (Projectile.velocity.X > 0)
                        Projectile.velocity.X -= speedModifier * 2;
                }
                if (Projectile.Center.Y < targetPos.Y)
                {
                    Projectile.velocity.Y += speedModifier;
                    if (Projectile.velocity.Y < 0)
                        Projectile.velocity.Y += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.Y -= speedModifier;
                    if (Projectile.velocity.Y > 0)
                        Projectile.velocity.Y -= speedModifier * 2;
                }
                if (Math.Abs(Projectile.velocity.X) > cap)
                    Projectile.velocity.X = cap * Math.Sign(Projectile.velocity.X);
                if (Math.Abs(Projectile.velocity.Y) > cap)
                    Projectile.velocity.Y = cap * Math.Sign(Projectile.velocity.Y);
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
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
