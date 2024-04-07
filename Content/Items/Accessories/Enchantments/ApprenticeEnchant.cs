using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class ApprenticeEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(93, 134, 166);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().ApprenticeEnchantActive = true;
            player.AddEffect<ApprenticeSupport>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ApprenticeHat)
            .AddIngredient(ItemID.ApprenticeRobe)
            .AddIngredient(ItemID.ApprenticeTrousers)
            .AddIngredient(ItemID.DD2FlameburstTowerT2Popper)
            //magic missile
            //ice rod
            //golden shower
            .AddIngredient(ItemID.BookStaff)
            .AddIngredient(ItemID.ClingerStaff)

            .AddTile(TileID.CrystalBall)
            .Register();
        }

        /*
        public static MethodInfo ApprenticeShootMethod
        {
            get;
            set;
        }
        public override void Load()
        {
            ApprenticeShootMethod = typeof(Player).GetMethod("ItemCheck_Shoot", LumUtils.UniversalBindingFlags);
        }
        public static void ApprenticeShoot(Player player, int playerWhoAmI, Item item, int weaponDamage)
        {
            object[] args = new object[] { playerWhoAmI, item, weaponDamage };
            ApprenticeShootMethod.Invoke(player, args);
;
        }
        */
    }
    public class ApprenticeSupport : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ApprenticeEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool forceEffect = modPlayer.ForceEffect<ApprenticeEnchant>();
            //update item cds
            for (int i = 0; i < 10; i++)
            {
                int itemCD = modPlayer.ApprenticeItemCDs[i];

                if (itemCD > 0)
                {
                    itemCD--;
                    modPlayer.ApprenticeItemCDs[i] = itemCD;
                }
            }
            if (player.controlUseItem)
            {
                int numExtraSlotsToUse = 1;
                if (modPlayer.DarkArtistEnchantActive && forceEffect)
                {
                    numExtraSlotsToUse = 3;
                }
                else if (modPlayer.DarkArtistEnchantActive || forceEffect)
                {
                    numExtraSlotsToUse = 2;
                }


                if (player.controlUseItem)
                {
                    Item item = player.HeldItem;

                    //non weapons and weapons with no ammo begone
                    if (item.damage <= 0 || !player.HasAmmo(item) || (item.mana > 0 && player.statMana < item.mana)
                        || item.createTile != -1 || item.createWall != -1 || item.ammo != AmmoID.None || item.hammer != 0 || item.pick != 0 || item.axe != 0) return;

                    int startingSlot = 0;

                    //first we need to find what slot the current weapon is
                    for (int j = 0; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2.type == item.type)
                        {
                            startingSlot = j;
                            break;
                        }
                    }

                    int weaponsUsed = 0;

                    //then go from there and find the next weapon to fire
                    for (int j = startingSlot; j < 10; j++) //hotbar
                    {
                        Item item2 = player.inventory[j];

                        if (item2 != null && item2.damage > 0 && item2.shoot > ProjectileID.None && item2.ammo <= 0 && item.type != item2.type && !item2.channel)
                        {
                            if (!player.HasAmmo(item2) || (item2.mana > 0 && player.statMana < item2.mana) || item2.sentry || ContentSamples.ProjectilesByType[item2.shoot].minion)
                            {
                                continue;
                            }

                            weaponsUsed++;

                            if (weaponsUsed > numExtraSlotsToUse)
                            {
                                break;
                            }

                            int itemCD = modPlayer.ApprenticeItemCDs[j];

                            if (itemCD > 0)
                            {
                                continue;
                            }

                            Vector2 pos = new Vector2(player.Center.X + Main.rand.Next(-50, 50), player.Center.Y + Main.rand.Next(-50, 50));
                            Vector2 velocity = Vector2.Normalize(Main.MouseWorld - pos);

                            //ApprenticeShoot(player, player.whoAmI, item2, item2.damage / 2);
                            int projToShoot = item2.shoot;
                            float speed = item2.shootSpeed;
                            int damage = item2.damage / 2;
                            float KnockBack = item2.knockBack;
                            int usedAmmoItemId;
                            if (item2.useAmmo > 0)
                            {
                                player.PickAmmo(item2, out projToShoot, out speed, out damage, out KnockBack, out usedAmmoItemId, ItemID.Sets.gunProj[item2.type]);
                            }

                            if (item2.mana > 0)
                            {
                                if (player.CheckMana(item2.mana / 2, true, false))
                                {
                                    player.manaRegenDelay = 300;
                                }
                            }
                            if (item2.consumable)
                            {
                                item2.stack--;
                            }
                            modPlayer.ApprenticeItemCDs[j] = item2.useAnimation * 4;

                            if (projToShoot == ProjectileID.RainbowFront || projToShoot == ProjectileID.RainbowBack) // prevent fucked up op interaction
                            {
                                foreach (Projectile rainbow in Main.projectile.Where(p => (p.TypeAlive(ProjectileID.RainbowFront) || p.TypeAlive(ProjectileID.RainbowBack)) && p.owner == player.whoAmI))
                                    rainbow.Kill();
                            }

                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(item), pos, Vector2.Normalize(velocity) * speed, projToShoot, damage, KnockBack, player.whoAmI);
                            Projectile proj = Main.projectile[p];

                            proj.noDropItem = true;


                            /*
                            int shoot = item2.shoot;
                            if (shoot == 10) //purification powder
                            {
                                float speed;
                                int damage;
                                float kb;
                                int usedAmmo;
                                
                                ItemLoader.ModifyShootStats(item2, player, ref pos, ref velocity, ref shoot, ref damage, ref item2.knockBack);
                            }
                            */
                            //proj.usesLocalNPCImmunity = true;
                            //proj.localNPCHitCooldown = 5;

                        }
                    }
                }
            }
        }
    }
}
