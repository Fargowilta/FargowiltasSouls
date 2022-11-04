using FargowiltasSouls.NPCs;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MoltenEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Molten Enchantment");
            Tooltip.SetDefault(
@"Nearby enemies are ignited
Enemies take 25% increased damage while inside the inferno ring
'They shall know the fury of hell' ");
        }

        protected override Color nameColor => new Color(193, 43, 43);
        public override string wizardEffect => "";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MoltenEffect(player);
        }

        public static void MoltenEffect(Player player)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.MoltenEnchantActive = true;

            if (player.GetToggleValue("Molten") && player.whoAmI == Main.myPlayer)
            {
                player.inferno = true;
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
                int buff = BuffID.OnFire;
                float distance = 200f;
                int baseDamage = 25;

                if (modPlayer.NatureForce)
                {
                    baseDamage *= 2;
                }

                int damage = FargoSoulsUtil.HighestDamageTypeScaling(player, baseDamage);

                if (player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && !(npc.damage == 0 && npc.lifeMax == 5)) //critters
                        {
                            if (Vector2.Distance(player.Center, FargoSoulsUtil.ClosestPointInHitbox(npc.Hitbox, player.Center)) <= distance)
                            {
                                int dmgRate = 30;//60;

                                if (npc.FindBuffIndex(buff) == -1)
                                {
                                    npc.AddBuff(buff, 120);
                                }

                                npc.GetGlobalNPC<FargoSoulsGlobalNPC>().MoltenAmplify = true;

                                //if (Vector2.Distance(player.Center, npc.Center) <= 50)
                                //{
                                //    dmgRate /= 10;
                                //}
                                //else if (Vector2.Distance(player.Center, npc.Center) <= 100)
                                //{
                                //    dmgRate /= 5;
                                //}
                                //else if (Vector2.Distance(player.Center, npc.Center) <= 150)
                                //{
                                //    dmgRate /= 2;
                                //}

                                if (player.infernoCounter % dmgRate == 0)
                                {
                                    player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.MoltenHelmet)
            .AddIngredient(ItemID.MoltenBreastplate)
            .AddIngredient(ItemID.MoltenGreaves)
            //ashwood ench
            .AddIngredient(ItemID.Sunfury)
            .AddIngredient(ItemID.DemonsEye)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
