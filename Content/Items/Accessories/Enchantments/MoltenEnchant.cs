using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class MoltenEnchant : BaseEnchant
    {

        public override Color nameColor => new(193, 43, 43);


        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MoltenEffect>(Item);
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
    public class MoltenEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<NatureHeader>();
        public override int ToggleItemType => ModContent.ItemType<MoltenEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.inferno = true;
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.65f, 0.4f, 0.1f);
                int buff = BuffID.OnFire;
                float distance = 200f;
                int baseDamage = 40;

                if (player.FargoSouls().ForceEffect<MoltenEnchant>())
                {
                    distance *= 1.5f;
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
                                if (player.FindBuffIndex(BuffID.OnFire) == -1)
                                {
                                    player.AddBuff(BuffID.OnFire, 10);
                                }

                                int dmgRate = 30;//60;

                                if (npc.FindBuffIndex(buff) == -1)
                                {
                                    npc.AddBuff(buff, 120);
                                }

                                int moltenDebuff = ModContent.BuffType<Buffs.Souls.MoltenAmplifyBuff>();
                                if (npc.FindBuffIndex(moltenDebuff) == -1)
                                    npc.AddBuff(moltenDebuff, 10);

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
    }
}
