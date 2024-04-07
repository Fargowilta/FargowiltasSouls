using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class PrecisionSeal : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Precision Seal");
            /* Tooltip.SetDefault(@"Grants immunity to Smite
Reduces your hurtbox size for projectiles
Hold the Precision Seal key to disable dashes and double jumps
Your hurtbox size is reduced even when not shown
'Dodge so close you can almost taste it'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "玲珑圣印");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"使你免疫惩戒减益
            // 缩小你对于敌对弹幕的碰撞判定
            // 按下“玲珑圣印”键后开启精确模式，禁用冲刺和二段跳
            // “躲得那么近，你都能舔到那个弹幕了”
            // （此翻译是在缺失贴图的情况下给出的，更新贴图后请及时告诉棱镜以进行修改）");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 8);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            player.buffImmune[ModContent.BuffType<SmiteBuff>()] = true;
            modPlayer.PrecisionSeal = true;
            player.AddEffect<PrecisionSealHurtbox>(Item);
        }
    }
    public class PrecisionSealHurtbox : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<HeartHeader>();
        public override int ToggleItemType => ModContent.ItemType<PrecisionSeal>();
        public override bool IgnoresMutantPresence => true;
    }
}
