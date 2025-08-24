using FargowiltasSouls.Assets.Textures;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class TinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(162, 139, 78);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TinEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TinHelmet)
                .AddIngredient(ItemID.TinChainmail)
                .AddIngredient(ItemID.TinGreaves)
                .AddIngredient(ItemID.TinBow)
                .AddIngredient(ItemID.TopazStaff)
                .AddIngredient(ItemID.PainterPaintballGun)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
    public class TinEffect : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<TerraHeader>();
        public override int ToggleItemType => ModContent.ItemType<TinEnchant>();

        public static int TinFloor(Player player)
        {
            if (player.FargoSouls().Eternity)
                return 50;
            if (player.HasEffect<TerraLightningEffect>())
                return 5;
            //if (player.FargoSouls().ForceEffect<TinEnchant>())
            //    return 6;
            return 3;
        }

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.Eternity)
            {
                if (modPlayer.TinEternityDamage > 47.5f)
                    modPlayer.TinEternityDamage = 47.5f;

                if (player.HasEffect<EternityTin>())
                {
                    player.GetDamage(DamageClass.Generic) += modPlayer.TinEternityDamage;
                    player.statDefense += (int)(modPlayer.TinEternityDamage * 10); //1 defense per .1 damage
                }
            }

            if (modPlayer.TinProcCD > 0)
                modPlayer.TinProcCD--;

            int floor = TinFloor(player);
            if (modPlayer.TinCrit < floor)
                modPlayer.TinCrit = floor;

            if (Main.myPlayer == player.whoAmI)
                CooldownBarManager.Activate("TinCritCharge", FargoAssets.GetTexture2D("Content/Items/Accessories/Enchantments", "TinEnchant").Value, new(162, 139, 78), 
                    () => (float)Main.LocalPlayer.FargoSouls().TinCrit / Main.LocalPlayer.FargoSouls().TinCritMax, true, activeFunction: () => player.HasEffect<TinEffect>());
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            TinOnHitEnemy(player, hitInfo);
        }
        // increase crit
        public static void TinOnHitEnemy(Player player, NPC.HitInfo hitInfo)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (hitInfo.Crit)
                modPlayer.TinCritBuffered = true;

            if (modPlayer.TinCritBuffered && modPlayer.TinProcCD <= 0)
            {
                modPlayer.TinCritBuffered = false;
                if (player.HasEffectEnchant<TinEffect>() || modPlayer.Eternity)
                {
                    int gain = modPlayer.Eternity ? 10 : 3;
                    modPlayer.TinCrit += gain;
                    if (modPlayer.TinCrit > modPlayer.TinCritMax)
                        modPlayer.TinCrit = modPlayer.TinCritMax;
                    else
                        CombatText.NewText(modPlayer.Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.TinEnchant.CritUp", gain));
                }
                else
                    return;
                /*
                void TryHeal(int healDenominator, int healCooldown)
                {
                    int amountToHeal = hitInfo.Damage / healDenominator;
                    if (modPlayer.TinCrit >= 100 && modPlayer.HealTimer <= 0 && !modPlayer.Player.moonLeech && !modPlayer.MutantNibble && amountToHeal > 0)
                    {
                        modPlayer.HealTimer = healCooldown;
                        modPlayer.Player.statLife = Math.Min(modPlayer.Player.statLife + amountToHeal, modPlayer.Player.statLifeMax2);
                        modPlayer.Player.HealEffect(amountToHeal);
                    }
                }
                */

                if (modPlayer.Eternity)
                {
                    modPlayer.TinProcCD = 1;
                    //TryHeal(10, 1);
                    modPlayer.TinEternityDamage += .05f;
                }
                //else if (modPlayer.TerrariaSoul)
                //{
                //    modPlayer.TinProcCD = 15;
                    //TryHeal(25, 10);
                //}
                else if (modPlayer.ForceEffect<TinEnchant>())
                {
                    modPlayer.TinProcCD = 30;
                }
                else
                {
                    modPlayer.TinProcCD = 90;
                }
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            if (info.Damage < 5)
                return;
            TinHurt(player);
        }
        //reset crit
        public static void TinHurt(Player player, bool forceReset = false)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float oldCrit = modPlayer.TinCrit;
            if (modPlayer.Eternity)
                modPlayer.TinEternityDamage = 0;

            if (forceReset)
                modPlayer.TinCrit = TinFloor(player);
            else
                modPlayer.TinCrit = Math.Max(modPlayer.TinCrit / 2, TinFloor(player));

            double diff = Math.Round(oldCrit - modPlayer.TinCrit, 1);
            if (diff > 0)
                CombatText.NewText(modPlayer.Player.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasSouls.Items.TinEnchant.CritReset", diff), true);
        }
        public override void PostUpdateMiscEffects(Player player)
        {
            TinPostUpdate(player);
        }
        //set max crit and current crit with no interference from accessory order
        public static void TinPostUpdate(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.Eternity)
            {
                modPlayer.TinCritMax = 100;
                FargoSoulsUtil.AllCritEquals(modPlayer.Player, modPlayer.TinCrit);
            }
            else
            {
                modPlayer.TinCritMax = player.HasEffect<TerraLightningEffect>() ? 25 : modPlayer.ForceEffect<TinEnchant>() ? 24 : 12;
                player.GetCritChance(DamageClass.Generic) += modPlayer.TinCrit;
            }
        }
    }
}
