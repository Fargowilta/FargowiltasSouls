using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class CreeperMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain Proj");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.dead) modPlayer.BrainMinion = false;
            if (modPlayer.BrainMinion) Projectile.timeLeft = 2;

            int Brain = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<BrainMinion>() && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner)
                {
                    Brain = i;
                }
            }
            if (Brain == -1)
                Projectile.Kill();
            else
            {
                for (int index = 0; index < Main.maxProjectiles; ++index)
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

                NPC targetnpc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, true));
                bool targetting = targetnpc != null;
                if (!targetting || Projectile.ai[0] > 0)
                {
                    float movespeed = Math.Max(Projectile.Distance(Main.projectile[Brain].Center) / 40f, 10f);

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.projectile[Brain].Center) * movespeed, 0.04f);
                    if (Projectile.Hitbox.Intersects(Main.projectile[Brain].Hitbox))
                    {
                        Projectile.ai[0] = 0;
                    }
                }
                if (targetting && Projectile.ai[0] == 0)
                {
                    float movespeed = Math.Max(Projectile.Distance(targetnpc.Center) / 40f, 14f);

                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(targetnpc.Center) * movespeed, 0.05f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.ai[0]++;
        }
    }
}