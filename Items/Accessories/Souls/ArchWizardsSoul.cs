using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class ArchWizardsSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Arch Wizard's Soul");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "巫师之魂");


            string tooltip =
@"30% increased magic damage
20% increased spell casting speed
15% increased magic crit chance
Increases your maximum mana by 200
Effects of Celestial Cuffs and Mana Flower
'Arcane to the core'";
            Tooltip.SetDefault(tooltip);
            //string tooltip_ch =
//@"增加30%魔法伤害
//增加20%施法速度
//增加15%魔法暴击率
//增加200点最大法力值
//拥有天界手铐和魔力花效果
//'奥术之力，合核凝一'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);

        }

        protected override Color? nameColor => new Color(255, 83, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().MagicSoul = true;
            player.GetDamage(DamageClass.Magic) += .3f;
            player.GetCritChance(DamageClass.Magic) += 15;
            player.statManaMax2 += 200;
            //accessorys
            if (player.GetToggleValue("ManaFlower", false))
                player.manaFlower = true;
            //add mana cloak
            player.manaMagnet = true;
            player.magicCuffs = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(null, "ApprenticesEssence")
            .AddIngredient(ItemID.ManaCloak)
            .AddIngredient(ItemID.MagnetFlower)
            .AddIngredient(ItemID.ArcaneFlower)

            .AddIngredient(ItemID.CelestialCuffs)
            .AddIngredient(ItemID.CelestialEmblem)
            .AddIngredient(ItemID.MedusaHead)
            .AddIngredient(ItemID.SharpTears)
            .AddIngredient(ItemID.MagnetSphere)
            .AddIngredient(ItemID.RainbowGun)

            .AddIngredient(ItemID.ApprenticeStaffT3)
            .AddIngredient(ItemID.SparkleGuitar)
            .AddIngredient(ItemID.RazorbladeTyphoon)

            //.AddIngredient(ItemID.BlizzardStaff);
            .AddIngredient(ItemID.LaserMachinegun)
            .AddIngredient(ItemID.LastPrism)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();


        }
    }
}
