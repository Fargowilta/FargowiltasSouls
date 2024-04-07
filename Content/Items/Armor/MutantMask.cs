using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class MutantMask : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("True Mutant Head");
            /* Tooltip.SetDefault(@"50% increased damage and 20% increased critical strike chance
Increases max number of minions and sentries by 10
25% reduced mana usage
25% chance not to consume ammo"); */

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 50);
            Item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.50f;
            player.GetCritChance(DamageClass.Generic) += 20;

            player.maxMinions += 10;
            player.maxTurrets += 10;

            player.manaCost -= 0.25f;
            player.ammoCost75 = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MutantBody>() && legs.type == ModContent.ItemType<MutantPants>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = GetSetBonusString();
            MutantSetBonus(player, Item);
        }

        public static string GetSetBonusString()
        {
            return Language.GetTextValue($"Mods.FargowiltasSouls.SetBonus.Mutant");
        }

        public static void MutantSetBonus(Player player, Item item)
        {
            player.AddEffect<MasoAbom>(item);
            player.AddEffect<MasoRing>(item);
            player.AddBuff(ModContent.BuffType<MutantPowerBuff>(), 2);

            player.FargoSouls().MutantSetBonusItem = item;
            player.FargoSouls().GodEaterImbue = true;
            player.FargoSouls().AttackSpeed += .2f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Fargowiltas.Items.Vanity.MutantMask>()
            .AddIngredient<AbomEnergy>(10)
            .AddIngredient<EternalEnergy>(10)
            .AddTile<CrucibleCosmosSheet>()

            .Register();
        }
    }
    public class MasoAbom : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MutantArmorHeader>();
        public override int ToggleItemType => ModContent.ItemType<MutantMask>();
        //public override bool MinionEffect => true; no, abom is stronger than minos
        public override void PostUpdateEquips(Player player)
        {
            player.FargoSouls().AbomMinion = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AbomMinion>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<AbomMinion>(), 900, 10f, player.whoAmI, -1);
        }
    }
    public class MasoRing : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MutantArmorHeader>();
        public override int ToggleItemType => ModContent.ItemType<MutantMask>();
        public override void PostUpdateEquips(Player player)
        {
            player.FargoSouls().PhantasmalRing = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PhantasmalRing>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Misc(""), player.Center, Vector2.Zero, ModContent.ProjectileType<PhantasmalRing>(), 1700, 0f, player.whoAmI);
        }

    }
}