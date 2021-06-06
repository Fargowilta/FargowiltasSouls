using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class DubiousCircuitry : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dubious Circuitry");
            Tooltip.SetDefault(@"Grants immunity to Cursed Inferno, Ichor, Lightning Rod, Defenseless, Nano Injection, and knockback
Your attacks inflict Cursed Inferno and Ichor
Your attacks have a small chance to inflict Lightning Rod
Two friendly probes fight by your side
Reduces damage taken by 5%
'Malware probably not included'");
            DisplayName.AddTranslation(GameCulture.Chinese, "可疑电路");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫诅咒狱火、灵液、避雷针、毫无防御, 纳米注射减益和击退
攻击会造成诅咒狱火和灵液减益
攻击有几率造成避雷针减益
召唤两个探针至你身边为你而战
减少5%所受伤害
'里面也许没有恶意软件'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Lime;
            item.value = Item.sellPrice(0, 5);
            item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Defenseless>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.NanoInjection>()] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.LightningRod>()] = true;

            player.GetModPlayer<FargoPlayer>().FusedLens = true;
            player.GetModPlayer<FargoPlayer>().GroundStick = true;
            if (player.GetToggleValue("MasoProbe"))
                player.AddBuff(ModContent.BuffType<Buffs.Minions.Probes>(), 2);

            player.endurance += 0.05f;
            player.noKnockback = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("FusedLens"));
            recipe.AddIngredient(mod.ItemType("GroundStick"));
            recipe.AddIngredient(mod.ItemType("ReinforcedPlating"));
            recipe.AddIngredient(ItemID.HallowedBar, 10);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
