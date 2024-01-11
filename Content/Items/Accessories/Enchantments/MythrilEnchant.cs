using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class MythrilEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Mythril Enchantment");
            /* Tooltip.SetDefault(
@"Temporarily increases attack speed after not attacking for a while
Bonus ends after attacking for 3 seconds and rebuilds over 5 seconds
'You feel the knowledge of your weapons seep into your mind'"); */
        }

        protected override Color nameColor => new(157, 210, 144);
        

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MythrilEffect>();
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("FargowiltasSouls:AnyMythrilHead")
            .AddIngredient(ItemID.MythrilChainmail)
            .AddIngredient(ItemID.MythrilGreaves)
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.Gatligator)
            .AddIngredient(ItemID.OnyxBlaster)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }

    public class MythrilEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override bool HasToggle => true;

        public static void CalcMythrilAttackSpeed(FargoSoulsPlayer modPlayer, Item item)
        {
            MythrilFields fields = modPlayer.Player.GetEffectFields<MythrilFields>();

            if (item.DamageType != DamageClass.Default && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.type != ModContent.ItemType<PrismaRegalia>())
            {
                float ratio = Math.Max((float)fields.MythrilTimer / fields.MythrilMaxTime, 0);
                modPlayer.AttackSpeed += fields.MythrilMaxSpeedBonus * ratio;
            }
        }

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            MythrilFields fields = modPlayer.Player.GetEffectFields<MythrilFields>();

            const int cooldown = 60 * 5;
            int mythrilEndTime = fields.MythrilMaxTime - cooldown;

            if (modPlayer.WeaponUseTimer > 0)
                fields.MythrilTimer--;
            else
            {
                fields.MythrilTimer++;
                if (fields.MythrilTimer == fields.MythrilMaxTime - 1 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(FargowiltasSouls)}/Assets/Sounds/ChargeSound"), player.Center);
                }
            }

            if (fields.MythrilTimer > fields.MythrilMaxTime)
                fields.MythrilTimer = fields.MythrilMaxTime;
            if (fields.MythrilTimer < mythrilEndTime)
                fields.MythrilTimer = mythrilEndTime;
        }
    }

    public class MythrilFields : EffectFields
    {
        public override void UpdateDead()
        {
            MythrilTimer = MythrilMaxTime;
        }

        public int MythrilTimer;
        public int MythrilMaxTime => Player.HasEffect<MythrilEffect>() ? Player.ForceEffect<MythrilEffect>() ? 300 : 180 : 180;
        public float MythrilMaxSpeedBonus => Player.HasEffect<MythrilEffect>() ? Player.ForceEffect<MythrilEffect>() ? 1.75f : 1.5f : 1.5f;
    }
}
