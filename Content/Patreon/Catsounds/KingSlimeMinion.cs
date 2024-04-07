using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Catsounds
{
    public class KingSlimeMinion : ModProjectile
    {
        public bool goingDown = false;
        public int spikeAttackCounter;
        public int slimeAttackCounter;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slime");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.alpha = 75;
            Projectile.width = 38;
            Projectile.height = 40;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = 26;
            AIType = ProjectileID.BabySlime;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<PatreonPlayer>().KingSlimeMinion)
                Projectile.timeLeft = 2;
            else
                Projectile.Kill();

            //no attacks while flying 
            if (Projectile.frame >= 2) return;

            //spike attack
            if (goingDown)
            {
                if (Projectile.velocity.Y <= 0f) //start attack
                {
                    goingDown = false;
                    spikeAttackCounter++;

                    if (spikeAttackCounter >= 10)
                    {
                        spikeAttackCounter = 0;
                        if (Projectile.owner == Main.myPlayer && FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 400, true) != -1)
                        {
                            for (int i = 0; i < 25; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X + Main.rand.Next(-5, 5), Projectile.Center.Y - 15),
                                    new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)),
                                    ModContent.ProjectileType<KingSlimeSpike>(), Projectile.damage, Projectile.knockBack / 2f, Main.myPlayer);
                            }
                        }
                    }

                }
            }
            else if (Projectile.velocity.Y > 0)
            {
                goingDown = true;
            }

            //slime rain attack
            if (++slimeAttackCounter > 150)
            {
                slimeAttackCounter = 0;

                int target = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 800, true);
                if (target != -1 && Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 spawn = Main.npc[target].Center + Main.npc[target].velocity * Main.rand.NextFloat(15f);
                        spawn.X += Main.rand.Next(-50, 51);
                        spawn.Y -= Main.rand.Next(600, 701);
                        Vector2 speed = Main.npc[target].Center - spawn;
                        speed.Normalize();
                        speed *= Main.rand.NextFloat(10f, 20f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawn, speed, ModContent.ProjectileType<KingSlimeBallPiercing>(), Projectile.damage, 0f, Main.myPlayer);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.position.Y + Projectile.height;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
    }
}