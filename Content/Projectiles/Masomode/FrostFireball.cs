using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FrostFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_253";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frostfireball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;

            if (ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                fargo.Call("LowRenderProj", Projectile);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.3f;
                Main.dust[index2].velocity.Y *= 0.3f;
            }
            /*index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].velocity.X *= 0.3f;
            Main.dust[index2].velocity.Y *= 0.3f;*/

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                int ai0 = (int)Projectile.ai[0];
                if (Main.npc[ai0].CanBeChasedBy(ai0))
                {
                    Vector2 dist = Main.npc[ai0].Center - Projectile.Center;
                    dist.Normalize();
                    dist *= 8f;
                    Projectile.velocity.X = (Projectile.velocity.X * 14 + dist.X) / 15;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 14 + dist.Y) / 15;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (--Projectile.localAI[0] < 0f)
                {
                    Projectile.localAI[0] = 10f;
                    float maxDistance = 1000f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile))// && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                        {
                            float npcDistance = Projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                possibleTarget = i;
                            }
                        }
                    }
                    Projectile.ai[0] = possibleTarget;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation += 0.3f * Projectile.direction;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int index1 = 0; index1 < 10; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;
                /*index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
                index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 2f;*/
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 240);
            //target.AddBuff(BuffID.ShadowFlame, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25) * Projectile.Opacity;
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