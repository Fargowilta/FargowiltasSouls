using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GaiaHelmet : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gaia Helmet");
            Tooltip.SetDefault(@"10% increased damage
5% increased critical strike chance
Increases max number of minions and sentries by 1");
            DisplayName.AddTranslation(GameCulture.Chinese, "盖亚头盔");
            Tooltip.AddTranslation(GameCulture.Chinese, "增加10%伤害\n增加5%暴击几率\n增加1个召唤栏位和1个哨兵栏位");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.sellPrice(0, 5);
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<FargoPlayer>().AllDamageUp(0.1f);
            player.GetModPlayer<FargoPlayer>().AllCritUp(5);

            player.maxMinions += 1;
            player.maxTurrets += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GaiaPlate>() && legs.type == ModContent.ItemType<GaiaGreaves>();
        }

        public override void ArmorSetShadows(Player player)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            if (fargoPlayer.GaiaOffense)
            {
                player.armorEffectDrawOutlinesForbidden = true;
                player.armorEffectDrawShadow = true;
            }
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = @"10% increased melee speed
Reduces mana usage by 10%
10% chance to not consume ammo
Increases max number of minions and sentries by 1
Hold up and double tap down to toggle offensive mode, which has the following effects:
20% increased damage and 10% increased critical strike chance
Increases armor penetration by 10
Reduces defense by 20
Reduces max life and damage reduction by 20%";
            if (GameCulture.Chinese.IsActive)
            {
                player.setBonus = "增加10%近战武器使用速度\n减少10%魔力花费\n增加10%几率不消耗弹药\n增加1个召唤栏位和1个哨兵栏位\n按住向上时同时双击向下键会进入进攻模式,进攻模式具有如下效果:增加20%伤害和10%暴击几率\n增加10点护甲穿透\n减少20点防御\n增加20%最大生命值和20%伤害减免";
            }

            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.GaiaSet = true;

            player.meleeSpeed += 0.1f;
            player.manaCost -= 0.1f;
            player.maxMinions += 1;
            player.maxTurrets += 1;

            if (player.whoAmI == Main.myPlayer && player.controlUp && player.controlDown && player.releaseDown
                && player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
            {
                fargoPlayer.GaiaOffense = !fargoPlayer.GaiaOffense;

                if (fargoPlayer.GaiaOffense)
                    Main.PlaySound(SoundID.Item4, player.Center);

                Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                const int max = 36; //make some indicator dusts
                for (int i = 0; i < max; i++)
                {
                    Vector2 vector6 = baseVel * 6f;
                    vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + player.Center;
                    Vector2 vector7 = vector6 - player.Center;
                    int d = Dust.NewDust(vector6 + vector7, 0, 0, Main.rand.Next(2) == 0 ? 107 : 110, 0f, 0f, 0, default(Color));
                    Main.dust[d].scale = 2.5f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = vector7;
                }
            }

            if (fargoPlayer.GaiaOffense)
            {
                fargoPlayer.AllDamageUp(0.3f);
                fargoPlayer.AllCritUp(15);
                player.armorPenetration += 20;
                player.statDefense -= 20;
                player.statLifeMax2 -= player.statLifeMax / 5;
                player.endurance -= 0.2f;
                Lighting.AddLight(player.Center, new Vector3(1, 1, 1));
                if (Main.rand.Next(3) == 0) //visual dust
                {
                    float scale = 2f;
                    int type = Main.rand.Next(2) == 0 ? 107 : 110;
                    int dust = Dust.NewDust(player.position, player.width, player.height, type, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 87, default(Color), scale);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 1f;
                    Main.dust[dust].velocity *= 1.8f;
                    if (Main.rand.Next(4) == 0)
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BeetleHusk, 3);
            recipe.AddIngredient(ItemID.ShroomiteBar, 6);
            recipe.AddIngredient(ItemID.SpectreBar, 6);
            recipe.AddIngredient(ItemID.SpookyWood, 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
