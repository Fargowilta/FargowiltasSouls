using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EaterHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eater Head");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 50;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.netImportant = true;
            ProjectileID.Sets.MinionTargettingFeature[base.projectile.type] = true;

            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 20;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                color25, projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            if ((int) Main.time % 120 == 0) projectile.netUpdate = true;
            if (!player.active)
            {
                projectile.active = false;
                return;
            }

            int num1038 = 10;
            if (player.dead) modPlayer.EaterMinion = false;
            if (modPlayer.EaterMinion) projectile.timeLeft = 2;
            num1038 = 30;

            Vector2 center = player.Center;
            float num1040 = 300f;
            float num1041 = 400f;
            int num1042 = -1;
            if (projectile.Distance(center) > 2000f)
            {
                projectile.Center = center;
                projectile.netUpdate = true;
            }

            bool flag66 = true;
            if (flag66)
            {
                NPC ownerMinionAttackTargetNPC5 = projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC5 != null && ownerMinionAttackTargetNPC5.CanBeChasedBy(projectile, false))
                {
                    float num1043 = projectile.Distance(ownerMinionAttackTargetNPC5.Center);
                    if (num1043 < num1040 * 2f)
                    {
                        num1042 = ownerMinionAttackTargetNPC5.whoAmI;
                        if (ownerMinionAttackTargetNPC5.boss)
                        {
                            int arg_2D352_0 = ownerMinionAttackTargetNPC5.whoAmI;
                        }
                        else
                        {
                            int arg_2D35E_0 = ownerMinionAttackTargetNPC5.whoAmI;
                        }
                    }
                }

                if (num1042 < 0)
                    for (int num1044 = 0; num1044 < 200; num1044++)
                    {
                        NPC nPC13 = Main.npc[num1044];
                        if (nPC13.CanBeChasedBy(projectile, false) && player.Distance(nPC13.Center) < num1041)
                        {
                            float num1045 = projectile.Distance(nPC13.Center);
                            if (num1045 < num1040)
                            {
                                num1042 = num1044;
                                bool arg_2D3CE_0 = nPC13.boss;
                            }
                        }
                    }
            }

            if (num1042 != -1)
            {
                NPC nPC14 = Main.npc[num1042];
                Vector2 vector132 = nPC14.Center - projectile.Center;
                (vector132.X > 0f).ToDirectionInt();
                (vector132.Y > 0f).ToDirectionInt();
                float scaleFactor15 = 0.4f;
                if (vector132.Length() < 600f) scaleFactor15 = 0.6f;
                if (vector132.Length() < 300f) scaleFactor15 = 0.8f;
                if (vector132.Length() > nPC14.Size.Length() * 0.75f)
                {
                    projectile.velocity += Vector2.Normalize(vector132) * scaleFactor15 * 1.5f;
                    if (Vector2.Dot(projectile.velocity, vector132) < 0.25f) projectile.velocity *= 0.8f;
                }

                float num1046 = 30f;
                if (projectile.velocity.Length() > num1046) projectile.velocity = Vector2.Normalize(projectile.velocity) * num1046;
            }
            else
            {
                float num1047 = 0.2f;
                Vector2 vector133 = center - projectile.Center;
                if (vector133.Length() < 200f) num1047 = 0.12f;
                if (vector133.Length() < 140f) num1047 = 0.06f;
                if (vector133.Length() > 100f)
                {
                    if (Math.Abs(center.X - projectile.Center.X) > 20f) projectile.velocity.X = projectile.velocity.X + num1047 * Math.Sign(center.X - projectile.Center.X);
                    if (Math.Abs(center.Y - projectile.Center.Y) > 10f) projectile.velocity.Y = projectile.velocity.Y + num1047 * Math.Sign(center.Y - projectile.Center.Y);
                }
                else if (projectile.velocity.Length() > 2f)
                {
                    projectile.velocity *= 0.96f;
                }

                if (Math.Abs(projectile.velocity.Y) < 1f) projectile.velocity.Y = projectile.velocity.Y - 0.1f;
                float num1048 = 15f;
                if (projectile.velocity.Length() > num1048) projectile.velocity = Vector2.Normalize(projectile.velocity) * num1048;
            }

            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            int direction = projectile.direction;
            projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
            if (direction != projectile.direction) projectile.netUpdate = true;
            float num1049 = MathHelper.Clamp(projectile.localAI[0], 0f, 50f);
            projectile.position = projectile.Center;
            projectile.scale = 1f + num1049 * 0.01f;
            projectile.width = projectile.height = (int) (num1038 * projectile.scale);
            projectile.Center = projectile.position;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 42;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }

            projectile.position -= projectile.velocity / 2;
        }
    }
}