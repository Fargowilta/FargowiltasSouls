using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Expert
{
    public class RustedOxygenTank : SoulsItem
    {
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 4);

            Item.expert = true;
        }

        public static void PassiveEffect(Player player)
        {
            player.ignoreWater = true;

            if (Collision.WetCollision(player.position, player.width, player.height))
            {
                player.moveSpeed += 1.25f;
                player.maxRunSpeed += 1.25f;
                return;
            }

            //ripped from "effects while in water" vanilla code
            if (player.merman)
            {
                player.gravity = 0.3f;
                player.maxFallSpeed = 7f;
            }
            else if (player.trident)
            {
                player.gravity = 0.25f;
                player.maxFallSpeed = 6f;
                Player.jumpHeight = 25;
                Player.jumpSpeed = 5.51f;
                if (player.controlUp)
                {
                    player.gravity = 0.1f;
                    player.maxFallSpeed = 2f;
                }
            }
            else
            {
                player.gravity = 0.2f;
                player.maxFallSpeed = 5f;
                Player.jumpHeight = 30;
                Player.jumpSpeed = 6.01f;
            }

            if (!player.wet)
            {
                player.wet = true;
                player.wetCount = 10;
            }
            //player.wingTime = player.wingTimeMax;

            /*
            if (player.controlJump && player.GetJumpState(ExtraJump.Flipper).Available) //simulate flipper jump
            {
                player.swimTime = 30;
                player.velocity.Y = (0f - Player.jumpSpeed) * player.gravDir;
                player.jump = Player.jumpHeight;
            }
            */
            //player.position -= player.velocity * 0.5f; //water speed is half of normal speed


        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<RustedOxygenTankInactive>());
        }
        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().OxygenTank = true;
        }
    }
    public class RustedOxygenTankInactive : SoulsItem
    {
        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Expert;
            Item.value = Item.sellPrice(0, 4);

            Item.expert = true;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.ReplaceItem(Item, ModContent.ItemType<RustedOxygenTank>());
        }
    }
}