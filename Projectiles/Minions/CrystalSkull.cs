using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class CrystalSkull : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_289";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Skull");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            Projectile.scale = 0.5f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        int clickTimer;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().CrystalSkullMinion)
                Projectile.timeLeft = 2;

            if (Projectile.originalDamage == 0)
                Projectile.originalDamage = 30;

            Vector2 vector2_1 = new Vector2(0f, -60f); //movement code
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

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.ai[0] = Main.MouseWorld.X;
                Projectile.ai[1] = Main.MouseWorld.Y;
            }
            Projectile.rotation = Projectile.rotation.AngleLerp(
                (new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center).ToRotation(), rotationModifier);

            Projectile.frame = 1;

            if (Projectile.localAI[0] < 0) //attacking
            {
                Projectile.localAI[0]++;
                Projectile.frame = 4;

                if (Projectile.localAI[0] % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(), Projectile.Center,
                            12f * Projectile.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(4)),
                            ModContent.ProjectileType<ShadowflamesFriendly>(), Projectile.damage, Projectile.knockBack,
                            Projectile.owner);
                    }
                }
            }
            else if (player.controlUseItem || (clickTimer > 0 && Projectile.localAI[0] <= chargeTime * 2f))
            {
                clickTimer--;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] == chargeTime * 2f)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true;
                    const int num226 = 64; //dusts indicate charged up
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
            else if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.localAI[0] > chargeTime * 2f)
                {
                    Projectile.localAI[0] = -120;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.localAI[0] = 0;
                }
            }

            if (player.controlUseItem || player.itemTime > 0 || player.itemAnimation > 0 || player.reuseDelay > 0)
                clickTimer = 20;

            Projectile.spriteDirection = System.Math.Abs(MathHelper.WrapAngle(Projectile.rotation)) > MathHelper.PiOver2 ? -1 : 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 100) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
                rotation += MathHelper.Pi;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}