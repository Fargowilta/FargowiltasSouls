using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionSaw : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prime Saw");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) patronPlayer.PrimeMinion = false;
            if (patronPlayer.PrimeMinion) Projectile.timeLeft = 2;

            int head = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<PrimeMinionProj>() && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner)
                {
                    head = i;
                }
            }
            if (head == -1)
            {
                if (Projectile.owner == Main.myPlayer)
                    Projectile.Kill();
            }
            else
            {
                for (int index = 0; index < 1000; ++index)
                {
                    if (index != Projectile.whoAmI && Main.projectile[index].active && (Main.projectile[index].owner == Projectile.owner && Main.projectile[index].type == Projectile.type) && (double)Math.Abs((float)(Projectile.position.X - Main.projectile[index].position.X)) + (double)Math.Abs((float)(Projectile.position.Y - Main.projectile[index].position.Y)) < (double)Projectile.width)
                    {
                        if (Projectile.position.X < Main.projectile[index].position.X)
                        {
                            Projectile.velocity.X -= 0.2f;
                        }
                        else
                        {
                            Projectile.velocity.X += 0.2f;
                        }
                        if (Projectile.position.Y < Main.projectile[index].position.Y)
                        {
                            Projectile.velocity.Y -= 0.2f;
                        }
                        else
                        {
                            Projectile.velocity.Y += 0.2f;
                        }
                    }
                }

                bool targetting = false;
                NPC targetnpc = null;
                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object)this, false))
                {
                    Vector2 distancetotarget = minionAttackTargetNpc.Center - Projectile.Center;
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
                            Vector2 distancetotarget = Main.npc[index].Center - Projectile.Center;
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
                    Projectile.direction = Projectile.spriteDirection = Math.Sign(targetnpc.Center.X - Projectile.Center.X);

                    float movespeed = Math.Max(Projectile.Distance(targetnpc.Center) / 40f, 18f);

                    if (Projectile.Distance(targetnpc.Center) > 32)
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(targetnpc.Center) * movespeed, 0.05f);
                }
                else
                {
                    Projectile.direction = Projectile.spriteDirection = Main.projectile[head].spriteDirection;

                    float movespeed = Math.Max(Projectile.Distance(Main.projectile[head].Center) / 40f, 14f);

                    if (Projectile.Distance(Main.projectile[head].Center) > 32)
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.projectile[head].Center) * movespeed, 0.04f);
                }

                Projectile.position += Main.projectile[head].velocity * 0.8f;
            }
        }
    }
}
