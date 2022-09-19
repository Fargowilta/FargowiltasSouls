using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class SaucerMinion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Saucer");
            Description.SetDefault("The Mini Saucer will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "迷你飞碟");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "迷你飞碟将会保护你");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MiniSaucer = true;
            if (player.whoAmI == Main.myPlayer)
            {
                const int damage = 50;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniSaucer>()] < 1)
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<MiniSaucer>(), damage, 3f, player.whoAmI);
            }
        }
    }
}