using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class VortexEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Vortex Enchantment");
            /* Tooltip.SetDefault(
@"Double tap down to toggle stealth, reducing chance for enemies to target you but slowing movement
When entering stealth, spawn a vortex that draws in enemies and projectiles
While in stealth, your own projectiles will not be sucked in
'Tear into reality'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "星旋魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"双击'下'键切换至隐形模式，减少敌人以你为目标的几率，但大幅降低移动速度
            // 进入隐形状态时生成一个会吸引敌人和弹幕的旋涡
            // 处于隐形状态时你的弹幕不会被旋涡吸引
            // '撕裂现实'");
        }

        public override Color nameColor => new(0, 242, 170);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            //portal spawn
            player.AddEffect<VortexStealthEffect>(item);
            player.AddEffect<VortexVortexEffect>(item);

            //player.AddEffect<VortexProjGravity>(item);

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.mount.Active)
                modPlayer.VortexStealth = false;

            if (modPlayer.VortexStealth)
            {
                player.moveSpeed *= 0.3f;
                player.aggro -= 1200;
                player.setVortex = true;
                player.stealth = 0f;
            }
        }
        public static void ActivateVortex(Player player)
        {
            if (player != Main.LocalPlayer)
            {
                return;
            }
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            bool stealthEffect = player.HasEffect<VortexStealthEffect>();
            bool vortexEffect = player.HasEffect<VortexVortexEffect>();
            if (stealthEffect || vortexEffect)
            {
                //stealth memes
                modPlayer.VortexStealth = !modPlayer.VortexStealth;

                if (!stealthEffect)
                    modPlayer.VortexStealth = false;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);

                if (modPlayer.VortexStealth && vortexEffect && !player.HasBuff(ModContent.BuffType<VortexCDBuff>()))
                {
                    int p = Projectile.NewProjectile(player.GetSource_EffectItem<VortexVortexEffect>(), player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Content.Projectiles.Souls.Void>(), FargoSoulsUtil.HighestDamageTypeScaling(player, 60), 5f, player.whoAmI);
                    Main.projectile[p].FargoSouls().CanSplit = false;
                    Main.projectile[p].netUpdate = true;

                    player.AddBuff(ModContent.BuffType<VortexCDBuff>(), 3600);
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.VortexHelmet)
            .AddIngredient(ItemID.VortexBreastplate)
            .AddIngredient(ItemID.VortexLeggings)
            //vortex wings
            .AddIngredient(ItemID.VortexBeater)
            .AddIngredient(ItemID.Phantasm)
            //chain gun
            //electrosphere launcher
            .AddIngredient(ItemID.SDMG)

            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
    public class VortexProjGravity : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool IgnoresMutantPresence => true;
        public override void PostUpdateEquips(Player player)
        {
            foreach (Projectile toProj in Main.projectile.Where(p => p != null && p.active && p.friendly && p.owner == player.whoAmI))
            {
                foreach (Projectile fromProj in Main.projectile.Where(p => p != null && p.active && p.friendly && p.whoAmI != toProj.whoAmI && p.owner == player.whoAmI && !TungstenEffect.TungstenAlwaysAffectProj(p) && p.FargoSouls().CanSplit && FargoSoulsUtil.CanDeleteProjectile(p, 0)))
                {
                    // if (fromProj.aiStyle != 1)
                    //    continue;
                    Vector2 dif = toProj.Center - fromProj.Center;
                    int distSq = (int)dif.LengthSquared();
                    if (distSq < 1 || float.IsNaN(distSq))
                        continue;
                    int rSquared = distSq;
                    rSquared += 100;
                    Vector2 force = Utils.SafeNormalize(dif, Vector2.UnitY);
                    const float gravityConstant = 9000f; // tweak
                    force *= gravityConstant /* (toProj.Size.Length() * fromProj.Size.Length())*/ / rSquared;
                    fromProj.velocity += force;
                }
            }
        }
    }
    public class VortexVortexEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<VortexEnchant>();
    }
    public class VortexStealthEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<VortexEnchant>();
    }
}
