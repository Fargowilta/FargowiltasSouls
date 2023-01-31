using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MythrilEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Mythril Enchantment");
            Tooltip.SetDefault(
@"Temporarily increases attack speed after not attacking for a while
Bonus ends after attacking for 3 seconds and rebuilds over 5 seconds
'You feel the knowledge of your weapons seep into your mind'");
        }

        protected override Color nameColor => new Color(157, 210, 144);
        public override string wizardEffect => Language.GetTextValue("Mods.FargowiltasSouls.WizardEffect.Mythril");

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MythrilEffect(player, Item);
        }

        public static void MythrilEffect(Player player, Item item)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (!player.GetToggleValue("Mythril"))
                return;

            fargoPlayer.MythrilEnchantItem = item;

            const int cooldown = 60 * 5;
            int mythrilEndTime = fargoPlayer.MythrilMaxTime - cooldown;

            if (fargoPlayer.WeaponUseTimer > mythrilEndTime)
                fargoPlayer.MythrilTimer--;
            else
                fargoPlayer.MythrilTimer++;

            if (fargoPlayer.MythrilTimer > fargoPlayer.MythrilMaxTime)
                fargoPlayer.MythrilTimer = fargoPlayer.MythrilMaxTime;
            if (fargoPlayer.MythrilTimer < mythrilEndTime)
                fargoPlayer.MythrilTimer = mythrilEndTime;
        }

        public static void CalcMythrilAttackSpeed(FargoSoulsPlayer modPlayer, Item item)
        {
            if (item.DamageType != DamageClass.Default && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                float ratio = Math.Max((float)modPlayer.MythrilTimer / modPlayer.MythrilMaxTime, 0);
                modPlayer.AttackSpeed += modPlayer.MythrilMaxSpeedBonus * ratio;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyMythrilHead")
            .AddIngredient(ItemID.MythrilChainmail)
            .AddIngredient(ItemID.MythrilGreaves)
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.Gatligator)
            .AddIngredient(ItemID.OnyxBlaster)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
}
