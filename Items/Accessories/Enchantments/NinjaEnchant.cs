using FargowiltasSouls.Buffs.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class NinjaEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Ninja Enchantment");
            Tooltip.SetDefault(
@"Use Ninja hotkey to throw a smoke bomb, use it again to teleport to it and gain the First Strike Buff
First Strike ensures your next attack hits a vital spot dealing 3x damage and reducing defense by 10
'Now you see me, now you don’t'");

            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "忍者魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"按下'忍者秘技'键后会扔出一颗烟雾弹，再次按下'忍者秘技'键时会将你传送至其落点的位置并使你获得先发制人增益
            // 使用混沌传送杖也会获得先发制人增益
            // 先发制人增益会使你下次攻击必定暴击且造成3倍伤害
            // '你现在能看到我了，诶，你又看不到我了'");
        }

        protected override Color nameColor => new Color(48, 49, 52);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 30000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            NinjaEffect(player);
        }

        public static void NinjaEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.NinjaEnchantActive = true;

            //rod bonus
            if (player.controlUseItem && player.HeldItem.type == ItemID.RodofDiscord)
            {
                player.AddBuff(ModContent.BuffType<FirstStrike>(), 60);
            }

            //cooldown
            if (modPlayer.SmokeBombCD != 0)
            {
                modPlayer.SmokeBombCD--;
            }
        }

        public static void SmokeBombKey(FargoSoulsPlayer modPlayer)
        {
            //throw smoke bomb
            if (modPlayer.NinjaSmokeBombProj == null)
            {
                const float gravity = 0.18f;
                float time = 60f;
                Vector2 distance = Main.MouseWorld - modPlayer.Player.Center;
                distance.X = distance.X / time;
                distance.Y = distance.Y / time - 0.5f * gravity * time;

                modPlayer.NinjaSmokeBombProj = Main.projectile[Projectile.NewProjectile(modPlayer.Player.GetSource_Misc(""), modPlayer.Player.Center, distance + Main.rand.NextVector2Square(0, 0) * 2,
                        ProjectileID.SmokeBomb, 0, 0f, Main.myPlayer)];

                modPlayer.SmokeBombCD = 15;
            }
            //already threw smoke bomb, tele to it
            else
            {
                Vector2 teleportPos = new Vector2(modPlayer.NinjaSmokeBombProj.position.X, modPlayer.NinjaSmokeBombProj.position.Y - 30);
                Vector2 originalPos = new Vector2(teleportPos.X, teleportPos.Y);

                //spiral out to find a save spot
                int count = 0;
                int increase = 10;
                while (Collision.SolidCollision(teleportPos, modPlayer.Player.width, modPlayer.Player.height))
                {
                    teleportPos = originalPos;

                    switch (count)
                    {
                        case 0:
                            teleportPos.X -= increase;
                            break;
                        case 1:
                            teleportPos.X += increase;
                            break;
                        case 2:
                            teleportPos.Y += increase;
                            break;
                        default:
                            teleportPos.Y -= increase;
                            increase += 10;
                            break;
                    }
                    count++;

                    if (count >= 4)
                    {
                        count = 0;
                    }

                    if (increase > 100)
                    {
                        return;
                    }
                }

                if (teleportPos.X > 50 && teleportPos.X < (double)(Main.maxTilesX * 16 - 50) && teleportPos.Y > 50 && teleportPos.Y < (double)(Main.maxTilesY * 16 - 50))
                {
                    modPlayer.Player.Teleport(teleportPos, 1);
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, modPlayer.Player.whoAmI, teleportPos.X, teleportPos.Y, 1);

                    modPlayer.Player.AddBuff(ModContent.BuffType<FirstStrike>(), 60);

                    modPlayer.NinjaSmokeBombProj.timeLeft = 120;
                    modPlayer.SmokeBombCD = 300;
                    modPlayer.NinjaSmokeBombProj = null;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient(ItemID.Shuriken, 100)
                .AddIngredient(ItemID.ThrowingKnife, 100)
                .AddIngredient(ItemID.SmokeBomb, 50)

                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
