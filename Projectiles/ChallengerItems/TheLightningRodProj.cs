using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.ChallengerItems
{
    public class TheLightningRodProj : ModProjectile
    {
        public int damage;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Lightning Rod");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 132;
            Projectile.height = 132;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 0;
            Projectile.timeLeft = 45;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        private const int maxTime = 80;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && !player.controlUseItem && Projectile.ai[0] == 0f)
            {
                Projectile.Kill();
                return;
            }

            if (player.dead || !player.active || player.ghost)
            {
                Projectile.Kill();
                return;
            }

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] % 20 == 0)
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (Projectile.ai[0] == 0 && player.velocity.X != 0)
                player.ChangeDir(Math.Sign(player.velocity.X));
            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            //player.itemAnimationMax = 15;

            const float maxRotation = MathHelper.Pi / 6.85f / 1.5f; //spin up to full speed
            float modifier = maxRotation * (Projectile.ai[0] == 0 ? Math.Min(1f, Projectile.localAI[0] / 80f) : 1f);

            if (Projectile.ai[0] == 0f) //while held
            {
                Projectile.timeLeft = maxTime + 2;

                damage = Projectile.damage;
                Projectile.numHits = 0;

                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
                Projectile.rotation += modifier * player.direction;
                Projectile.velocity = Projectile.rotation.ToRotationVector2();
                Projectile.position -= Projectile.velocity;
                player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);

                if (Projectile.localAI[0] == 40)
                {
                    Projectile.ai[1] = 1f;
                }
                else if (Projectile.localAI[0] == 120) //time to throw
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile.ai[0] = 1f;
                        Projectile.localAI[0] = 0f;
                        Projectile.localAI[1] = Projectile.Distance(Main.MouseWorld);
                        if (Projectile.localAI[1] < 200) //minimum throwing distance
                            Projectile.localAI[1] = 200;
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld);
                        Projectile.netUpdate = true;
                    }
                }
            }
            else //while thrown
            {
                Projectile.damage = (int)(damage * Math.Pow(0.933, Projectile.numHits));
                if (Projectile.damage < damage / 2)
                    Projectile.damage = damage / 2;

                if (Projectile.localAI[0] > maxTime)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.Kill();
                    return;
                }
                else //player faces where this was thrown
                {
                    Projectile.direction = Math.Sign(Projectile.Center.X - player.Center.X);
                    player.ChangeDir(Projectile.direction);
                    player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
                }

                //while going out
                if (Projectile.localAI[0] < maxTime / 2)
                {
                    //rain lightning
                    if (Projectile.localAI[0] % 6 == 0 && Projectile.owner == Main.myPlayer)
                    {
                        Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 2);
                        spawnPos -= Main.rand.NextFloat(900f, 1800f) * Vector2.UnitY;
                        float ai1 = Projectile.Center.Y + Main.rand.NextFloat(-Projectile.height / 4, Projectile.height / 4);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, 12f * Vector2.UnitY, ModContent.ProjectileType<TheLightning>(),
                            Projectile.damage, Projectile.knockBack / 2, Projectile.owner, Vector2.UnitY.ToRotation(), ai1);
                    }

                    //if hits a solid surface, immediately rebound
                    if (Collision.SolidTiles(Projectile.Center, 2, 2, false) && Projectile.owner == Main.myPlayer)
                    {
                        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                        Projectile.localAI[0] = maxTime - Projectile.localAI[0];
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.rotation += modifier * player.direction * 1.25f; //spin faster when thrown

                float distanceModifier = (float)Math.Sin(Math.PI / maxTime * Projectile.localAI[0]); //fly out and back
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distanceModifier * Projectile.localAI[1];
            }

            if (Projectile.ai[1] != 0f)
            {
                //dust!
                int dustId = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 156, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 156, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustId3].noGravity = true;
            }

            Projectile.Center = ownerMountedCenter;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[0] == 0f) //less damage when held
                damage /= 2;

            hitDirection = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] != 0f && Collision.CanHitLine(Main.player[Projectile.owner].Center, 0, 0, Projectile.Center, 0, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) <= Projectile.width / 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            if (Projectile.ai[1] != 0f)
            {
                Color color26 = lightColor;
                color26 = Projectile.GetAlpha(color26);

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26 * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}