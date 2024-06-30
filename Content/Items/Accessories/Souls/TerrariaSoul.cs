using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerrariaSoul : BaseSoul
    {
        public static List<int> Forces = 
            [
            ModContent.ItemType<TimberForce>(),
            ModContent.ItemType<TerraForce>(),
            ModContent.ItemType<EarthForce>(),
            ModContent.ItemType<NatureForce>(),
            ModContent.ItemType<LifeForce>(),
            ModContent.ItemType<ShadowForce>(),
            ModContent.ItemType<SpiritForce>(),
            ModContent.ItemType<WillForce>(),
            ModContent.ItemType<CosmoForce>()
            ];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Soul of Terraria");

            // Tooltip.SetDefault(tooltip);

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 24));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye), Item, null); //use living rainbow dye shader
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = -12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            foreach (int force in Forces)
                modPlayer.ForceEffects.Add(force);

            //includes revive, both spectres, adamantite, and star heal
            modPlayer.TerrariaSoul = true;
            modPlayer.WizardEnchantActive = true;

            // super moons
            player.AddEffect<TerrariaMoonEffect>(Item);
            // revive
            player.AddEffect<FossilEffect>(Item);
            // meteor movement
            player.AddEffect<MeteorMomentumEffect>(Item);
            // coins to piggy
            player.AddEffect<GoldToPiggy>(Item);
            // platinum loot
            modPlayer.PlatinumEffect = Item;
            // wood discount and completion
            player.FargoSouls().WoodEnchantDiscount = true;
            player.AddEffect<WoodCompletionEffect>(Item);
            // iron items and attraction
            IronEnchant.AddEffects(player, Item);

            /*
            //TIMBER
            ModContent.GetInstance<TimberForce>().UpdateAccessory(player, hideVisual);
            //TERRA
            ModContent.GetInstance<TerraForce>().UpdateAccessory(player, hideVisual);
            //EARTH
            ModContent.GetInstance<EarthForce>().UpdateAccessory(player, hideVisual);
            //NATURE
            ModContent.GetInstance<NatureForce>().UpdateAccessory(player, hideVisual);
            //LIFE
            ModContent.GetInstance<LifeForce>().UpdateAccessory(player, hideVisual);
            //SPIRIT
            ModContent.GetInstance<SpiritForce>().UpdateAccessory(player, hideVisual);
            //SHADOW
            ModContent.GetInstance<ShadowForce>().UpdateAccessory(player, hideVisual);
            //WILL
            ModContent.GetInstance<WillForce>().UpdateAccessory(player, hideVisual);
            //COSMOS
            ModContent.GetInstance<CosmoForce>().UpdateAccessory(player, hideVisual);
            */
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
            player.AddEffect<GoldToPiggy>(Item);
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }

        public override void UpdateInventory(Player player)
        {
            player.FargoSouls().WoodEnchantDiscount = true;
            player.AddEffect<GoldToPiggy>(Item);
            AshWoodEnchant.PassiveEffect(player);
            IronEnchant.AddEffects(player, Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int force in Forces)
                recipe.AddIngredient(force);

            recipe.AddIngredient(null, "AbomEnergy", 10)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();
        }
    }
    public class TerrariaMoonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EternityHeader>();
        public override int ToggleItemType => ModContent.ItemType<TerrariaSoul>();
        public override bool ExtraAttackEffect => true;
        

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerrariaSoulProcCD > 0)
                modPlayer.TerrariaSoulProcCD--;
        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (hitInfo.Crit)
            {
                MoonProc(player, target);
            }
        }

        public static void MoonProc(Player player, NPC target)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.TerrariaSoulProcCD == 0)
            {
                int dmg = 1222;
                int cdLength = 160;

                // cooldown scaling from 2x to 1x depending on how recently you got hurt
                int maxHurtTime = 60 * 30;
                if (modPlayer.TimeSinceHurt < maxHurtTime)
                {
                    float multiplier = 2f - (modPlayer.TimeSinceHurt / maxHurtTime) * 1f;
                    cdLength = (int)(cdLength * multiplier);
                }

                Vector2 ai = target.Center - player.Center;
                Vector2 velocity = Vector2.Normalize(ai) * 0.1f;

                int damage = FargoSoulsUtil.HighestDamageTypeScaling(modPlayer.Player, dmg);
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ThrowShort") with { Volume = 0.5f }, player.Center);
                FargoSoulsUtil.NewProjectileDirectSafe(modPlayer.Player.GetSource_ItemUse(player.HeldItem), player.Center, velocity, ModContent.ProjectileType<TerrariaSoulMoon>(), damage, 0f, player.whoAmI, ai1: target.whoAmI);

                modPlayer.TerrariaSoulProcCD = cdLength;
            }
        }
        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.TerrariaSoulProcCD = 160 * 2;
        }
    }
}
