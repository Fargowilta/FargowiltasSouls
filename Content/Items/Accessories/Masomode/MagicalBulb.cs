using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class MagicalBulb : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magical Bulb");
            /* Tooltip.SetDefault(@"Grants immunity to Acid Venom, Ivy Venom, and Swarming
Press the Magical Cleanse key to cure yourself of most debuffs on a cooldown
Cleanse cooldown is faster when not attacking
Increases life regeneration based on how much light you receive
Attracts a legendary plant's offspring which flourishes in combat
'Matricide?'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "魔法球茎");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'杀妈?'
            // 免疫毒液, 常春藤毒和蜂群
            // 增加生命回复
            // 吸引一株传奇植物的后代, 其会在战斗中茁壮成长");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[ModContent.BuffType<IvyVenomBuff>()] = true;
            player.buffImmune[ModContent.BuffType<SwarmingBuff>()] = true;

            Point pos = player.Center.ToTileCoordinates();
            if (pos.X > 0 && pos.Y > 0 && pos.X < Main.maxTilesX && pos.Y < Main.maxTilesY
                && player.whoAmI == Main.myPlayer) //check for multiplayer hopefully
            {
                float lightStrength = Lighting.GetColor(pos).ToVector3().Length();
                float ratio = lightStrength / 1.732f; //this value is 1,1,1 lighting
                if (ratio < 1)
                    ratio /= 2;
                player.lifeRegen += (int)(6 * ratio);
            }

            player.FargoSouls().MagicalBulb = true;
            player.AddEffect<PlantMinionEffect>(item);

        }
    }
    public class PlantMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaliceHeader>();
        public override int ToggleItemType => ModContent.ItemType<MagicalBulb>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<PlanterasChildBuff>(), 2);
        }
    }
}