using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class PuffInABottle : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Puff in a Bottle");
            // Tooltip.SetDefault(@"Allows the holder to double jump");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.CloudinaBottle);
            Item.value = Item.sellPrice(0, 0, 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetJumpState<PuffJump>().Enable();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.Cloud)
                .Register();
        }
    }
    public class PuffJump : ExtraJump
    {
        public override Position GetDefaultPosition() => new After(BlizzardInABottle);

        public override IEnumerable<Position> GetModdedConstraints() //hover for explanation, leaving here if we want it later
        {
            return null;
        }

        public override float GetDurationMultiplier(Player player)
        {
            // Use this hook to set the duration of the extra jump
            // The XML summary for this hook mentions the values used by the vanilla extra jumps
            return 0.375f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            // Use this hook to modify "player.runAcceleration" and "player.maxRunSpeed"
            // The XML summary for this hook mentions the values used by the vanilla extra jumps
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            int offsetY = player.height;
            if (player.gravDir == -1f)
                offsetY = 0;

            offsetY -= 16;

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position + new Vector2(-34f, offsetY), 102, 32, DustID.Cloud, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, Color.Gray, 1.5f);
                dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
            }

            SpawnCloudPoof(player, player.Top + new Vector2(-16f, offsetY));
            //SpawnCloudPoof(player, player.position + new Vector2(-36f, offsetY));
            //SpawnCloudPoof(player, player.TopRight + new Vector2(4f, offsetY));
        }

        private static void SpawnCloudPoof(Player player, Vector2 position)
        {
            Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), position, -player.velocity, Main.rand.Next(11, 14), Scale: 1.25f);
            gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
            gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
        }

        public override void ShowVisuals(Player player) //once again leaving this if we want it later
        {
            // Use this hook to trigger effects that should appear throughout the duration of the extra jump
        }
    }
}