using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class RainbowSlime : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Slime");
            Description.SetDefault("The Rainbow Slime will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "彩虹史莱姆");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "彩虹史莱姆将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().RainbowSlime = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.RainbowSlime>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.RainbowSlime>(), 35, 3f, player.whoAmI);
            }
        }
    }
}