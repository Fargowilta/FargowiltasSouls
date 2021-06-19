using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class TheSmallSting : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Small Sting");
            Tooltip.SetDefault("Uses darts for ammo" +
                "\n50% chance to not consume ammo" +
                "\nStingers will stick to enemies, hitting the same spot again will deal extra damage" +
                "\n'Repurposed from the abdomen of a defeated foe..'");
                            DisplayName.AddTranslation(GameCulture.Chinese, "小螫刺");
            Tooltip.AddTranslation(GameCulture.Chinese, "使用飞镖作为弹药" +
                "\n有50%的几率不消耗弹药" +
                "\n刺会粘在敌人身上，再次命中会造成额外伤害" +
                "\n'重装自被打败敌人的腹部..");
        }

        public override void SetDefaults()
        {
            item.damage = 39;
            item.crit = 0;
            item.ranged = true;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = 50000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SmallStinger>();
            item.useAmmo = AmmoID.Dart;
            item.UseSound = SoundID.Item97;
            item.shootSpeed = 40f;
            item.width = 44;
            item.height = 16;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<SmallStinger>();

            return true;
        }

        // Remove the Crit Chance line because of a custom crit method
        public override void SafeModifyTooltips(List<TooltipLine> tooltips) => tooltips.Remove(tooltips.FirstOrDefault(line => line.Name == "CritChance" && line.mod == "Terraria"));

        //make them hold it different
        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(2) == 0;
    }
}
