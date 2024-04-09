using Fargowiltas.Common.Configs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class SquireEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(148, 143, 140);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = 150000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SquireEffect(player, Item);
        }

        public static void SquireEffect(Player player, Item item)
        {
            player.AddEffect<SquireMountSpeed>(item);
            player.AddEffect<SquireMountJump>(item);
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.SquireEnchantActive = true;

            player.buffImmune[BuffID.BallistaPanic] = true;

            Mount mount = player.mount;

            if (mount.Active)
            {
                if (modPlayer.BaseMountType != mount.Type)
                {
                    //we want to reset the prev mount first or all hell breaks loose
                    if (modPlayer.BaseMountType != -1)
                    {
                        ResetMountStats(modPlayer);
                    }

                    Mount.MountData original = Mount.mounts[mount.Type];
                    //copy over ANYTHING that will be changed
                    modPlayer.BaseSquireMountData = new Mount.MountData
                    {
                        acceleration = original.acceleration,
                        dashSpeed = original.dashSpeed,
                        fallDamage = original.fallDamage,

                        jumpSpeed = original.jumpSpeed,
                        //usesHover = original.usesHover
                    };

                    modPlayer.BaseMountType = mount.Type;
                }

                int defenseBoost;
                float accelBoost;
                float speedBoost;

                if (modPlayer.ValhallaEnchantActive && modPlayer.ForceEffect<ValhallaKnightEnchant>())
                {
                    defenseBoost = 20;
                    accelBoost = 2f;
                    speedBoost = 1.5f;
                }
                else if (modPlayer.ValhallaEnchantActive || modPlayer.ForceEffect<SquireEnchant>())
                {
                    defenseBoost = 15;
                    accelBoost = 1.5f;
                    speedBoost = 1.5f;
                }
                else
                {
                    defenseBoost = 10;
                    accelBoost = 1.25f;
                    speedBoost = 1.25f;
                }


                if (!player.HasEffect<SquireMountSpeed>())
                {
                    accelBoost = 1;
                    speedBoost = 1;
                }
                if (player.HasEffect<ValhallaDash>())
                {

                    if (modPlayer.ValhallaDashCD > 0)
                    {
                        modPlayer.ValhallaDashCD--;
                    }

                    if (modPlayer.ValhallaDashCD == 0)
                    {
                        if (Main.myPlayer == player.whoAmI)
                        {
                            if (Fargowiltas.Fargowiltas.DashKey.Current)
                            {
                                if (player.controlDown)
                                {
                                    ValhallaDash(player, true, 1);
                                }
                                //up
                                else if (player.controlUp)
                                {
                                    ValhallaDash(player, true, -1);
                                }
                                if (player.controlRight)
                                {
                                    ValhallaDash(player, false, 1);
                                }
                                else if (player.controlLeft)
                                {
                                    ValhallaDash(player, false, -1);
                                }
                            }
                            else if (!ModContent.GetInstance<FargoClientConfig>().DoubleTapDashDisabled)
                            {
                                //mount dash
                                if ((player.controlDown && (player.releaseDown)))
                                {
                                    if (player.doubleTapCardinalTimer[0] > 0 && player.doubleTapCardinalTimer[0] != 15)
                                    {
                                        ValhallaDash(player, true, 1);
                                    }
                                }
                                //up
                                else if ((player.controlUp && player.releaseUp))
                                {
                                    if (player.doubleTapCardinalTimer[1] > 0 && player.doubleTapCardinalTimer[1] != 15)
                                    {
                                        ValhallaDash(player, true, -1);
                                    }
                                }
                                if (player.controlRight && player.releaseRight)
                                {
                                    if (player.doubleTapCardinalTimer[2] > 0 && player.doubleTapCardinalTimer[2] != 15)
                                    {
                                        ValhallaDash(player, false, 1);
                                    }
                                }
                                else if (player.controlLeft && player.releaseLeft)
                                {
                                    if (player.doubleTapCardinalTimer[3] > 0 && player.doubleTapCardinalTimer[3] != 15)
                                    {
                                        ValhallaDash(player, false, -1);
                                    }
                                }
                            }
                        }
                    }
                }

                if (player.HasEffect<SquireMountJump>())
                {
                    player.GetJumpState(ExtraJump.FartInAJar).Enable();
                }

                player.statDefense += defenseBoost;

                mount._data.acceleration = modPlayer.BaseSquireMountData.acceleration * accelBoost;
                mount._data.dashSpeed = modPlayer.BaseSquireMountData.dashSpeed * speedBoost;
                mount._data.jumpSpeed = modPlayer.BaseSquireMountData.jumpSpeed * speedBoost;
                mount._data.fallDamage = 0;
                player.noFallDmg = true;

                /*
                if (modPlayer.IsDashingTimer == 0)
                {
                    //mount._data.usesHover = modPlayer.BaseSquireMountData.usesHover;
                }
                */


                //Main.NewText(mount.DashSpeed);
            }
        }

        public static void ValhallaDash(Player player, bool vertical, int direction)
        {
            //horizontal
            if (!vertical)
            {
                player.FargoSouls().MonkDashing = 10;
                player.velocity.X = 50 * (float)direction;
            }
            else
            {
                float multi;

                //down
                if (direction == 1)
                {
                    multi = 80;
                }
                //up
                else
                {
                    multi = 40;
                }

                player.FargoSouls().MonkDashing = -10;
                player.velocity.Y = multi * (float)direction;
            }

            player.FargoSouls().ValhallaDashCD = 30;
            player.dashDelay = 60;
            if (player.FargoSouls().IsDashingTimer < 10)
                player.FargoSouls().IsDashingTimer = 10;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);

            //dash dust n stuff
            for (int num17 = 0; num17 < 20; num17++)
            {
                int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Dust expr_CDB_cp_0 = Main.dust[num18];
                expr_CDB_cp_0.position.X += Main.rand.Next(-5, 6);
                Dust expr_D02_cp_0 = Main.dust[num18];
                expr_D02_cp_0.position.Y += Main.rand.Next(-5, 6);
                Main.dust[num18].velocity *= 0.2f;
                Main.dust[num18].scale *= 1f + Main.rand.Next(20) * 0.01f;
                //Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, this);
            }
            int num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 34f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
            num19 = Gore.NewGore(player.GetSource_FromThis(), new Vector2(player.position.X + player.width / 2 - 24f, player.position.Y + player.height / 2 - 14f), default, Main.rand.Next(61, 64), 1f);
            Main.gore[num19].velocity.X = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity.Y = Main.rand.Next(-50, 51) * 0.01f;
            Main.gore[num19].velocity *= 0.4f;
        }

        public static void ResetMountStats(FargoSoulsPlayer modPlayer)
        {
            if (modPlayer.BaseSquireMountData == null || modPlayer.BaseMountType < 0 || modPlayer.BaseMountType >= Mount.mounts.Length)
            {
                return;
            }

            Mount.mounts[modPlayer.BaseMountType].acceleration = modPlayer.BaseSquireMountData.acceleration;
            Mount.mounts[modPlayer.BaseMountType].dashSpeed = modPlayer.BaseSquireMountData.dashSpeed;
            Mount.mounts[modPlayer.BaseMountType].fallDamage = modPlayer.BaseSquireMountData.fallDamage;

            Mount.mounts[modPlayer.BaseMountType].jumpSpeed = modPlayer.BaseSquireMountData.jumpSpeed;
            //Mount.mounts[modPlayer.BaseMountType].usesHover = modPlayer.BaseSquireMountData.usesHover;
            modPlayer.BaseMountType = -1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.SquireGreatHelm)
            .AddIngredient(ItemID.SquirePlating)
            .AddIngredient(ItemID.SquireGreaves)
            .AddIngredient(ItemID.DD2BallistraTowerT2Popper)
            .AddIngredient(ItemID.MajesticHorseSaddle)
            .AddIngredient(ItemID.JoustingLance)

            .AddTile(TileID.CrystalBall)
            .Register();
        }
    }
    public class SquireMountSpeed : AccessoryEffect
    {

        public override Header ToggleHeader => Header.GetHeader<WillHeader>();

        public override int ToggleItemType => ModContent.ItemType<SquireEnchant>();
    }
    public class SquireMountJump : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WillHeader>();
        public override int ToggleItemType => ModContent.ItemType<SquireEnchant>();
    }
}
