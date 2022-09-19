using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class DreadShell : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dread Shell");
            Tooltip.SetDefault(@"Grants immunity to Anticoagulation
Grants immunity to knockback
Right Click to guard with your shield
Defense and damage reduction drastically decreased while and shortly after guarding
Guard exactly as an attack touches you to counter it on a very long cooldown
Counterattack deals massive damage and inflicts Anticoagulation
Absorb Anticoagulation blood clots to gain 30% increased damage
'It was a mistake to chum here'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "恐惧螺壳");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"免疫凝血失效和击退
            // 右键点击来用护盾防护
            // 防护时和防护后短时间内，防御力和伤害减免大幅下降
            // 如果你在防护时正好受到攻击，你会进行反击，反击有很长的冷却时间
            // 反击会造成巨量伤害，并造成凝血失效减益
            // “在这钓鱼是个错误”");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Anticoagulation>()] = true;

            player.noKnockback = true;

            if (player.GetToggleValue("DreadShellParry"))
                player.GetModPlayer<FargoSoulsPlayer>().DreadShellItem = Item;
        }
    }
}