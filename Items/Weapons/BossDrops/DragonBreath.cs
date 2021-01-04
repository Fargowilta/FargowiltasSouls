using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class DragonBreath : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath");
            Tooltip.SetDefault("Uses gel for ammo\n33% chance to not consume ammo\n'The shrunken body of a defeated foe..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "巨龙之息");
            Tooltip.AddTranslation(GameCulture.Chinese, "使用凝胶作为弹药\n33%的几率不消耗弹药\n'一个被打败的敌人的缩小版尸体..'");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.ranged = true;
            //item.mana = 10;
            item.width = 24;
            item.height = 24;
            item.useTime = 45;
            item.useAnimation = 45;
            item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.DD2_BetsyFlameBreath;
            item.useAmmo = AmmoID.Gel;
            //Item.staff[item.type] = true;
            item.value = 50000;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<DragonBreathProj>();
            item.shootSpeed = 35f;
            item.noUseGraphic = false;
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, -6);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[item.shoot] > 0)
                return false;

            return true;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.Next(3) != 0;
        }
    }
}
