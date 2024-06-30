using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class JungleEnchant : BaseEnchant
    {
        public override Color nameColor => new(113, 151, 31);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<JungleJump>(Item);
            player.AddEffect<JungleDashEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.JungleHat)
            .AddIngredient(ItemID.JungleShirt)
            .AddIngredient(ItemID.JunglePants)
            .AddIngredient(ItemID.ThornChakram)
            .AddIngredient(ItemID.IvyWhip)
            .AddIngredient(ItemID.JungleRose)
            //.AddIngredient(ItemID.Buggy);
            //panda pet

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
    public class JungleDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<JungleEnchant>();
        
        public static void AddDash(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.HasDash)
                return;
            modPlayer.HasDash = true;
            modPlayer.FargoDash = DashManager.DashType.Jungle;
        }
        public static void JungleDash(Player player, int direction)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dashSpeed = modPlayer.ChlorophyteEnchantActive ? 12f : 9f;
            player.velocity.X = dashSpeed * direction;
            if (modPlayer.IsDashingTimer < 10)
                modPlayer.IsDashingTimer = 10;
            player.dashDelay = 60;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        }
    }
    public class JungleJump : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<JungleEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.grapCount > 0)
            {
                modPlayer.CanJungleJump = true;
                modPlayer.JungleJumping = false;
            }
            else if (player.controlJump)
            {
                if (player.GetJumpState(ExtraJump.BlizzardInABottle).Available || player.GetJumpState(ExtraJump.SandstormInABottle).Available || player.GetJumpState(ExtraJump.CloudInABottle).Available || player.GetJumpState(ExtraJump.FartInAJar).Available || player.GetJumpState(ExtraJump.TsunamiInABottle).Available || player.GetJumpState(ExtraJump.UnicornMount).Available)
                {
                }
                else
                {
                    if (player.jump == 0 && player.releaseJump && player.velocity.Y != 0f && !player.mount.Active && modPlayer.CanJungleJump)
                    {
                        player.jump = (int)((double)Player.jumpHeight * 3);

                        modPlayer.JungleJumping = true;
                        modPlayer.JungleCD = 0;
                        modPlayer.CanJungleJump = false;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
                    }
                }
            }

            if (modPlayer.JungleJumping)
            {
                if (player.rocketBoots > 0)
                {
                    modPlayer.savedRocketTime = player.rocketTimeMax;
                    player.rocketTime = 0;
                }

                player.runAcceleration *= 3f;
                //Player.maxRunSpeed *= 2f;

                //spwn cloud
                if (modPlayer.JungleCD == 0)
                {
                    int tier = 1;
                    if (modPlayer.ChlorophyteEnchantActive)
                        tier++;
                    bool jungleForceEffect = modPlayer.ForceEffect<JungleEnchant>();
                    if (jungleForceEffect)
                        tier++;

                    modPlayer.JungleCD = 18 - tier * tier;
                    int dmg = 12 * tier * tier - 5;

                    SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f }, player.Center);

                    if (player.whoAmI == Main.myPlayer)
                    {
                        foreach (Projectile p in FargoSoulsUtil.XWay(10, GetSource_EffectItem(player), player.Bottom, ProjectileID.SporeCloud, 4f, FargoSoulsUtil.HighestDamageTypeScaling(player, dmg), 0f))
                        {
                            if (p == null)
                                continue;
                            p.usesIDStaticNPCImmunity = true;
                            p.idStaticNPCHitCooldown = 10;
                            p.FargoSouls().noInteractionWithNPCImmunityFrames = true;
                            p.extraUpdates += 1;
                        }
                    }
                }

                if (player.jump == 0 || player.velocity == Vector2.Zero)
                {
                    modPlayer.JungleJumping = false;
                    player.rocketTime = modPlayer.savedRocketTime;
                }
            }
            else if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.CanJungleJump = true;
            }

            if (modPlayer.JungleCD != 0)
            {
                modPlayer.JungleCD--;
            }
        }
    }
}
