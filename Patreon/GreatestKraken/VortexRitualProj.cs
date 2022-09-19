using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Patreon.GreatestKraken
{
    public class VortexRitualProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_465";

        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Vortex Ritual");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        const int baseDimension = 70;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = baseDimension;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 0.5f;

            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
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

            return Math.Sqrt(dX * dX + dY * dY) <= Projectile.width / 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Projectile.width);
            writer.Write7BitEncodedInt(Projectile.height);
            writer.Write(Projectile.scale);
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.width = reader.Read7BitEncodedInt();
            Projectile.height = reader.Read7BitEncodedInt();
            Projectile.scale = reader.ReadSingle();

            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
                mousePos = buffer;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            //kill me if player is not holding
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active || !(player.HeldItem.type == ModContent.ItemType<VortexMagnetRitual>() && player.channel && player.CheckMana(player.HeldItem.mana)))
            {
                Projectile.Kill();
                return;
            }

            Projectile.damage = player.GetWeaponDamage(player.HeldItem);
            Projectile.knockBack = player.GetWeaponKnockback(player.HeldItem, player.HeldItem.knockBack);

            //drain mana 3 times per second
            /*if (++Projectile.ai[0] >= 20)
            {
                if (player.CheckMana(10))
                {
                    player.statMana -= player.HeldItem.mana;
                    player.manaRegenDelay = 300;
                    Projectile.ai[0] = 0;
                }
                else
                {
                    Projectile.Kill();
                }
            }*/

            Projectile.alpha -= 10;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.owner == Main.myPlayer)
            {
                if (--syncTimer < 0)
                {
                    syncTimer = 20;
                    Projectile.netUpdate = true;
                }

                mousePos = Main.MouseWorld;
            }

            //if (Projectile.Distance(mousePos) < Math.Sqrt(2 * Projectile.width * Projectile.width) / 2)
            if (Projectile.scale < 5f) //grow
                Projectile.scale *= 1.007f;
            else
                Projectile.scale = 5f;

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(baseDimension * Projectile.scale);
            Projectile.height = (int)(baseDimension * Projectile.scale);
            Projectile.Center = Projectile.position;

            float maxDistance = Projectile.width * 2f;
            if (++Projectile.localAI[0] > 6)
            {
                Projectile.localAI[0] = 0;
                if (Projectile.owner == Main.myPlayer)
                {
                    int maxShots = 8;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy() && Projectile.Distance(npc.Center) < maxDistance)
                        {
                            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 4);
                            if (Collision.CanHitLine(spawnPos, 0, 0, npc.Center, 0, 0))
                            {
                                if (--maxShots < 0)
                                    break;

                                Vector2 baseVel = Vector2.Normalize(npc.Center - spawnPos);
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, 6f * baseVel, ProjectileID.MagnetSphereBolt,
                                    Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, 21f * baseVel, ModContent.ProjectileType<VortexBolt>(),
                                    Projectile.damage, Projectile.knockBack, Projectile.owner, baseVel.ToRotation(), Main.rand.Next(80));
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].DamageType = DamageClass.Magic;
                            }
                        }
                    }
                }
            }

            int dustMax = 5 + 5 * (int)Projectile.scale;
            for (int i = 0; i < dustMax; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * maxDistance);
                offset.Y += (float)(Math.Cos(angle) * maxDistance);
                Dust dust = Main.dust[Dust.NewDust(
                    Projectile.Center + offset, 0, 0,
                    226, 0, 0, 100, Color.White, Projectile.scale / 5f
                    )];
                dust.velocity = Projectile.velocity;
                if (Main.rand.NextBool(3))
                {
                    dust.velocity += Vector2.Normalize(offset) * -Main.rand.NextFloat(5f);
                    dust.position += dust.velocity * 10f;
                }
                dust.noGravity = true;
            }

            /*Projectile.velocity = (mousePos - Projectile.Center) / 20;
            const float speed = 4f;
            if (Projectile.velocity.Length() < speed)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Projectile.velocity *= speed;
            }
            if (Projectile.Distance(mousePos) <= speed)
            {
                Projectile.Center = mousePos;
                Projectile.velocity = Vector2.Zero;
            }*/
            //Projectile.velocity = Vector2.Zero;

            const float speed = 2f;
            if (Projectile.Distance(mousePos) <= speed)
            {
                Projectile.Center = mousePos;
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                Projectile.velocity = Projectile.DirectionTo(mousePos);
            }

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            Projectile.rotation -= MathHelper.TwoPi / 300;
            Projectile.localAI[1] += MathHelper.TwoPi / 300 + MathHelper.TwoPi / 120;
            if (Projectile.rotation < MathHelper.TwoPi)
                Projectile.rotation += MathHelper.TwoPi;
            if (Projectile.localAI[1] > MathHelper.TwoPi)
                Projectile.localAI[1] -= MathHelper.TwoPi;
        }

        public override void Kill(int timeLeft)
        {
            MakeDust();
        }

        private void MakeDust()
        {
            for (int index1 = 0; index1 < 25; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(), 1.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 7f * Projectile.scale;
                Main.dust[index2].noLight = true;
                int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, new Color(), 1f);
                Main.dust[index3].velocity *= 4f * Projectile.scale;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].noLight = true;
            }

            int max = 30 * (int)Projectile.scale;
            for (int i = 0; i < max; i++) //warning dust ring
            {
                Vector2 vector6 = Vector2.UnitY * 10f * Projectile.scale;
                vector6 = vector6.RotatedBy((i - (80 / 2 - 1)) * max) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, 92, 0f, 0f, 0, default(Color), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }

            for (int a = 0; a < (int)Projectile.scale; a++)
            {
                for (int index1 = 0; index1 < 3; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0.0f, 0.0f, 0, new Color(), 2.5f);
                    Main.dust[index2].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity = dust1.velocity * 1f;
                    int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index3].position = new Vector2((float)(Projectile.width / 2), 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity = dust2.velocity * 1f;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 21f * Projectile.scale;
                    Main.dust[index2].noLight = true;
                    int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index3].velocity *= 12f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].noLight = true;
                }

                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3.5f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(9f, 12f);
                    //Main.dust[d].position = Main.player[Projectile.owner].Center;
                }
            }
        }

        private int GetDamage(int damage)
        {
            return (int)(damage * Projectile.scale / 5f);
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
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Patreon/GreatestKraken/VortexRitualRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle ringRect = texture2D14.Bounds;
            Vector2 origin = ringRect.Size() / 2f;
            float scale = Projectile.scale / 360f * 96f;
            float rotation = Projectile.rotation + Projectile.localAI[1];
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(ringRect), Projectile.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
