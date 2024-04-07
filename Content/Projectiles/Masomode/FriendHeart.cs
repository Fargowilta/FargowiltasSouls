using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FriendHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Friend Heart");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 900;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ai[0] = -1;
            }

            NPC target = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
            if (target != null)
            {
                if (target.CanBeChasedBy())
                {
                    if (Projectile.Distance(target.Center) > 40f)
                        Projectile.velocity = (Projectile.velocity * 16f + 17f * Projectile.SafeDirectionTo(target.Center)) / 17f;
                    else if (Projectile.velocity.Length() < 17)
                        Projectile.velocity *= 1.05f;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (--Projectile.ai[1] < 0f)
                {
                    Projectile.ai[1] = 6f;
                    float maxDistance = 1700f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())
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

                Projectile.localAI[1] = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Lovestruck, 600);

            if (Projectile.owner == Main.myPlayer)
            {
                int healAmount = 2;
                Main.player[Main.myPlayer].HealEffect(healAmount);
                Main.player[Main.myPlayer].statLife += healAmount;

                if (Main.player[Main.myPlayer].statLife > Main.player[Main.myPlayer].statLifeMax2)
                    Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; // ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; // ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}