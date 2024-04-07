using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class TrawlerSoul : BaseSoul
    {

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 750000;
        }
        public static readonly Color ItemColor = new(0, 238, 125);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item, hideVisual);
        }
        public static void AddEffects(Player player, Item item, bool hideVisual)
        {
            Player Player = player;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //instacatch
            modPlayer.FishSoul1 = true;
            //extra lures
            modPlayer.FishSoul2 = true;

            //tackle bag
            Player.fishingSkill += 60;
            Player.sonarPotion = true;
            Player.cratePotion = true;
            Player.accFishingLine = true;
            Player.accTackleBox = true;
            Player.accFishFinder = true;
            Player.accLavaFishing = true;

            //volatile gel
            player.AddEffect<TrawlerGel>(item);

            //spore sac
            player.AddEffect<TrawlerSporeSac>(item);

            //arctic diving gear
            Player.arcticDivingGear = true;
            Player.accFlipper = true;
            Player.accDivingHelm = true;
            Player.iceSkate = true;
            if (Player.wet)
            {
                Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 0.2f, 0.8f, 0.9f);
            }

            //sharkron balloon
            player.AddEffect<TrawlerJump>(item);

            Player.jumpBoost = true;
            Player.noFallDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "AnglerEnchant")
            //inner tube
            .AddIngredient(ItemID.BalloonHorseshoeSharkron)
            .AddIngredient(ItemID.ArcticDivingGear)
            //frog gear
            .AddIngredient(ItemID.VolatileGelatin)
            .AddIngredient(ItemID.SporeSac)

            //engineer rod
            .AddIngredient(ItemID.SittingDucksFishingRod)
            //hotline fishing
            .AddIngredient(ItemID.GoldenFishingRod)
            .AddIngredient(ItemID.GoldenCarp)
            .AddIngredient(ItemID.ReaverShark)
            .AddIngredient(ItemID.Bladetongue)
            .AddIngredient(ItemID.ObsidianSwordfish)
            .AddIngredient(ItemID.FuzzyCarrot)
            .AddIngredient(ItemID.HardySaddle)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
    public class TrawlerGel : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ItemID.VolatileGelatin;
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return;
            }
            player.volatileGelatinCounter++;
            if (player.volatileGelatinCounter > 50)
            {
                player.volatileGelatinCounter = 0;
                int damage = 65;
                float knockBack = 7f;
                float num = 640f;
                NPC npc = null;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc2 = Main.npc[i];
                    if (npc2 != null && npc2.active && npc2.CanBeChasedBy(player, false) && Collision.CanHit(player, npc2))
                    {
                        float num2 = Vector2.Distance(npc2.Center, player.Center);
                        if (num2 < num)
                        {
                            num = num2;
                            npc = npc2;
                        }
                    }
                }
                if (npc != null)
                {
                    Vector2 vector = npc.Center - player.Center;
                    vector = vector.SafeNormalize(Vector2.Zero) * 6f;
                    vector.Y -= 2f;
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center.X, player.Center.Y, vector.X, vector.Y, ProjectileID.VolatileGelatinBall, damage, knockBack, player.whoAmI, 0f, 0f);
                }
            }
        }
    }
    public class TrawlerSporeSac : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ItemID.SporeSac;
        public override bool ExtraAttackEffect => true;
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.sporeSac = true;
                player.SporeSac(EffectItem(player));
            }
        }
    }
    public class TrawlerJump : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ModContent.ItemType<TrawlerSoul>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.wingTime == 0)
                player.GetJumpState(ExtraJump.TsunamiInABottle).Enable();
        }
    }
}
