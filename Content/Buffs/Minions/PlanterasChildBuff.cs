using FargowiltasSouls.Content.Projectiles.Minions;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class PlanterasChildBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plantera's Child");
            // Description.SetDefault("The child of Plantera will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "世纪之花的孩子将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().PlanterasChild = true;
            const int damage = 66;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<PlanterasChild>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, -Vector2.UnitY, ModContent.ProjectileType<PlanterasChild>(), damage, 3f, player.whoAmI);
        }
    }
}