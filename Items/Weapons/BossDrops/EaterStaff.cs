using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class EaterStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Eater of Worlds Staff");
            Tooltip.SetDefault(
                @"Summons 4 segments for each minion slot
'An old foe beaten into submission..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "世界吞噬者法杖");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese,
@"一个被迫屈服的老对手..
每个召唤栏召唤4段身体");
        }

        public override void SetDefaults()
        {
            Item.mana = 10;
            Item.damage = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<EaterHead>();
            Item.width = 26;
            Item.height = 28;
            Item.UseSound = SoundID.Item44;
            Item.useAnimation = 36;
            Item.useTime = 36;
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.buffType = ModContent.BuffType<EaterMinion>();
            Item.buffTime = 3600;
            Item.DamageType = DamageClass.Summon;
            Item.value = 40000;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //to fix tail disapearing meme
            float slotsUsed = 0;

            Main.projectile.Where(x => x.active && x.owner == player.whoAmI && x.minionSlots > 0).ToList().ForEach(x => { slotsUsed += x.minionSlots; });

            if (player.maxMinions - slotsUsed < 1) return false;

            int headCheck = -1;
            int tailCheck = -1;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI)
                {
                    if (headCheck == -1 && proj.type == ModContent.ProjectileType<EaterHead>()) headCheck = i;
                    if (tailCheck == -1 && proj.type == ModContent.ProjectileType<EaterTail>()) tailCheck = i;
                    if (headCheck != -1 && tailCheck != -1) break;
                }
            }

            //initial spawn
            if (headCheck == -1 && tailCheck == -1)
            {
                int current = FargoSoulsUtil.NewSummonProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<EaterHead>(), Item.damage, knockback, player.whoAmI, 0f, 0);

                int previous = 0;

                for (int i = 0; i < 4; i++)
                {
                    current = FargoSoulsUtil.NewSummonProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<EaterBody>(), Item.damage, knockback, player.whoAmI, Main.projectile[current].identity, 0);
                    previous = current;
                }

                current = FargoSoulsUtil.NewSummonProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<EaterTail>(), Item.damage, knockback, player.whoAmI, Main.projectile[current].identity, 0);

                Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                Main.projectile[previous].netUpdate = true;
            }
            //spawn more body segments
            else
            {
                int previous = (int)Main.projectile[tailCheck].ai[0];
                int current = 0;

                for (int i = 0; i < 4; i++)
                {
                    int prevUUID = FargoSoulsUtil.GetProjectileByIdentity(player.whoAmI, Main.projectile[previous].identity);
                    current = FargoSoulsUtil.NewSummonProjectile(source, position, velocity, ModContent.ProjectileType<EaterBody>(),
                        Item.damage, knockback, player.whoAmI, prevUUID, 0);

                    previous = current;
                }

                Main.projectile[current].localAI[1] = Main.projectile[tailCheck].identity;

                Main.projectile[tailCheck].ai[0] = FargoSoulsUtil.GetProjectileByIdentity(player.whoAmI, Main.projectile[current].identity);
                Main.projectile[tailCheck].netUpdate = true;
                Main.projectile[tailCheck].ai[1] = 1f;
            }

            return false;
        }
    }
}