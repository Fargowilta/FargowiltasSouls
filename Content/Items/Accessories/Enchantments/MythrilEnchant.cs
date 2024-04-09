using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MythrilEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Mythril Enchantment");
            /* Tooltip.SetDefault(
@"Temporarily increases attack speed after not attacking for a while
Bonus ends after attacking for 3 seconds and rebuilds over 5 seconds
'You feel the knowledge of your weapons seep into your mind'"); */
        }

        public override Color nameColor => new(157, 210, 144);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MythrilEffect>(Item);
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

    public class MythrilEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<MythrilEnchant>();

        public static void CalcMythrilAttackSpeed(FargoSoulsPlayer modPlayer, Item item)
        {


            if (item.DamageType != DamageClass.Default && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.type != ModContent.ItemType<PrismaRegalia>())
            {
                float ratio = Math.Max((float)modPlayer.MythrilTimer / modPlayer.MythrilMaxTime, 0);
                modPlayer.AttackSpeed += modPlayer.MythrilMaxSpeedBonus * ratio;
            }
        }

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            const int cooldown = 60 * 5;
            int mythrilEndTime = modPlayer.MythrilMaxTime - cooldown;

            if (modPlayer.WeaponUseTimer > 0)
                modPlayer.MythrilTimer--;
            else
            {
                modPlayer.MythrilTimer++;
                if (modPlayer.MythrilTimer == modPlayer.MythrilMaxTime - 1 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(FargowiltasSouls)}/Assets/Sounds/ChargeSound"), player.Center);
                }
            }

            if (modPlayer.MythrilTimer > modPlayer.MythrilMaxTime)
                modPlayer.MythrilTimer = modPlayer.MythrilMaxTime;
            if (modPlayer.MythrilTimer < mythrilEndTime)
                modPlayer.MythrilTimer = mythrilEndTime;
        }
    }

}
