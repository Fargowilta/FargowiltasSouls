using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class RainbowSlime : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Rainbow Slime");
            //Description.SetDefault("The Rainbow Slime will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "彩虹史莱姆");
            //Description.AddTranslation(GameCulture.Chinese, "彩虹史莱姆将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().RainbowSlime = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("RainbowSlime")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("RainbowSlime"), 0, 3f, player.whoAmI);
            }
        }
    }
}