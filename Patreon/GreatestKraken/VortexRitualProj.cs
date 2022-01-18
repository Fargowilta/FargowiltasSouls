using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.IO;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexRitualProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_465";

        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Vortex Ritual");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        const int baseDimension = 70;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = baseDimension;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.scale = 0.5f;

            projectile.GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().DeletionImmuneRank = 2;
            projectile.GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.width);
            writer.Write(projectile.height);
            writer.Write(projectile.scale);
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.width = reader.ReadInt32();
            projectile.height = reader.ReadInt32();
            projectile.scale = reader.ReadSingle();

            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (projectile.owner != Main.myPlayer)
                mousePos = buffer;
        }

        public override void AI()
        {
            projectile.timeLeft = 2;

            //kill me if player is not holding
            Player player = Main.player[projectile.owner];

            if (player.dead || !player.active || !(player.HeldItem.type == ModContent.ItemType<VortexMagnetRitual>() && player.channel && player.CheckMana(player.HeldItem.mana)))
            {
                projectile.Kill();
                return;
            }

            projectile.damage = player.GetWeaponDamage(player.HeldItem);
            projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);

            //drain mana 3 times per second
            /*if (++projectile.ai[0] >= 20)
            {
                if (player.CheckMana(10))
                {
                    player.statMana -= player.HeldItem.mana;
                    player.manaRegenDelay = 300;
                    projectile.ai[0] = 0;
                }
                else
                {
                    projectile.Kill();
                }
            }*/

            projectile.alpha -= 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            if (projectile.owner == Main.myPlayer)
            {
                if (--syncTimer < 0)
                {
                    syncTimer = 20;
                    projectile.netUpdate = true;
                }

                mousePos = Main.MouseWorld;
            }

            //if (projectile.Distance(mousePos) < Math.Sqrt(2 * projectile.width * projectile.width) / 2)
            if (projectile.scale < 5f) //grow
                projectile.scale *= 1.007f;
            else
                projectile.scale = 5f;

            projectile.position = projectile.Center;
            projectile.width = (int)(baseDimension * projectile.scale);
            projectile.height = (int)(baseDimension * projectile.scale);
            projectile.Center = projectile.position;

            float maxDistance = projectile.width * 2f;
            if (++projectile.localAI[0] > 6)
            {
                projectile.localAI[0] = 0;
                if (projectile.owner == Main.myPlayer)
                {
                    int maxShots = 8;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile) && projectile.Distance(npc.Center) < maxDistance)
                        {
                            Vector2 spawnPos = projectile.Center + Main.rand.NextVector2Circular(projectile.width / 4, projectile.height / 4);
                            if (Collision.CanHitLine(spawnPos, 0, 0, npc.Center, 0, 0))
                            {
                                if (--maxShots < 0)
                                    break;

                                Vector2 baseVel = Vector2.Normalize(npc.Center - spawnPos);
                                Projectile.NewProjectile(spawnPos, 6f * baseVel, ProjectileID.MagnetSphereBolt,
                                    projectile.damage / 2, projectile.knockBack, projectile.owner);
                                int p = Projectile.NewProjectile(spawnPos, 21f * baseVel, ModContent.ProjectileType<VortexBolt>(),
                                    projectile.damage, projectile.knockBack, projectile.owner, baseVel.ToRotation(), Main.rand.Next(80));
                                if (p != Main.maxProjectiles)
                                {
                                    Main.projectile[p].ranged = false;
                                    Main.projectile[p].magic = true;
                                }
                            }
                        }
                    }
                }
            }

            int dustMax = 5 + 5 * (int)projectile.scale;
            for (int i = 0; i < dustMax; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * maxDistance);
                offset.Y += (float)(Math.Cos(angle) * maxDistance);
                Dust dust = Main.dust[Dust.NewDust(
                    projectile.Center + offset, 0, 0,
                    226, 0, 0, 100, Color.White, projectile.scale / 5f
                    )];
                dust.velocity = projectile.velocity;
                if (Main.rand.NextBool(3))
                {
                    dust.velocity += Vector2.Normalize(offset) * -Main.rand.NextFloat(5f);
                    dust.position += dust.velocity * 10f;
                }
                dust.noGravity = true;
            }

            /*projectile.velocity = (mousePos - projectile.Center) / 20;
            const float speed = 4f;
            if (projectile.velocity.Length() < speed)
            {
                projectile.velocity.SafeNormalize(Vector2.UnitX);
                projectile.velocity *= speed;
            }
            if (projectile.Distance(mousePos) <= speed)
            {
                projectile.Center = mousePos;
                projectile.velocity = Vector2.Zero;
            }*/
            //projectile.velocity = Vector2.Zero;

            const float speed = 2f;
            if (projectile.Distance(mousePos) <= speed)
            {
                projectile.Center = mousePos;
                projectile.velocity = Vector2.Zero;
            }
            else
            {
                projectile.velocity = projectile.DirectionTo(mousePos);
            }

            Lighting.AddLight(projectile.Center, 0.4f, 0.85f, 0.9f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                    projectile.frame = 0;
            }

            projectile.rotation -= MathHelper.TwoPi / 300;
            projectile.localAI[1] += MathHelper.TwoPi / 300 + MathHelper.TwoPi / 120;
            if (projectile.rotation < MathHelper.TwoPi)
                projectile.rotation += MathHelper.TwoPi;
            if (projectile.localAI[1] > MathHelper.TwoPi)
                projectile.localAI[1] -= MathHelper.TwoPi;
        }

        public override void Kill(int timeLeft)
        {
            MakeDust();
        }

        private void MakeDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            int max = 30 * (int)projectile.scale;
            for (int i = 0; i < max; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 10f * projectile.scale;
                vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * max) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 92, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }

            for (int a = 0; a < (int)projectile.scale; a++)
            {
                for (int index1 = 0; index1 < 3; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0.0f, 0.0f, 0, new Color(), 2.5f);
                    Main.dust[index2].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 1f;
                    int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index3].position = new Vector2((float)(projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + projectile.Center;
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity = dust2.velocity * 1f;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 21f * projectile.scale;
                    Main.dust[index2].noLight = true;
                    int index3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index3].velocity *= 12f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].noLight = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                    //Main.dust[d].position = Main.player[projectile.owner].Center;
                }
            }
        }

        private int GetDamage(int damage)
        {
            return (int)(damage * projectile.scale / 5f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = GetDamage(damage);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            damage = GetDamage(damage);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            damage = GetDamage(damage);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            Texture2D texture2D14 = mod.GetTexture("Patreon/GreatestKraken/VortexRitualRing");
            Rectangle ringRect = texture2D14.Bounds;
            Vector2 origin = ringRect.Size() / 2f;
            float scale = projectile.scale / 360f * 96f;
            float rotation = projectile.rotation + projectile.localAI[1];
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(ringRect), projectile.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
