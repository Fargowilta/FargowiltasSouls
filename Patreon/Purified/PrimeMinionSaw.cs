using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionSaw : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Prime Saw");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.minion = true;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) patronPlayer.PrimeMinion = false;
            if (patronPlayer.PrimeMinion) projectile.timeLeft = 2;

            int head = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<PrimeMinionProj>() && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner)
                {
                    head = i;
                }
            }
            if (head == -1)
            {
                if (projectile.owner == Main.myPlayer)
                    projectile.Kill();
            }
            else
            {
                for (int index = 0; index < 1000; ++index)
                {
                    if (index != projectile.whoAmI && Main.projectile[index].active && (Main.projectile[index].owner == projectile.owner && Main.projectile[index].type == projectile.type) && (double)Math.Abs((float)(projectile.position.X - Main.projectile[index].position.X)) + (double)Math.Abs((float)(projectile.position.Y - Main.projectile[index].position.Y)) < (double)projectile.width)
                    {
                        if (projectile.position.X < Main.projectile[index].position.X)
                        {
                            projectile.velocity.X -= 0.2f;
                        }
                        else
                        {
                            projectile.velocity.X += 0.2f;
                        }
                        if (projectile.position.Y < Main.projectile[index].position.Y)
                        {
                            projectile.velocity.Y -= 0.2f;
                        }
                        else
                        {
                            projectile.velocity.Y += 0.2f;
                        }
                    }
                }

                bool targetting = false;
                NPC targetnpc = null;
                NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object)this, false))
                {
                    Vector2 distancetotarget = minionAttackTargetNpc.Center - projectile.Center;
                    Vector2 headtoTarget = minionAttackTargetNpc.Center - Main.projectile[head].Center;
                    if (distancetotarget.Length() < 1000 && headtoTarget.Length() < 200)
                    {
                        targetnpc = minionAttackTargetNpc;
                        targetting = true;
                    }
                }
                else if (!targetting)
                {
                    float distancemax = 1000;
                    for (int index = 0; index < 200; ++index)
                    {
                        if (Main.npc[index].CanBeChasedBy((object)this, false))
                        {
                            Vector2 distancetotarget = Main.npc[index].Center - projectile.Center;
                            Vector2 headtotarget = Main.npc[index].Center - Main.projectile[head].Center;
                            if (distancetotarget.Length() < distancemax && headtotarget.Length() < 200)
                            {
                                distancemax = distancetotarget.Length();
                                targetnpc = Main.npc[index];
                                targetting = true;
                            }
                        }
                    }
                }

                if (targetting)
                {
                    projectile.direction = projectile.spriteDirection = Math.Sign(targetnpc.Center.X - projectile.Center.X);

                    float movespeed = Math.Max(projectile.Distance(targetnpc.Center) / 40f, 18f);

                    if (projectile.Distance(targetnpc.Center) > 32)
                        projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(targetnpc.Center) * movespeed, 0.05f);
                }
                else
                {
                    projectile.direction = projectile.spriteDirection = Main.projectile[head].spriteDirection;

                    float movespeed = Math.Max(projectile.Distance(Main.projectile[head].Center) / 40f, 14f);

                    if (projectile.Distance(Main.projectile[head].Center) > 32)
                        projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.DirectionTo(Main.projectile[head].Center) * movespeed, 0.04f);
                }

                projectile.position += Main.projectile[head].velocity * 0.8f;
            }
        }
    }
}
