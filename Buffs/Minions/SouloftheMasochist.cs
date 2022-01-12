using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SouloftheMasochist : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Soul of the Siblings");
            //Description.SetDefault("The power of Eternity Mode is with you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "受虐之魂");
            //Description.AddTranslation(GameCulture.Chinese, "受虐模式的力量与你同在");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

                if (player.GetToggleValue("MasoSkele"))
                {
                    fargoPlayer.SkeletronArms = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("SkeletronArmL")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("SkeletronArmL"), 0, 8f, player.whoAmI);
                    if (player.ownedProjectileCounts[mod.ProjectileType("SkeletronArmR")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("SkeletronArmR"), 0, 8f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoPugent"))
                {
                    fargoPlayer.PungentEyeballMinion = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("PungentEyeball")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("PungentEyeball"), 0, 0f, player.whoAmI);
                }

                if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoRainbow"))
                {
                    fargoPlayer.RainbowSlime = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("RainbowSlime")] < 1)
                    {
                        Projectile pro = Main.projectile[Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("RainbowSlime"), 0, 3f, player.whoAmI)];
                        pro.netUpdate = true;
                    }
                }

                if (player.GetToggleValue("MasoProbe"))
                {
                    fargoPlayer.Probes = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("Probe1")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("Probe1"), 0, 9f, player.whoAmI);
                    if (player.ownedProjectileCounts[mod.ProjectileType("Probe2")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("Probe2"), 0, 9f, player.whoAmI, 0f, -1f);
                }

                if (player.GetToggleValue("MasoPlant"))
                {
                    fargoPlayer.MagicalBulb = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("PlanterasChild")] < 1)
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, -0.15f, -0.1f, mod.ProjectileType("PlanterasChild"), 0, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoFlocko"))
                {
                    fargoPlayer.SuperFlocko = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("SuperFlocko")] < 1)
                        Projectile.NewProjectile(player.Center, new Vector2(0f, -10f), mod.ProjectileType("SuperFlocko"), 0, 4f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoUfo"))
                {
                    fargoPlayer.MiniSaucer = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("MiniSaucer")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("MiniSaucer"), 0, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoCultist"))
                {
                    fargoPlayer.LunarCultist = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("LunarCultist")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("LunarCultist"), 0, 2f, player.whoAmI, -1f);
                }

                if (player.GetToggleValue("MasoTrueEye"))
                {
                    fargoPlayer.TrueEyes = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeL")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeL"), 0, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeR")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeR"), 0, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[mod.ProjectileType("TrueEyeS")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("TrueEyeS"), 0, 3f, player.whoAmI, -1f);
                }
            }
        }
    }
}