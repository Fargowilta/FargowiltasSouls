using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class PlanterasChild : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera's Child");
            Description.SetDefault("The child of Plantera will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子");
            Description.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MagicalBulb = true;
            const int damage = 60;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.PlanterasChild>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY, ModContent.ProjectileType<Projectiles.Minions.PlanterasChild>(), damage, 3f, player.whoAmI);
        }
    }
}