using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SupremeDeathbringerFairy : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Supreme Deathbringer Fairy");
            /* Tooltip.SetDefault(@"Grants immunity to Slimed, Berserked, Lethargic, and Infested
Honey buff increases your armor penetration by 10
Increased fall speed
When you land after a jump, slime will fall from the sky over your cursor
While dashing or running quickly you will create a trail of blood scythes
Press the Debuff Install key while holding UP and DOWN to go berserk
Press the Special Dash key to perform a short quick bee dash
Increases speed when dashing by 50%
When dashing, take 75% less contact damage and reflect 100% of contact damage on attacker
Bees and weak Hornets become friendlier
Summons 2 Skeletron arms to whack enemies
'Supremacy not necessarily guaranteed'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "至高告死精灵");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'霸权不一定能得到保证'
            // 免疫黏糊, 狂暴, 昏昏欲睡和感染
            // 增加10%伤害, 增加10点护甲穿透
            // 增加15%掉落速度
            // 跳跃落地后, 在光标处落下史莱姆
            // 冲刺或快速奔跑时发射一串血镰
            // 攻击造成毒液效果
            // 蜜蜂和虚弱黄蜂变得友好
            // 永久蜂蜜Buff效果
            // 召唤2个骷髅王手臂重击敌人");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line = new(Mod, "tooltip", Language.GetTextValue($"Mods.{Mod.Name}.Items.{Name}.ExtTooltip"));
                tooltips.Add(line);
            }

            else
            {
                TooltipLine line = new(Mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Items.Extra.PressShift"));
                tooltips.Add(line);
            }

        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 4);
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            fargoPlayer.SupremeDeathbringerFairy = true;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            player.AddEffect<SlimeFallEffect>(Item);

            if (player.AddEffect<SlimyShieldEffect>(Item))
            {
                player.FargoSouls().SlimyShieldItem = Item;
            }

            //agitating lens
            player.buffImmune[ModContent.BuffType<BerserkedBuff>()] = true;
            //player.GetDamage(DamageClass.Generic) += 0.1f;
            player.AddEffect<AgitatingLensEffect>(Item);
            player.AddEffect<AgitatingLensInstall>(Item);

            //queen stinger
            player.buffImmune[ModContent.BuffType<InfestedBuff>()] = true;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStingerItem = Item;
            if (player.honey)
                player.GetArmorPenetration(DamageClass.Generic) += 5;

            //necromantic brew
            player.buffImmune[ModContent.BuffType<LethargicBuff>()] = true;
            fargoPlayer.NecromanticBrewItem = Item;
            player.AddEffect<NecroBrewSpin>(Item);
            player.AddEffect<SkeleMinionEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<SlimyShield>())
            .AddIngredient(ModContent.ItemType<AgitatingLens>())
            .AddIngredient(ModContent.ItemType<QueenStinger>())
            .AddIngredient(ModContent.ItemType<NecromanticBrew>())
            .AddIngredient(ItemID.HellstoneBar, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)

            .AddTile(TileID.DemonAltar)

            .Register();
        }
    }
}