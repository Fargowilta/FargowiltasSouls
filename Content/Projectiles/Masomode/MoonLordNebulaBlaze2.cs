using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MoonLordNebulaBlaze2 : MoonLordNebulaBlaze
    {
        public override string Texture => "Terraria/Images/Projectile_634";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.timeLeft = 2400 * 3;
        }

        public override bool? CanDamage()
        {
            if (base.CanDamage() == false)
                return false;

            return counter > 30 * Projectile.MaxUpdates;
        }

        public int counter;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.MoonLordCore);
            if (npc == null || npc.ai[0] == 2f)
            {
                Projectile.Kill();
                return;
            }

            counter++;

            if (Projectile.ai[1] == 0) //identify the ritual CLIENT SIDE
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<LunarRitual>() && Main.projectile[i].ai[1] == npc.whoAmI)
                    {
                        Projectile.localAI[1] = i;
                        break;
                    }
                }
            }

            Projectile ritual = FargoSoulsUtil.ProjectileExists(Projectile.localAI[1], ModContent.ProjectileType<LunarRitual>());
            if (ritual != null && ritual.ai[1] == npc.whoAmI)
            {
                if (Projectile.Distance(ritual.Center) > 1600f) //bounce off arena walls
                {
                    if (npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState != 2)
                    {
                        Projectile.Kill();
                        return;
                    }

                    Projectile.velocity = -Projectile.velocity;
                    Vector2 toCenter = ritual.Center - Projectile.Center;
                    float rotationDifference = toCenter.ToRotation() - Projectile.velocity.ToRotation();
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.WrapAngle(2 * rotationDifference * Main.rand.NextFloat(0.8f, 1.2f)));

                    Projectile.position -= ritual.velocity;

                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }

            base.AI();
        }
    }
}