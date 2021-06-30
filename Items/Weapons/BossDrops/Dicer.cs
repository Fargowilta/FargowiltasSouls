using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Utilities;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class Dicer : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dicer");
            Tooltip.SetDefault("'A defeated foe's attack now on a string'");

            DisplayName.AddTranslation(GameCulture.Chinese, "切肉器");
            Tooltip.AddTranslation(GameCulture.Chinese, "'败将之击，悬于一线之上'");

            ItemID.Sets.Yoyo[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.width = 24;
            item.height = 24;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.channel = true;
            item.noMelee = true;
            item.shoot = ModContent.ProjectileType<DicerProj>();
            item.useAnimation = 25;
            item.useTime = 25;
            item.shootSpeed = 16f;
            item.knockBack = 2.5f;
            item.damage = 60;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Yellow;
        }

        public override void HoldItem(Player player) => player.stringColor = 5;
    }
}
