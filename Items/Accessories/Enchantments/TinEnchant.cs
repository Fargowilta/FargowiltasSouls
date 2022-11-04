using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class TinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Tin Enchantment");

            string tooltip =
@"Sets your critical strike chance to 5%
Every crit will increase it by 5% up to double your critical strike chance or 15%
Getting hit resets your crit to 5%
'Return of the Crit'";
            Tooltip.SetDefault(tooltip);
        }

        protected override Color nameColor => new Color(162, 139, 78);
        public override string wizardEffect => "Minimum crit increased to 10%, max crit is at least 50%, reduced proc cooldown";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TinEffect(player, Item);
        }

        public static void TinEffect(Player player, Item item)
        {
            if (!player.GetToggleValue("Tin", false)) return;

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.TinEnchantItem = item;

            if (modPlayer.Eternity)
            {
                if (modPlayer.TinEternityDamage > 47.5f)
                    modPlayer.TinEternityDamage = 47.5f;

                if (player.GetToggleValue("Eternity", false))
                {
                    player.GetDamage(DamageClass.Generic) += modPlayer.TinEternityDamage;
                    player.statDefense += (int)(modPlayer.TinEternityDamage * 100); //10 defense per .1 damage
                }
            }

            if (modPlayer.TinProcCD > 0)
                modPlayer.TinProcCD--;
        }

        //set max crit and current crit with no interference from accessory order
        public static void TinPostUpdate(FargoSoulsPlayer modPlayer)
        {
            modPlayer.TinCritMax = Math.Max(FargoSoulsUtil.HighestCritChance(modPlayer.Player) * 2, modPlayer.TerraForce ?50 : 15);

            if (modPlayer.TinCritMax > 100)
                modPlayer.TinCritMax = 100;

            FargoSoulsUtil.AllCritEquals(modPlayer.Player, modPlayer.TinCrit);
        }

        //increase crit
        public static void TinOnHitEnemy(FargoSoulsPlayer modPlayer, int damage, bool crit)
        {
            if (crit)
                modPlayer.TinCritBuffered = true;

            if (modPlayer.TinCritBuffered && modPlayer.TinProcCD <= 0)
            {
                modPlayer.TinCritBuffered = false;
                modPlayer.TinCrit += 5;
                if (modPlayer.TinCrit > modPlayer.TinCritMax)
                    modPlayer.TinCrit = modPlayer.TinCritMax;
                else
                    CombatText.NewText(modPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.TinCritUp"));


                void TryHeal(int healDenominator, int healCooldown)
                {
                    int amountToHeal = damage / healDenominator;
                    if (modPlayer.TinCrit >= 100 && modPlayer.HealTimer <= 0 && !modPlayer.Player.moonLeech && !modPlayer.MutantNibble && amountToHeal > 0)
                    {
                        modPlayer.HealTimer = healCooldown;
                        modPlayer.Player.statLife = Math.Min(modPlayer.Player.statLife + amountToHeal, modPlayer.Player.statLifeMax2);
                        modPlayer.Player.HealEffect(amountToHeal);
                    }
                }

                if (modPlayer.Eternity)
                {
                    modPlayer.TinProcCD = 1;
                    TryHeal(10, 1);
                    modPlayer.TinEternityDamage += .05f;
                }
                else if (modPlayer.TerrariaSoul)
                {
                    modPlayer.TinProcCD = 15;
                    TryHeal(25, 10);
                }
                else if (modPlayer.TerraForce)
                {
                    modPlayer.TinProcCD = 30;
                }
                else
                {
                    modPlayer.TinProcCD = 60;
                }
            }
        }

        //reset crit
        public static void TinHurt(FargoSoulsPlayer modPlayer)
        {
            float oldCrit = modPlayer.TinCrit;
            if (modPlayer.Eternity)
            {
                modPlayer.TinCrit = 50;
                modPlayer.TinEternityDamage = 0;
            }
            else if (modPlayer.TerrariaSoul)
            {
                modPlayer.TinCrit = 20;
            }
            else if (modPlayer.TerraForce)
            {
                modPlayer.TinCrit = 10;
            }
            else
            {
                modPlayer.TinCrit = 5;
            }

            double diff = Math.Round(oldCrit - modPlayer.TinCrit, 1);
            if (diff > 0)
                CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.TinCritReset", diff), true);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinHelmet)
                .AddIngredient(ItemID.TinChainmail)
                .AddIngredient(ItemID.TinGreaves)
                .AddIngredient(ItemID.TinBow)
                .AddIngredient(ItemID.Revolver)
                .AddIngredient(ItemID.TopazStaff)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
