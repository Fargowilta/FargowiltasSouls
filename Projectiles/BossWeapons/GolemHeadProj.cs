using FargowiltasSouls.Items.Weapons.SwarmDrops;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    internal class GolemHeadProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golem Head");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        int headsStacked;
        const int maxHeadsStacked = 10;

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.scale *= 0.75f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(headsStacked);

        public override void ReceiveExtraAI(BinaryReader reader) => headsStacked = reader.ReadInt32();

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ai[0] = -1; //for homing

                foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.owner == Projectile.owner && p.ai[1] == 0 && p.whoAmI != Projectile.whoAmI))
                {
                    headsStacked++;
                }

                if (headsStacked == maxHeadsStacked - 1)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit41, Projectile.Center);
                    FargoSoulsUtil.DustRing(Projectile.Center, 96, 87, 12f, default, 2f);
                }

                //just fire myself if too many
                if (headsStacked >= maxHeadsStacked && Projectile.owner == Main.myPlayer)
                {
                    headsStacked = 0; //cancel my damage boost

                    Projectile.ai[1] = 1000; //fly immediately, no delay
                    Projectile.localAI[0] = Projectile.DirectionTo(Main.MouseWorld).ToRotation();
                    Projectile.netUpdate = true;
                }
            }

            const int longestHomingDelay = 25;
            const float desiredFlySpeedInPixelsPerFrame = 48;
            const float amountOfFramesToLerpBy = 15; // minimum of 1, please keep in full numbers even though it's a float!

            if (Projectile.ai[1] >= 0) //orbit player
            {
                Projectile.timeLeft++;

                Player player = Main.player[Projectile.owner];

                Projectile.position += (player.position - player.oldPosition) * 0.9f;

                if (Projectile.Center != player.Center)
                {
                    if (Projectile.Distance(player.Center) < 120)
                        Projectile.velocity += Projectile.DirectionFrom(player.Center) * 0.75f;

                    if (Projectile.Distance(player.Center) > 16 * 6)
                    {
                        Vector2 desiredVelocity = desiredFlySpeedInPixelsPerFrame * 0.5f * Projectile.DirectionTo(player.Center);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                    }
                }

                const float IdleAccel = 0.1f;
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.type == Projectile.type && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
                {
                    Projectile.velocity.X += IdleAccel * (Projectile.position.X < p.position.X ? -1 : 1);
                    Projectile.velocity.Y += IdleAccel * (Projectile.position.Y < p.position.Y ? -1 : 1);
                    p.velocity.X += IdleAccel * (p.position.X < Projectile.position.X ? -1 : 1);
                    p.velocity.Y += IdleAccel * (p.position.Y < Projectile.position.Y ? -1 : 1);
                }

                if (Projectile.ai[1] == 0 && Projectile.owner == Main.myPlayer && (player.dead || player.ghost || !player.controlUseItem || player.HeldItem.type != ModContent.ItemType<GolemTome2>()))
                {
                    Projectile.ai[1] = 1;
                    Projectile.localAI[0] = player.DirectionTo(Main.MouseWorld).ToRotation();
                    Projectile.netUpdate = true;
                }

                if (Projectile.ai[1] > 0) //told to fly
                {
                    Projectile.velocity *= 0.97f;

                    //staggered launch
                    if (++Projectile.ai[1] > (player.ownedProjectileCounts[Projectile.type] - headsStacked) * 4)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit41, Projectile.Center);

                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.damage = (int)(Projectile.damage * (1.0 + 2.0 * headsStacked / maxHeadsStacked));

                            Projectile.ai[1] = -1;
                            Projectile.velocity = 24f * player.DirectionTo(Main.MouseWorld);
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                if (!Projectile.tileCollide)
                {
                    Projectile.tileCollide = !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
                    if (Projectile.tileCollide)
                    {
                        Projectile.position = Projectile.Center;
                        Projectile.width = (int)(Projectile.width / 0.75f);
                        Projectile.height = (int)(Projectile.height / 0.75f);
                        Projectile.scale /= 0.75f;
                        Projectile.Center = Projectile.position;
                    }
                }

                if (Projectile.ai[0] == -1) //homing but no target atm
                {
                    if (++Projectile.localAI[1] > longestHomingDelay - headsStacked * 2)
                    {
                        Projectile.localAI[1] = 0;
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 400f + 800f / maxHeadsStacked * headsStacked, true);
                        Projectile.netUpdate = true;
                    }
                }
                else //currently have target
                {
                    Projectile.timeLeft++; //dont despawn partway

                    NPC npc = Main.npc[(int)Projectile.ai[0]];

                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        Vector2 desiredVelocity = Projectile.DirectionTo(npc.Center) * desiredFlySpeedInPixelsPerFrame;
                        float homingModifier = 0.75f + 0.75f * headsStacked / maxHeadsStacked;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy * homingModifier);
                    }
                    else //target lost, reset
                    {
                        Projectile.ai[0] = -1;
                        Projectile.localAI[1] = 0;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (Projectile.ai[1] >= 0)
                Projectile.rotation += 0.2f * Main.player[Projectile.owner].direction;
            else
                Projectile.rotation += 0.3f * Math.Sign(Projectile.velocity.X);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = Projectile.width / 4;
            height = Projectile.height / 4;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                Projectile.timeLeft = 0;
                Projectile.penetrate = -1;
                Projectile.position = Projectile.Center;
                Projectile.width = 300;
                Projectile.height = 300;
                Projectile.Center = Projectile.position;
                Projectile.Damage();
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int num615 = 0; num615 < 45; num615++)
            {
                int num616 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num616].velocity *= 1.4f;
            }

            for (int num617 = 0; num617 < 30; num617++)
            {
                int num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[num618].noGravity = true;
                Main.dust[num618].velocity *= 7f;
                num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num618].velocity *= 3f;
            }

            for (int num619 = 0; num619 < 3; num619++)
            {
                float scaleFactor9 = 0.4f;
                if (num619 == 1) scaleFactor9 = 0.8f;
                int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore97 = Main.gore[num620];
                gore97.velocity.X = gore97.velocity.X + 1f;
                Gore gore98 = Main.gore[num620];
                gore98.velocity.Y = gore98.velocity.Y + 1f;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore99 = Main.gore[num620];
                gore99.velocity.X = gore99.velocity.X - 1f;
                Gore gore100 = Main.gore[num620];
                gore100.velocity.Y = gore100.velocity.Y + 1f;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore101 = Main.gore[num620];
                gore101.velocity.X = gore101.velocity.X + 1f;
                Gore gore102 = Main.gore[num620];
                gore102.velocity.Y = gore102.velocity.Y - 1f;
                num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[num620].velocity *= scaleFactor9;
                Gore gore103 = Main.gore[num620];
                gore103.velocity.X = gore103.velocity.X - 1f;
                Gore gore104 = Main.gore[num620];
                gore104.velocity.Y = gore104.velocity.Y - 1f;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                int max = Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] < 16 ? 8 : 4;
                for (int i = 0; i < max; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                        Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.TwoPi / max * (i + Main.rand.NextFloat(-0.5f, 0.5f))) * Main.rand.NextFloat(12f, 20f), ModContent.ProjectileType<GolemGib>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0, Main.rand.Next(11) + 1);
                    if (p != Main.maxProjectiles)
                        Main.projectile[p].timeLeft = Main.rand.Next(45, 90) * 2;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color26 = Color.Orange * (Projectile.Opacity * headsStacked / maxHeadsStacked);
            color26.A = 20;
            if (Projectile.ai[1] == 0)
                color26 *= 0.5f;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = color26;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

            if (Projectile.ai[1] < 0) //flying, lights on
            {
                Texture2D eyes = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Vanilla/GolemLights1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Rectangle eyeRectangle = new Rectangle(0, eyes.Height / 2, eyes.Width, eyes.Height / 2);
                Vector2 eyeOrigin = eyeRectangle.Size() / 2f;
                eyeOrigin.Y -= 4;
                Main.EntitySpriteDraw(eyes, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(eyeRectangle), Color.White * Projectile.Opacity, Projectile.rotation, eyeOrigin, Projectile.scale, effects, 0);
            }

            return false;
        }
    }
}