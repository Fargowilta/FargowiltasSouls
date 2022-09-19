using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SouloftheMasochist : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of the Master");
            Description.SetDefault("The power of Eternity Mode is with you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐之魂");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐模式的力量与你同在");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                if (player.GetToggleValue("MasoSkele"))
                {
                    fargoPlayer.SkeletronArms = true;
                    const int damage = 64;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmL>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmL>(), damage, 8f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmR>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmR>(), damage, 8f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoPugent"))
                {
                    fargoPlayer.PungentEyeballMinion = true;
                    const int damage = 150;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.PungentEyeball>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.PungentEyeball>(), damage, 0f, player.whoAmI);
                }

                if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoRainbow"))
                {
                    fargoPlayer.RainbowSlime = true;
                    const int damage = 105;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.RainbowSlime>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.RainbowSlime>(), damage, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoProbe"))
                {
                    fargoPlayer.Probes = true;
                    const int damage = 105;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe1>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe1>(), damage, 9f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe2>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe2>(), damage, 9f, player.whoAmI, 0f, -1f);
                }

                if (player.GetToggleValue("MasoPlant"))
                {
                    fargoPlayer.MagicalBulb = true;
                    const int damage = 120;
                    if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.PlanterasChild>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY, ModContent.ProjectileType<Projectiles.Minions.PlanterasChild>(), damage, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoUfo"))
                {
                    fargoPlayer.MiniSaucer = true;
                    const int damage = 100;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniSaucer>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<MiniSaucer>(), damage, 3f, player.whoAmI);
                }

                if (player.GetToggleValue("MasoCultist"))
                {
                    fargoPlayer.LunarCultist = true;
                    const int damage = 160;
                    if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.LunarCultist>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.LunarCultist>(), damage, 2f, player.whoAmI, -1f);
                }

                if (player.GetToggleValue("MasoTrueEye"))
                {
                    fargoPlayer.TrueEyes = true;

                    const int damage = 180;

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeL>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeL>(), damage, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeR>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeR>(), damage, 3f, player.whoAmI, -1f);

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<TrueEyeS>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueEyeS>(), damage, 3f, player.whoAmI, -1f);
                }
            }
        }
    }
}