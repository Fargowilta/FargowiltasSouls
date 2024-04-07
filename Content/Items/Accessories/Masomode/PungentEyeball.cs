using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class PungentEyeball : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pungent Eyeball");
            /* Tooltip.SetDefault(@"Grants immunity to Blackout and Obstructed
Increases spawn rate of rare enemies
Your cursor causes nearby enemies to take increased damage
Effect intensifies the longer you track them
'It's fermenting'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "辛辣的眼球");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'它在发酵'
            // 免疫致盲和阻塞
            // +2最大召唤栏
            // +2最大哨兵栏");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.AddEffect<PungentEyeballCursor>(Item);
            player.FargoSouls().PungentEyeball = true;
        }
    }
    public class PungentEyeballCursor : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<LumpofFleshHeader>();
        public override int ToggleItemType => ModContent.ItemType<PungentEyeball>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                const float distance = 16 * 5;

                foreach (NPC n in Main.npc.Where(n => n.active && !n.dontTakeDamage && n.lifeMax > 5 && !n.friendly))
                {
                    if (Vector2.Distance(Main.MouseWorld, FargoSoulsUtil.ClosestPointInHitbox(n.Hitbox, Main.MouseWorld)) < distance)
                    {
                        n.AddBuff(ModContent.BuffType<PungentGazeBuff>(), 2, true);
                    }
                }

                for (int i = 0; i < 32; i++)
                {
                    Vector2 spawnPos = Main.MouseWorld + Main.rand.NextVector2CircularEdge(distance, distance);
                    Dust dust = Main.dust[Dust.NewDust(spawnPos, 0, 0, DustID.GemRuby, 0, 0, 100, Color.White)];
                    dust.scale = 0.5f;
                    dust.velocity = Vector2.Zero;
                    if (Main.rand.NextBool(3))
                    {
                        dust.velocity += Vector2.Normalize(Main.MouseWorld - dust.position) * Main.rand.NextFloat(5f);
                        dust.position += dust.velocity * 5f;
                    }
                    dust.noGravity = true;
                }
            }
        }
    }
}