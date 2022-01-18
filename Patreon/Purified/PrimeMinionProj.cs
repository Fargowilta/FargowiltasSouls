using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Micro Prime");
            Main.projFrames[projectile.type] = 7;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 38;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.netImportant = true;
            //projectile.minionSlots = 1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 18000;
            projectile.GetGlobalProjectile<Projectiles.FargoGlobalProjectile>().CanSplit = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) patronPlayer.PrimeMinion = false;
            if (patronPlayer.PrimeMinion) projectile.timeLeft = 2;
           // projectile.alpha = 0;

            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 6;
            }

            Vector2 targetPos;
            bool spin = false;

            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[1]);
            if (npc == null)
            {
                targetPos = player.Top - 32 * Vector2.UnitY;
                projectile.direction = projectile.spriteDirection = player.direction;

                if (projectile.Distance(targetPos) > 1200)
                    projectile.Center = player.Center;

                if (++projectile.localAI[0] > 10)
                {
                    projectile.localAI[0] = 0;
                    projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(projectile, 800f, true, player.Center);
                    projectile.netUpdate = true;
                }
            }
            else
            {
                if (++projectile.ai[0] > 360)
                {
                    spin = true;
                }

                if (projectile.ai[0] > 540)
                {
                    projectile.ai[0] = 0;
                    projectile.netUpdate = true;
                }

                targetPos = spin ? npc.Center : npc.Top - 32 * Vector2.UnitY;

                if (!spin)
                    projectile.direction = projectile.spriteDirection = Math.Sign(npc.Center.X - projectile.Center.X);

                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && projectile.ai[1] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy(projectile))
                {
                    projectile.ai[1] = minionAttackTargetNpc.whoAmI;
                    projectile.netUpdate = true;
                }

                if (!npc.CanBeChasedBy() || player.Distance(npc.Center) > 1200)
                {
                    projectile.ai[1] = -1;
                    projectile.netUpdate = true;
                }
            }
            
            if (projectile.Distance(targetPos) > 16 || spin)
            {
                float speed = npc == null ? 12f : 16f;
                float lerp = 0.03f;
                lerp += 0.03f * Math.Min(1f, projectile.localAI[1] / 300f); //gradually gets better tracking until it gets in range

                if (spin)
                {
                    speed *= 1.5f;
                    lerp *= 2f;
                }

                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(targetPos) * speed, lerp);
            }
            else
            {
                projectile.velocity *= 0.99f;
                projectile.localAI[1] = 0;
            }

            if (spin)
            {
                projectile.rotation += MathHelper.TwoPi / 20f;
                projectile.direction = projectile.spriteDirection = 1;
            }
            else
            {
                projectile.rotation = 0;
            }

            //
            //projectile.velocity = Vector2.Lerp(projectile.velocity, (projectile.DirectionTo(mousePos) * 20), 0.05f);
            //projectile.alpha = (int)(Math.Cos(projectile.ai[0] * MathHelper.TwoPi / 180) * 122.5 + 122.5);
            // Main.NewText(projectile.DirectionTo(Main.MouseWorld).ToString());
            /*projectile.ai[0]++;
            if (projectile.ai[0] == 180)
            {

                projectile.Center = Main.MouseWorld;
                //projectile.velocity = projectile.DirectionTo(Main.MouseWorld) * 8;
                projectile.netUpdate = true;
                projectile.ai[0] = 0;
            }*/
        }
    }
}
    