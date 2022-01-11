using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class SlimeKingsSlasher : SoulsItem
    {
        private int numSpikes = 3;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Slime King's Slasher");
            //Tooltip.SetDefault("'Torn from the insides of a defeated foe..'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "史莱姆王的屠戮者");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'撕裂敌人内部而得来的..'");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.melee = true;
            item.width = 40;
            item.height = 40;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SlimeSpikeFriendly>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockback)
        {
            int p = Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), type, damage, knockback, player.whoAmI);

            float spread = MathHelper.Pi / 8;

            if (numSpikes == 5)
            {
                spread = MathHelper.Pi / 5;
            }

            FargoGlobalProjectile.SplitProj(Main.projectile[p], numSpikes, spread, 1);

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