using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class SouloftheMasochistBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul of the Master");
            // Description.SetDefault("The power of Eternity Mode is with you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐之魂");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "受虐模式的力量与你同在");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            if (player.whoAmI == Main.myPlayer)
            {
                Item item = null;

                if (player.AddEffect<SkeleMinionEffect>(item))
                {
                    fargoPlayer.SkeletronArms = true;
                    const int damage = 64;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmL>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmL>(), damage, 8f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronArmR>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SkeletronArmR>(), damage, 8f, player.whoAmI);
                }

                if (player.AddEffect<PungentMinion>(item))
                {
                    fargoPlayer.PungentEyeballMinion = true;
                    const int damage = 150;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PungentEyeballMinion>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<PungentEyeballMinion>(), damage, 0f, player.whoAmI);
                }


                if (player.AddEffect<RainbowSlimeMinion>(item))
                {
                    fargoPlayer.RainbowSlime = true;
                    const int damage = 105;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<RainbowSlime>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<RainbowSlime>(), damage, 3f, player.whoAmI);
                }

                if (player.AddEffect<ProbeMinionEffect>(item))
                {
                    fargoPlayer.Probes = true;
                    const int damage = 105;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe1>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe1>(), damage, 9f, player.whoAmI);
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<Probe2>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Probe2>(), damage, 9f, player.whoAmI, 0f, -1f);
                }

                if (player.AddEffect<PlantMinionEffect>(item))
                {
                    fargoPlayer.PlanterasChild = true;
                    const int damage = 120;
                    if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PlanterasChild>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PlanterasChild>(), damage, 3f, player.whoAmI);
                }

                if (player.AddEffect<UfoMinionEffect>(item))
                {
                    fargoPlayer.MiniSaucer = true;
                    const int damage = 100;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniSaucer>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<MiniSaucer>(), damage, 3f, player.whoAmI);
                }
                /*
                if (player.AddEffect<CultistMinionEffect>(item))
                {
                    fargoPlayer.LunarCultist = true;
                    const int damage = 160;
                    if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<LunarCultist>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<LunarCultist>(), damage, 2f, player.whoAmI, -1f);
                }
                */
                if (player.AddEffect<MasoTrueEyeMinion>(item))
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