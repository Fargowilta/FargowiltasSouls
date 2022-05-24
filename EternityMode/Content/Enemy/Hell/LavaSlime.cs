using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hell
{
    public class LavaSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.LavaSlime);

        public bool CanDoLavaJump;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.velocity.Y < 0f)
            {
                CanDoLavaJump = true;
            }
            else if (npc.velocity.Y > 0f) //coming down
            {
                //when below target, in hell, with line of sight
                if (CanDoLavaJump && Main.netMode != NetmodeID.MultiplayerClient && npc.HasValidTarget && npc.Bottom.Y > Main.player[npc.target].Bottom.Y
                    && npc.Center.ToTileCoordinates().Y > Main.maxTilesY - 200 && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                {
                    CanDoLavaJump = false;
                    //Projectile.NewProjectile(npc.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT1Explosion, 0, 0, Main.myPlayer);

                    int tileX = (int)(npc.Center.X + npc.velocity.X) / 16;
                    int tileY = (int)(npc.Center.Y + npc.velocity.Y) / 16;
                    Tile tile = Framing.GetTileSafely(tileX, tileY);
                    if (tile != null && !tile.HasTile && tile.LiquidAmount == 0)
                    {
                        tile.LiquidType = LiquidID.Lava; //does this still spawn the liquid properly??
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, tileX, tileY, 1);
                        WorldGen.SquareTileFrame(tileX, tileY, true);
                        npc.velocity.Y = 0;
                        npc.netUpdate = true;
                    }
                }
            }
            else //npc vel y = 0
            {
                CanDoLavaJump = false;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Oiled>(), 900);
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ItemID.LavaCharm, 100));
        }
    }
}
