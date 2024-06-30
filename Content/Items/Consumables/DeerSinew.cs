using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Consumables
{
    public class DeerSinew : SoulsItem
    {

        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Deer Sinew");
            /* Tooltip.SetDefault(
@"Allows the ability to dash
Double tap a direction
Taking damage may briefly freeze you
Reduces damage when your speed increases, down to 0.85x at max
All effects negated if toggled off or another dash is already in use
'Cold but twitching'"); */
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;
            Item.value = Item.sellPrice(0, 3);
        }

        public override bool CanUseItem(Player player)
        {
            return !player.FargoSouls().DeerSinew;
        }

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.FargoSouls().DeerSinew = true;
            }
            return true;
        }
    }
    public class DeerSinewEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<DeerSinew>();
        
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.DeerSinewFreezeCD > 0)
                modPlayer.DeerSinewFreezeCD--;
        }
        public static void AddDash(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.HasDash || player.mount.Active)
                return;

            modPlayer.HasDash = true;
            modPlayer.FargoDash = DashManager.DashType.DeerSinew;
            modPlayer.DeerSinewNerf = true;

            if (modPlayer.IsDashingTimer > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.GemSapphire, Scale: 1.25f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.2f;
                }
            }
        }
    }
}
