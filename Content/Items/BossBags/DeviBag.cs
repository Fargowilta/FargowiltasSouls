using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.BossBags
{
    public class DeviBag : BossBag
    {
        protected override bool IsPreHMBag => true;
        public override void RightClick(Player player)
        {
            for (int i = -1; i < 2; i++)
            {
                Vector2 vel = Vector2.UnitY * 0.5f + Vector2.UnitX * 6 * i;
                Projectile.NewProjectile(player.GetSource_OpenItem(Type), player.Center, vel, ProjectileID.FoulPotion, 0, 0, player.whoAmI);
            }
            base.RightClick(player);
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeviatingEnergy>(), 1, 15, 30));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<DeviBoss>()));
        }
    }
}