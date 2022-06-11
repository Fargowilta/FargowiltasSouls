using FargowiltasSouls.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class EridanusHat : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eridanus Hat");
            Tooltip.SetDefault(@"5% increased damage
5% increased critical strike chance
Increases your max number of minions by 4
Increases your max number of sentries by 4");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 14);
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 5;

            player.maxMinions += 4;
            player.maxTurrets += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<EridanusBattleplate>() && legs.type == ModContent.ItemType<EridanusLegwear>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.FargowiltasSouls.SetBonus.Eridanus");

            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            fargoPlayer.EridanusEmpower = true;

            if (fargoPlayer.EridanusTimer % (60 * 10) == 1) //make dust whenever changing classes
            {
                SoundEngine.PlaySound(SoundID.Item4, player.Center);

                int type;
                switch (fargoPlayer.EridanusTimer / (60 * 10))
                {
                    case 0: type = 127; break; //solar
                    case 1: type = 229; break; //vortex
                    case 2: type = 242; break; //nebula
                    default: //stardust
                        type = 135;
                        player.GetModPlayer<EModePlayer>().MasomodeMinionNerfTimer = 0; //so that player isn't punished for using weapons during prior phase
                        break;
                }

                const int max = 100; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = Vector2.UnitY * 20f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + player.Center;
                    Vector2 vector7 = vector6 - player.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, type, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }

                for (int i = 0; i < 50; i++) //make some indicator dusts
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, type, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 24f;
                }

                if (Main.myPlayer == player.whoAmI)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++) //clear minions
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI
                            && Main.projectile[i].type != ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>()
                            && Main.projectile[i].minionSlots > 0)
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }
            }

            if (++fargoPlayer.EridanusTimer > 60 * 10 * 4) //handle loop
            {
                fargoPlayer.EridanusTimer = 0;
            }

            void Bonuses(DamageClass damageClass)
            {
                player.GetDamage(damageClass) += 0.75f;

                if (damageClass == DamageClass.Summon)
                    fargoPlayer.SpiderEnchantActive = true;
                
                player.GetCritChance(damageClass) += 20;

                if (player.HeldItem.CountsAsClass(damageClass))
                    fargoPlayer.AttackSpeed += .3f;
            }

            switch (fargoPlayer.EridanusTimer / (60 * 10)) //damage boost according to current class
            {
                case 0: Bonuses(DamageClass.Melee); break;
                case 1: Bonuses(DamageClass.Ranged); break;
                case 2: Bonuses(DamageClass.Magic); break;
                default: Bonuses(DamageClass.Summon); break;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>()] < 1)
                {
                    FargoSoulsUtil.NewSummonProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.EridanusMinion>(), 300, 12f, player.whoAmI, -1);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.EridanusRitual>()] < 1)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.EridanusRitual>(), 0, 0f, player.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Eridanium>(), 5)
            .AddIngredient(ItemID.FragmentSolar, 5)
            .AddIngredient(ItemID.FragmentVortex, 5)
            .AddIngredient(ItemID.FragmentNebula, 5)
            .AddIngredient(ItemID.FragmentStardust, 5)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}