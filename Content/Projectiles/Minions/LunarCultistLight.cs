using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class LunarCultistLight : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_522";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ancient Light");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 2;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                Projectile.ai[0] = -1f;
                for (int index1 = 0; index1 < 13; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 90, new Color(), 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].fadeIn = 1f;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 4f;
                    Main.dust[index2].noLight = true;
                }
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                if (Main.rand.Next(10 - (int)Math.Min(7f, Projectile.velocity.Length())) < 1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 90, new Color(), 2.5f);
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 0.2f;
                    Main.dust[index2].fadeIn = 0.4f;
                    if (Main.rand.NextBool(6))
                    {
                        Dust dust2 = Main.dust[index2];
                        dust2.velocity *= 5f;
                        Main.dust[index2].noLight = true;
                    }
                    else
                        Main.dust[index2].velocity = Projectile.DirectionFrom(Main.dust[index2].position) * Main.dust[index2].velocity.Length();
                }
            }
            if (Projectile.timeLeft < 180)
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1], new Vector2());
            if (Projectile.timeLeft < 120)
                Projectile.velocity = Projectile.velocity * 0.98f;
            if (Projectile.velocity.Length() < 0.2f)
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (Projectile.ai[0] > -1 && Projectile.ai[0] < 200) //has target
            {
                Vector2 speed = Main.npc[(int)Projectile.ai[0]].Center - Projectile.Center;
                speed.Normalize();
                speed *= 9f;
                Projectile.velocity.X += speed.X / 100f;
                if (Projectile.velocity.X > 9f)
                    Projectile.velocity.X = 9f;
                else if (Projectile.velocity.X < -9f)
                    Projectile.velocity.X = -9f;
                Projectile.velocity.Y += speed.Y / 100f;
                if (Projectile.velocity.Y > 9f)
                    Projectile.velocity.Y = 9f;
                else if (Projectile.velocity.Y < -9f)
                    Projectile.velocity.Y = -9f;
            }
            else
            {
                if (Projectile.localAI[1]++ > 15)
                {
                    Projectile.localAI[1] = 0;
                    float maxDistance = 2000f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())// && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float npcDistance = Projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                possibleTarget = i;
                            }
                        }
                    }
                    if (possibleTarget > -1)
                    {
                        Projectile.ai[0] = possibleTarget;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 13; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AmberBolt, 0f, 0f, 90, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].fadeIn = 1f;
                Dust dust = Main.dust[index2];
                dust.velocity *= 4f;
                Main.dust[index2].noLight = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}