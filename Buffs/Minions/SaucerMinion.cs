using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SaucerMinion : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Mini Saucer");
            //Description.SetDefault("The Mini Saucer will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "迷你飞碟");
            //Description.AddTranslation(GameCulture.Chinese, "迷你飞碟将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().MiniSaucer = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("MiniSaucer")] < 1)
                    Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("MiniSaucer"), 0, 3f, player.whoAmI);
            }
        }
    }
}