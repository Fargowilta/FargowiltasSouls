using FargowiltasSouls.Projectiles.Souls;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class CopperEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Copper Enchantment");
            Tooltip.SetDefault(
@"Crits have a chance to shock enemies with chain lightning
'Behold'");
        }

        protected override Color nameColor => new Color(213, 102, 23);
        public override string wizardEffect => "Lightning damage increased, cooldown reduced";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CopperEffect(player, Item);
        }

        public static void CopperEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.CopperEnchantItem = item;

            if (modPlayer.CopperProcCD > 0)
                modPlayer.CopperProcCD--;
        }

        public static void CopperProc(FargoSoulsPlayer modPlayer, NPC target)
        {
            if (modPlayer.Player.GetToggleValue("Copper") && modPlayer.CopperProcCD == 0)
            {
                target.AddBuff(BuffID.Electrified, 180);

                int dmg = 35;
                int maxTargets = 1;
                int cdLength = 300;

                if (modPlayer.TerraForce)
                {
                    dmg = 150;
                    maxTargets = 5;
                    cdLength = 150;
                }

                List<int> npcIndexes = new List<int>();
                float closestDist = 500f;
                NPC closestNPC;

                for (int i = 0; i < maxTargets; i++)
                {
                    closestNPC = null;

                    //find closest npc to target that has not been chained to yet
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC npc = Main.npc[j];

                        if (npc.active && npc.whoAmI != target.whoAmI && npc.CanBeChasedBy() && npc.Distance(target.Center) < closestDist && !npcIndexes.Contains(npc.whoAmI)
                            && Collision.CanHitLine(npc.Center, 0, 0, target.Center, 0, 0))
                        {
                            closestNPC = npc;
                            closestDist = npc.Distance(target.Center);
                            //break;
                        }
                    }

                    if (closestNPC != null)
                    {
                        npcIndexes.Add(closestNPC.whoAmI);

                        Vector2 ai = closestNPC.Center - target.Center;
                        Vector2 velocity = Vector2.Normalize(ai) * 20;

                        int damage = FargoSoulsUtil.HighestDamageTypeScaling(modPlayer.Player, dmg);
                        FargoSoulsUtil.NewProjectileDirectSafe(modPlayer.Player.GetSource_ItemUse(modPlayer.Player.HeldItem), target.Center, velocity, ModContent.ProjectileType<CopperLightning>(), damage, 0f, modPlayer.Player.whoAmI, ai.ToRotation(), damage);
                    }
                    else
                    {
                        break;
                    }

                    target = closestNPC;
                }

                modPlayer.CopperProcCD = cdLength;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CopperHelmet)
                .AddIngredient(ItemID.CopperChainmail)
                .AddIngredient(ItemID.CopperGreaves)
                .AddIngredient(ItemID.CopperShortsword)
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient(ItemID.ThunderStaff)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
