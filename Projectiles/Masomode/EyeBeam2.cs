using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.NPCs.Champions;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class EyeBeam2 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_259";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Eye Beam");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeBeam);
            aiType = ProjectileID.EyeBeam;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 300);
            NPC npc = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (npc != null)
            {
                target.AddBuff(BuffID.BrokenArmor, 600);
                target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
                target.AddBuff(BuffID.WitheredArmor, 600);
                if (Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] == null || //outside temple
                    Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].wall != WallID.LihzahrdBrickUnsafe)
                {
                    target.AddBuff(BuffID.Burning, 120);
                }
            }

            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<EarthChampion>()))
            {
                target.AddBuff(BuffID.Burning, 300);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }
    }
}