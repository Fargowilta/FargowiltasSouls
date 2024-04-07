using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.BossBars
{
    public class CompositeBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];

            return base.GetIconTexture(ref iconFrame);
        }
        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)/* tModPorter Note: life and shield current and max values are now separate to allow for hp/shield number text draw */
        {
            NPC npc = FargoSoulsUtil.NPCExists(info.npcIndexToAimAt);

            if (npc == null || !npc.active)
                return false;

            bossHeadIndex = npc.GetBossHeadTextureIndex();

            bool retval = true;
            if (npc.ModNPC is TrojanSquirrel trojanSquirrel)
            {
                float lifeSegments = 0;
                if (trojanSquirrel.head != null)
                    lifeSegments += trojanSquirrel.head.life;
                if (trojanSquirrel.arms != null)
                    lifeSegments += trojanSquirrel.arms.life;
                life = npc.life + lifeSegments;
                lifeMax = npc.lifeMax + trojanSquirrel.lifeMaxHead + trojanSquirrel.lifeMaxArms;
            }
            else if (npc.type == ModContent.NPCType<ShadowChampion>())
            {
                float shieldSegments = 0;
                float shieldMaxSegments = 0;
                if (npc.ai[0] == -1)
                {
                    shield = 1;
                    shieldMax = 1;
                    // shieldPercent = 1;
                }
                else
                {
                    foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<ShadowOrbNPC>() && n.ai[0] == npc.whoAmI))
                    {
                        shieldMaxSegments++;
                        if (!n.dontTakeDamage)
                            shieldSegments++;
                    }
                    shield = shieldSegments;
                    shieldMax = shieldMaxSegments;
                    if (shield <= 0)
                    {
                        return null;
                    }

                }
            }
            else if (npc.type == NPCID.LunarTowerStardust)
            {
                int cellLife = 0;
                int cellLifeMax = 0;
                foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[2] == npc.whoAmI && n.frame.Y == 0)) //frame check is to check if big
                {
                    cellLife += n.life;
                    cellLifeMax += n.lifeMax;
                }
                life = npc.life + cellLife;
                lifeMax = npc.lifeMax + cellLifeMax;
                return true;
            }
            else
            {
                retval = false;
            }


            return retval;
        }
    }
}
