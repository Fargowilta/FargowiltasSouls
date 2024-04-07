using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.DeviBoss
{
    public class DeviRainHeart2 : DeviRainHeart
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/FakeHeart";

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<DeviBoss>());
            if (npc != null)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    if (WorldSavingSystem.MasochistModeReal)
                    {
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, -Vector2.UnitY, ModContent.ProjectileType<DeviDeathray>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (Main.player[npc.target].Center.Y > Projectile.Center.Y)
                            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<DeviDeathray>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    else
                    {
                        SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);

                        for (int i = 0; i < 5; i++)
                        {
                            float speed = 4f + i * 8f;
                            int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, -Vector2.UnitY * speed, ModContent.ProjectileType<DeviHeart>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = 20;

                            if (Main.player[npc.target].Center.Y > Projectile.Center.Y)
                            {
                                p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.UnitY * speed, ModContent.ProjectileType<DeviHeart>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                if (p != Main.maxProjectiles)
                                    Main.projectile[p].timeLeft = 20;
                            }
                        }
                    }
                }
            }
        }
    }
}