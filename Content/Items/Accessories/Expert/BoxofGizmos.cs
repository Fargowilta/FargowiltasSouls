using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class BoxofGizmos : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Box of Gizmos");
            /* Tooltip.SetDefault(@"Works in your inventory
Stand still to expose nearby treasure
Grants autofire to all items
Slightly reduces use speed of affected items"); */

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 1);

            Item.expert = true;
        }

        int counter;

        void PassiveEffect(Player player)
        {
            player.FargoSouls().BoxofGizmos = true;

            if (player.whoAmI == Main.myPlayer && player.FargoSouls().IsStandingStill && player.itemAnimation == 0)
            {
                if (++counter > 60)
                {
                    player.detectCreature = true;

                    if (counter % 10 == 0)
                        Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(player.Center);
                }
            }
            else
            {
                counter = 0;
            }
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual) => PassiveEffect(player);
    }
}