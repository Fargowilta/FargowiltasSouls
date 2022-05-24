using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Shield)]
    public class ColossusSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Colossus Soul");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "巨像之魂");

            string tooltip =
@"Increases HP by 100
15% damage reduction
Increases life regeneration by 5
Grants immunity to knockback, several debuffs, and fall damage
Enemies are more likely to target you
Effects of Brain of Confusion, Star Veil, and Bee Cloak
Effects of Shiny Stone, Paladin's Shield, and Frozen Turtle Shell
'Nothing can stop you'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"增加100点最大生命值
增加15%伤害减免
增加5点生命恢复速度
使你免疫击退、一些减益和摔落伤害
增加敌人以你为目标的几率
拥有混乱之脑、星星面纱和蜜蜂斗篷效果
拥有闪亮石、圣骑士护盾和冰冻海龟壳效果
'你无人可挡'";
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.defense = 10;
            Item.shieldSlot = 4;
        }

        protected override Color? nameColor => new Color(252, 59, 0);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //any new effects, brain of confusion
            modPlayer.ColossusSoul(Item, 100, 0.15f, 5, hideVisual);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.HandWarmer)
            .AddIngredient(ItemID.ObsidianHorseshoe)
            .AddIngredient(ItemID.WormScarf)
            .AddIngredient(ItemID.BrainOfConfusion)
            .AddIngredient(ItemID.PocketMirror)
            .AddIngredient(ItemID.CharmofMyths)
            .AddIngredient(ItemID.BeeCloak)
            .AddIngredient(ItemID.StarVeil)
            .AddIngredient(ItemID.ShinyStone)
            .AddIngredient(ItemID.HeroShield)
            .AddIngredient(ItemID.FrozenShield)
            .AddIngredient(ItemID.AnkhShield)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))


            .Register();
        }
    }
}
