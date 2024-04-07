using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Neck)]
    public class SnipersSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Sniper's Soul");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神枪手之魂");

            // Tooltip.SetDefault(tooltip);

            //string tooltip_ch =
            //@"增加30%远程伤害
            //20%几率不消耗弹药
            //增加15%远程暴击率
            //拥有狙击镜效果
            //'预备，瞄准，开火'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }


        public static readonly Color ItemColor = new(188, 253, 68);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //reduce ammo consume
            player.FargoSouls().RangedSoul = true;
            player.GetDamage(DamageClass.Ranged) += 0.3f;
            player.GetCritChance(DamageClass.Ranged) += 15;

            //add new effects

            player.AddEffect<SniperScopeEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            /*
hive pack*/
            .AddIngredient(null, "SharpshootersEssence")
            .AddIngredient(ItemID.MoltenQuiver)
            .AddIngredient(ItemID.StalkersQuiver)
            .AddIngredient(ItemID.ReconScope)

            .AddIngredient(ItemID.DartPistol)
            .AddIngredient(ItemID.Megashark)
            .AddIngredient(ItemID.PulseBow)
            .AddIngredient(ItemID.NailGun)
            .AddIngredient(ItemID.PiranhaGun)
            .AddIngredient(ItemID.SniperRifle)
            .AddIngredient(ItemID.Tsunami)
            .AddIngredient(ItemID.StakeLauncher)
            .AddIngredient(ItemID.ElfMelter)
            .AddIngredient(ItemID.Xenopopper)
            .AddIngredient(ItemID.Celeb2)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();

        }
    }
    public class SniperScopeEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ItemID.SniperScope;
        public override void PostUpdateEquips(Player player)
        {
            player.scope = true;
        }
    }
}
