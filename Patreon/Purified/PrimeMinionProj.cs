using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Purified
{
    public class PrimeMinionProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Micro Prime");
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 38;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            //Projectile.minionSlots = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.GetGlobalProjectile<Projectiles.FargoSoulsGlobalProjectile>().CanSplit = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer patronPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead) patronPlayer.PrimeMinion = false;
            if (patronPlayer.PrimeMinion) Projectile.timeLeft = 2;
            // Projectile.alpha = 0;

            bool foundLimbs = false;
            int[] limbs = new int[]
            {
                ModContent.ProjectileType<PrimeMinionCannon>(),
                ModContent.ProjectileType<PrimeMinionLaserGun>(),
                ModContent.ProjectileType<PrimeMinionSaw>(),
                ModContent.ProjectileType<PrimeMinionVice>()
            };
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && limbs.Contains(Main.projectile[i].type))
                {
                    foundLimbs = true;
                    break;
                }
            }
            if (!foundLimbs && Projectile.owner == Main.myPlayer)
                Projectile.Kill();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 6;
            }

            Vector2 targetPos;
            bool spin = false;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1]);
            if (npc == null)
            {
                targetPos = player.Top - 32 * Vector2.UnitY;
                Projectile.direction = Projectile.spriteDirection = player.direction;

                if (Projectile.Distance(targetPos) > 1200)
                    Projectile.Center = player.Center;

                if (++Projectile.localAI[0] > 10)
                {
                    Projectile.localAI[0] = 0;
                    Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 800f, true, player.Center);
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (++Projectile.ai[0] > 360)
                {
                    spin = true;
                }

                if (Projectile.ai[0] > 540)
                {
                    Projectile.ai[0] = 0;
                    Projectile.netUpdate = true;
                }

                targetPos = spin ? npc.Center : npc.Top - 32 * Vector2.UnitY;

                if (!spin)
                    Projectile.direction = Projectile.spriteDirection = Math.Sign(npc.Center.X - Projectile.Center.X);

                NPC minionAttackTargetNpc = Projectile.OwnerMinionAttackTargetNPC;
                if (minionAttackTargetNpc != null && Projectile.ai[1] != minionAttackTargetNpc.whoAmI && minionAttackTargetNpc.CanBeChasedBy())
                {
                    Projectile.ai[1] = minionAttackTargetNpc.whoAmI;
                    Projectile.netUpdate = true;
                }

                if (!npc.CanBeChasedBy() || player.Distance(npc.Center) > 1200)
                {
                    Projectile.ai[1] = -1;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.Distance(targetPos) > 16 || spin)
            {
                float speed = npc == null ? 12f : 16f;
                float lerp = 0.03f;
                lerp += 0.03f * Math.Min(1f, Projectile.localAI[1] / 300f); //gradually gets better tracking until it gets in range

                if (spin)
                {
                    speed *= 1.5f;
                    lerp *= 2f;
                }

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(targetPos) * speed, lerp);
            }
            else
            {
                Projectile.velocity *= 0.99f;
                Projectile.localAI[1] = 0;
            }

            if (spin)
            {
                Projectile.rotation += MathHelper.TwoPi / 20f;
                Projectile.direction = Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.rotation = 0;
            }

            //
            //Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Projectile.DirectionTo(mousePos) * 20), 0.05f);
            //Projectile.alpha = (int)(Math.Cos(Projectile.ai[0] * MathHelper.TwoPi / 180) * 122.5 + 122.5);
            // Main.NewText(Projectile.DirectionTo(Main.MouseWorld).ToString());
            /*Projectile.ai[0]++;
            if (Projectile.ai[0] == 180)
            {

                Projectile.Center = Main.MouseWorld;
                //Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 8;
                Projectile.netUpdate = true;
                Projectile.ai[0] = 0;
            }*/
        }
    }
}
