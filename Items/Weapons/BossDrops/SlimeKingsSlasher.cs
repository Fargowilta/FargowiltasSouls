using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class SlimeKingsSlasher : SoulsItem
    {
        private int numSpikes = 3;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Slime King's Slasher");
            Tooltip.SetDefault("'Torn from the insides of a defeated foe..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "史莱姆王的屠戮者");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'撕裂敌人内部而得来的..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlimeSpikeFriendly>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), player.Center, velocity, type, damage, knockback, player.whoAmI);

            float spread = MathHelper.Pi / 8;

            if (numSpikes == 5)
            {
                spread = MathHelper.Pi / 5;
            }

            FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSpikes, spread, 1, true);

            numSpikes += 2;

            if (numSpikes > 5)
            {
                numSpikes = 3;
            }

            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }
    }
}