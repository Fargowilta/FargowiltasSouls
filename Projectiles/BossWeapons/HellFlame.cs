using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HellFlame : ModProjectile
    {
        public int targetID = -1;
        public int searchTimer = 18;

        public override string Texture => "Terraria/Images/Projectile_687";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Flame");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetID = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Black;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.frame = (int)Projectile.ai[0];
                Projectile.ai[0] = -1;

                Projectile.localAI[0] = Main.rand.NextFloat(0.25f, 2f); //used for random variation in homing
                searchTimer = Main.rand.Next(18);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (Projectile.timeLeft > 120) Projectile.timeLeft = 120;
            Projectile.ai[1]++;
            Projectile.scale = 1f + Projectile.ai[1] / 80;

            Projectile.rotation += 0.3f * Projectile.direction;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 17)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            if (Projectile.frame > 6)
                Projectile.Kill();

            if (Projectile.frame > 4)
            {
                Projectile.alpha = 155;
                return;
            }

            if (targetID == -1) //no target atm
            {
                if (searchTimer == 0) //search every 18/3=6 ticks
                {
                    searchTimer = 18;
                    targetID = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 300);
                    Projectile.netUpdate = true;
                }
                searchTimer--;
            }
            else //currently have target
            {
                NPC npc = Main.npc[targetID];

                if (npc.active && npc.CanBeChasedBy()) //target is still valid
                {
                    Vector2 distance = npc.Center - Projectile.Center;
                    double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (Projectile.ai[0] == -1)
                    {
                        if (Math.Abs(angle) > Math.PI * 0.75)
                        {
                            Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.07 * Projectile.localAI[0]);
                        }
                        else
                        {
                            float range = distance.Length();
                            float difference = 12.7f / range;
                            distance *= difference;
                            distance /= 7f;
                            Projectile.velocity += distance;
                            if (range > 70f)
                            {
                                Projectile.velocity *= 0.977f;
                            }
                        }
                    }
                    else
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.1 * Projectile.localAI[0]);
                    }
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.X -= 30;
            hitbox.Y -= 30;
            hitbox.Width += 60;
            hitbox.Height += 60;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5;
            target.AddBuff(BuffID.OnFire, 180, false);
            target.AddBuff(BuffID.Oiled, 180, false);
            target.AddBuff(BuffID.BetsysCurse, 180, false);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Fuchsia * Projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale;// * 0.9f;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + (Main.GlobalTime * 0.6f);
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                    color27, num165, origin2, scale, effects, 0);
            }*/

            Color color27 = Color.Fuchsia * Projectile.Opacity;
            float scale = Projectile.scale;
            Vector2 value4 = Projectile.Center;
            if (Projectile.velocity != Vector2.Zero && !Projectile.velocity.HasNaNs())
                value4 -= Vector2.Normalize(Projectile.velocity) * 4f;
            float num165 = Projectile.rotation + (Main.GlobalTimeWrappedHourly * 0.6f);

            Vector2 previousPosOffset = Projectile.oldPos[2] - Projectile.position;
            float prevPosRotation = Projectile.oldRot[2] + (Main.GlobalTimeWrappedHourly * 0.6f);
            Main.EntitySpriteDraw(texture2D13, previousPosOffset + value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color27, prevPosRotation, origin2, scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, previousPosOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Color.Black * Projectile.Opacity, prevPosRotation, origin2, Projectile.scale, effects, 0);

            Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                color27, num165, origin2, scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                Color.Black * Projectile.Opacity, num165, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}