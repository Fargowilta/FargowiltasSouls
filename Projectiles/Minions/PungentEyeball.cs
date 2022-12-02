using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class PungentEyeball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pungent Eyeball");
            Main.projFrames[Projectile.type] = 6;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 32;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().PungentEyeballMinion)
                Projectile.timeLeft = 2;

            //if (Projectile.damage == 0) Projectile.damage = (int)(50f * player.GetDamage(DamageClass.Summon).Additive);

            Vector2 vector2_1 = new Vector2(0f, -85f); //movement code
            Vector2 vector2_2 = player.MountedCenter + vector2_1;
            float num1 = Vector2.Distance(Projectile.Center, vector2_2);
            if (num1 > 1000) //teleport when out of range
                Projectile.Center = player.Center + vector2_1;
            Vector2 vector2_3 = vector2_2 - Projectile.Center;
            float num2 = 4f;
            if (num1 < num2)
                Projectile.velocity *= 0.25f;
            if (vector2_3 != Vector2.Zero)
            {
                if (vector2_3.Length() < num2)
                    Projectile.velocity = vector2_3;
                else
                    Projectile.velocity = vector2_3 * 0.1f;
            }

            const float rotationModifier = 0.08f;
            const float chargeTime = 180f;
            if (Projectile.localAI[1] > 0)
            {
                Projectile.localAI[1]--;
                if (Projectile.owner == Main.myPlayer)
                    Projectile.netUpdate = true;
            }
            if (player.controlUseItem)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalDeathrayPungent>()] < 1)
                {
                    Projectile.localAI[0]++;
                    if (player.GetModPlayer<FargoSoulsPlayer>().MasochistSoul)
                        Projectile.localAI[0] += 2;
                }
                if (Projectile.localAI[0] == chargeTime)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                    const int num226 = 36; //dusts indicate charged up
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.UnitX.RotatedBy(Projectile.rotation) * 6f;
                        vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Projectile.Center;
                        Vector2 vector7 = vector6 - Projectile.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 27, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].velocity = vector7;
                    }
                }
                if (Projectile.localAI[0] > chargeTime)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                }
                if (Projectile.localAI[0] == chargeTime * 2f)
                {
                    if (Projectile.owner == Main.myPlayer) //fully charged
                    {
                        Projectile.netUpdate = true;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -1);
                    }
                }
                if (Projectile.localAI[0] > chargeTime * 2f)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 4f;
                    Main.dust[d].scale += 0.5f;
                    d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 27, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 1.5f;
                }
            }
            else
            {
                if (Projectile.localAI[0] > chargeTime)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                    Projectile.localAI[1] = 120f;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.rotation), ModContent.ProjectileType<PhantasmalDeathrayPungent>(),
                            Projectile.damage, 4f, Projectile.owner, Projectile.identity, (Projectile.localAI[0] >= chargeTime * 2f) ? 1f : 0f);
                }
                Projectile.localAI[0] = 0;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.ai[0] = Main.MouseWorld.X;
                Projectile.ai[1] = Main.MouseWorld.Y;
            }
            Projectile.rotation = Projectile.rotation.AngleLerp(
                (new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center).ToRotation(), rotationModifier);

            if (Projectile.frameCounter++ > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}