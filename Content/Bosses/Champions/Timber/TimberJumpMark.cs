using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Timber
{
    public class TimberJumpMark : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Timber Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<TimberChampion>());
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.localAI[0] > 4)
            {
                Projectile.localAI[0] = 0;

                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 spawnPos = Projectile.Center + Projectile.ai[1] * Projectile.localAI[1] * i * Vector2.UnitX;

                    SoundEngine.PlaySound(SoundID.Item14, spawnPos);

                    if (FargoSoulsUtil.HostCheck)
                    {
                        Projectile p = Projectile.NewProjectileDirect(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, Projectile.damage, 0f, Main.myPlayer);
                        p.friendly = false;
                        p.hostile = true;
                    }

                }

                int max = WorldSavingSystem.MasochistModeReal ? 18 : 6;
                if (++Projectile.localAI[1] > max)
                    Projectile.Kill();
            }
        }
    }
}
