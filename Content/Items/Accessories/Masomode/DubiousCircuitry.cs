﻿using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class DubiousCircuitry : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dubious Circuitry");
            /* Tooltip.SetDefault(@"Grants immunity to Cursed Inferno, Ichor, Lightning Rod, Defenseless, Nano Injection, and knockback
When inflicted with Cursed Inferno, 15% increased attack speed and you inflict Cursed Inferno
When inflicted with Ichor, 15% increased critical strike chance and you inflict Ichor
Press the Debuff Install key to inflict yourself with Cursed Inferno and Ichor for 30 seconds
Your attacks have a small chance to inflict Lightning Rod
Electric and ray attacks supercharge you and do halved damage if not already supercharged
While supercharged, you have increased movement speed, attack speed, and inflict Electrified
Two friendly probes fight by your side and will supercharge with you
Reduces damage taken by 5%
'Malware probably not included'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "可疑电路");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'里面也许没有恶意软件'
            // 免疫诅咒地狱,脓液,避雷针,毫无防御,昏迷和击退
            // 攻击造成诅咒地狱和脓液效果
            // 攻击小概率造成避雷针效果
            // 召唤2个友善的探测器为你而战
            // 减少6%所受伤害");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 5);
            Item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.DefenselessBuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.NanoInjectionBuff>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.LightningRodBuff>()] = true;

            player.FargoSouls().FusedLens = true;
            player.AddEffect<FusedLensInstall>(Item);
            if (player.onFire2)
                player.FargoSouls().AttackSpeed += 0.15f;
            if (player.ichor)
                player.GetCritChance(DamageClass.Generic) += 15;

            player.AddEffect<ProbeMinionEffect>(Item);
            player.AddEffect<GroundStickDR>(Item);

            player.endurance += 0.05f;
            player.noKnockback = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<FusedLens>())
            .AddIngredient(ModContent.ItemType<GroundStick>())
            .AddIngredient(ModContent.ItemType<ReinforcedPlating>())
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}