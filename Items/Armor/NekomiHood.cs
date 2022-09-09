using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class NekomiHood : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Nekomi Hood");
            Tooltip.SetDefault(@"7% increased damage
Increases max number of minions by 2");
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 50);
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.07f;
            player.maxMinions += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<NekomiHoodie>() && legs.type == ModContent.ItemType<NekomiLeggings>();
        }

        public const int MAX_METER = 60 * 60;
        public const int MAX_HEARTS = 9;

        public override void UpdateArmorSet(Player player)
        {
            string key = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            player.setBonus = Language.GetTextValue($"Mods.{Mod.Name}.SetBonus.Nekomi", key);

            player.GetDamage(DamageClass.Generic) += 0.07f;
            player.GetCritChance(DamageClass.Generic) += 7;

            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            fargoPlayer.NekomiSet = true;
            fargoPlayer.GrazeRadius *= 1.5f;

            const int decayTime = 420 / 2;
            if (fargoPlayer.NekomiTimer > 0)
            {
                const int bonusSpeedPoint = 90;
                int increment = fargoPlayer.NekomiTimer / bonusSpeedPoint + 1;
                fargoPlayer.NekomiTimer -= increment;
                fargoPlayer.NekomiMeter += increment;

                if (fargoPlayer.NekomiMeter > MAX_METER)
                    fargoPlayer.NekomiMeter = MAX_METER;
            }
            else if (--fargoPlayer.NekomiTimer < -decayTime)
            {
                if (fargoPlayer.NekomiTimer < -decayTime * 2)
                    fargoPlayer.NekomiTimer = -decayTime * 2;

                int depreciation = -decayTime - fargoPlayer.NekomiTimer;
                fargoPlayer.NekomiMeter -= (int)MathHelper.Lerp(1, MAX_METER / decayTime, depreciation / decayTime);
                if (fargoPlayer.NekomiMeter < 0)
                    fargoPlayer.NekomiMeter = 0;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                int ritualType = ModContent.ProjectileType<NekomiRitual>();
                if (player.ownedProjectileCounts[ritualType] < 1)
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ritualType, 0, 0f, player.whoAmI);

                bool doubleTap = Main.ReversedUpDownArmorSetBonuses
                    ? player.controlUp && player.releaseUp && player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15
                    : player.controlDown && player.releaseDown && player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15;

                if (doubleTap)
                {
                    bool superAttack = fargoPlayer.NekomiMeter >= MAX_METER;
                    if (superAttack)
                    {
                        int baseDamage = 2222 / 3;
                        if (!Main.hardMode)
                            baseDamage /= 2;
                        FargoSoulsUtil.NewSummonProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<NekomiDevi>(), baseDamage, 16f, player.whoAmI);
                        SoundEngine.PlaySound(SoundID.Item43, player.Center);
                        fargoPlayer.NekomiMeter = 0;
                    }
                    else
                    {
                        int hearts = (int)((double)fargoPlayer.NekomiMeter / MAX_METER * MAX_HEARTS);
                        for (int i = 0; i < hearts; i++)
                        {
                            Vector2 offset = -150f * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / hearts * i);
                            Vector2 spawnPos = player.Center + offset;
                            const float speed = 12;
                            Vector2 vel = speed * player.DirectionFrom(spawnPos);
                            int baseHeartDamage = 17;
                            const float ai1 = 150 / speed;
                            FargoSoulsUtil.NewSummonProjectile(player.GetSource_Accessory(Item), spawnPos, vel, ModContent.ProjectileType<FriendHeart>(), baseHeartDamage, 3f, player.whoAmI, -1, ai1);
                        }

                        if (hearts > 0)
                            fargoPlayer.NekomiMeter = 0;
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 5)
            .AddTile(TileID.Loom)

            .Register();
        }
    }
}