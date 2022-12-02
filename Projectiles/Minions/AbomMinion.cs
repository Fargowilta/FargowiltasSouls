using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class AbomMinion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Eternals/AbominationnSoul";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominationn");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            Projectile.scale = 1;

            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().AbomMinion)
                Projectile.timeLeft = 2;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs) //has target
            {
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[0] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
                    Projectile.ai[0] = minionAttackTargetNpc.whoAmI;

                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.CanBeChasedBy(Projectile))
                {
                    Projectile.direction = Projectile.spriteDirection = Projectile.Center.X > npc.Center.X ? 1 : -1;
                    Vector2 targetPos = npc.Center + Projectile.DirectionFrom(npc.Center) * 250;
                    if (Projectile.Distance(targetPos) > 50)
                        Movement(targetPos, 1f);
                    if (++Projectile.localAI[0] > 15)
                    {
                        Projectile.localAI[0] = 0;
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity + Projectile.DirectionTo(npc.Center) * 30, ModContent.ProjectileType<AbomMinionScythe>(), Projectile.damage, Projectile.knockBack / 2, Projectile.owner, npc.whoAmI);
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
                Projectile.direction = Projectile.spriteDirection = Projectile.Center.X > player.Center.X ? 1 : -1;

                Vector2 targetPos = player.Center;
                if (player.velocity.X > 0)
                    targetPos.X -= 100;
                else if (player.velocity.X < 0)
                    targetPos.X += 100;
                else
                    targetPos.X += 100 * -player.direction;
                targetPos.Y -= 50;

                if (Projectile.Distance(targetPos) > 3000)
                    Projectile.Center = player.Center;
                else if (Projectile.Distance(targetPos) > 50)
                    Movement(targetPos, 1f);

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
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
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
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
            target.AddBuff(BuffID.ShadowFlame, 600);
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
                Color color27 = Color.White * Projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity * 0.75f;
        }
    }
}
