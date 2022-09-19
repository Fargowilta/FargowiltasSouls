using FargowiltasSouls.Projectiles.Minions;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class MutantPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Power");
            Description.SetDefault("The power of Mutant is with you");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "突变之力");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "突变之力与你同在");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

                if (player.GetToggleValue("MasoAbom"))
                {
                    fargoPlayer.AbomMinion = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<AbomMinion>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<AbomMinion>(), 900, 10f, player.whoAmI, -1);
                }

                if (player.GetToggleValue("MasoRing"))
                {
                    fargoPlayer.PhantasmalRing = true;
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalRing>()] < 1)
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalRing>(), 1700, 0f, player.whoAmI);
                }
            }
        }
    }
}