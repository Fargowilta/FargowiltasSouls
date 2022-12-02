using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class OpticRetinazer : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retinazer");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.minionSlots = 1.5f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;
            Projectile.scale = .5f;

            /*Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;*/
        }

        private bool spawn;

        public override void AI()
        {
            if (!spawn)
            {
                spawn = true;
                Projectile.ai[0] = -1;
            }

            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().TwinsEX)
                Projectile.timeLeft = 2;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(Projectile))
                    Projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.CanBeChasedBy(Projectile))
                {
                    Projectile.position += npc.velocity / 4f;

                    Vector2 target = npc.Center + npc.velocity * Projectile.ai[1];
                    Vector2 targetPos = target + Projectile.DirectionFrom(target) * 300;
                    if (Projectile.Distance(targetPos) > 50)
                        Movement(targetPos, 0.5f);
                    Projectile.rotation = Projectile.DirectionTo(target).ToRotation() - (float)Math.PI / 2;

                    if (++Projectile.localAI[0] > 15)
                    {
                        Projectile.localAI[0] = 0;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                Projectile.DirectionTo(target) * 12, ModContent.ProjectileType<OpticLaser>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Projectile.ai[1] = Main.rand.NextFloat(10, 30);
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else //forget target
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                    Projectile.netUpdate = true;
                }
            }
            else //no target
            {
                Projectile.ai[1] = 0;

                Vector2 targetPos = player.Center;
                targetPos.Y -= 100;

                if (Projectile.Distance(targetPos) > 3000)
                    Projectile.Center = player.Center;
                else if (Projectile.Distance(targetPos) > 200)
                    Movement(targetPos, 0.5f);

                Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 2;

                if (++Projectile.localAI[1] > 6)
                {
                    Projectile.localAI[1] = 0;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1500);
                    if (Projectile.ai[0] != -1)
                        Projectile.netUpdate = true;
                }
            }

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 3;
            }
            if (Projectile.frame < 3)
                Projectile.frame = 3;

            const float IdleAccel = 0.05f;
            int otherMinion = ModContent.ProjectileType<OpticSpazmatism>();
            foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == Projectile.owner && (p.type == Projectile.type || p.type == otherMinion) && p.whoAmI != Projectile.whoAmI && p.Distance(Projectile.Center) < Projectile.width))
            {
                Projectile.velocity.X += IdleAccel * (Projectile.Center.X < p.Center.X ? -1 : 1);
                Projectile.velocity.Y += IdleAccel * (Projectile.Center.Y < p.Center.Y ? -1 : 1);
                p.velocity.X += IdleAccel * (p.Center.X < Projectile.Center.X ? -1 : 1);
                p.velocity.Y += IdleAccel * (p.Center.Y < Projectile.Center.Y ? -1 : 1);
            }
        }

        private void Movement(Vector2 targetPos, float speedModifier)
        {
            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > 24)
                Projectile.velocity.X = 24 * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > 24)
                Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(BuffID.Ichor, 600);
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

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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