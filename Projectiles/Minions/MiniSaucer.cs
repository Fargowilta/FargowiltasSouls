using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class MiniSaucer : ModProjectile
    {
        private int rotation = 0;
        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Saucer");
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.scale = 1f;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale = 1.5f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().MiniSaucer)
                Projectile.timeLeft = 2;

            /*NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
            if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
            {
                Projectile.ai[0] = minionAttackTargetNpc.whoAmI;
                Projectile.netUpdate = true;
            }*/

            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;
                mousePos.Y -= 250f;
            }

            if (Projectile.Distance(Main.player[Projectile.owner].Center) > 2000)
            {
                Projectile.Center = player.Center;
                Projectile.velocity = Vector2.UnitX.RotatedByRandom(2 * Math.PI) * 12f;
            }

            Vector2 distance = mousePos - Projectile.Center;
            float length = distance.Length();
            if (length > 10f)
            {
                distance /= 18f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }

                if (player.controlUseItem)
                {
                    if (++Projectile.localAI[0] > 5f) //shoot laser
                    {
                        Projectile.localAI[0] = 0f;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Vector2 vel = Projectile.DirectionTo(Main.MouseWorld) * 16f;
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);

                            FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 2.5f,
                                vel.RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 3.0),
                                ModContent.ProjectileType<SaucerLaser>(), Projectile.originalDamage / 2, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    if (++Projectile.localAI[1] > 20f) //try to find target for rocket
                    {
                        Projectile.localAI[1] = 0f;

                        float maxDistance = 500f;
                        int possibleTarget = -1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.CanBeChasedBy(Projectile) && Collision.CanHitLine(Projectile.Center, 0, 0, npc.Center, 0, 0))
                            {
                                float npcDistance = player.Distance(npc.Center);
                                if (npcDistance < maxDistance)
                                {
                                    maxDistance = npcDistance;
                                    possibleTarget = i;
                                }
                            }
                        }

                        if (possibleTarget >= 0) //shoot rocket
                        {
                            Vector2 vel = new Vector2(0f, -10f).RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI);
                            FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<SaucerRocket>(),
                                Projectile.originalDamage, Projectile.knockBack * 4f, Projectile.owner, possibleTarget, 20f);
                        }

                        FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<SaucerDeathray>(),
                            Projectile.originalDamage / 2, Projectile.knockBack / 2f, Projectile.owner, 0f, Projectile.identity);
                    }
                }
            }

            const float cap = 32f;
            if (Projectile.velocity.X > cap)
                Projectile.velocity.X = cap;
            if (Projectile.velocity.X < -cap)
                Projectile.velocity.X = -cap;
            if (Projectile.velocity.Y > cap)
                Projectile.velocity.Y = cap;
            if (Projectile.velocity.Y < -cap)
                Projectile.velocity.Y = -cap;

            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<SaucerDeathray>()] > 0)
            {
                Projectile.rotation = 0;
                rotation = 0;
            }
            else
            {
                Projectile.rotation = (float)Math.Sin(2 * Math.PI * rotation++ / 90) * (float)Math.PI / 8f;
                if (rotation > 180)
                    rotation = 0;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 360);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}