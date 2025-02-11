using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.PlayerDrawLayers;
using FargowiltasSouls.Content.Tiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Hooking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace FargowiltasSouls
{
    public partial class FargowiltasSouls : ICustomDetourProvider
    {
        private static readonly MethodInfo CombinedHooks_ModifyHitNPCWithProj_Method = typeof(CombinedHooks).GetMethod("ModifyHitNPCWithProj", LumUtils.UniversalBindingFlags);

        public delegate void Orig_CombinedHooks_ModifyHitNPCWithProj(Projectile projectile, NPC nPC, ref NPC.HitModifiers modifiers);

        public void LoadDetours()
        {
            On_Player.CheckSpawn_Internal += LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff += AddBuff;
            On_Player.QuickHeal_GetItemToUse += QuickHeal_GetItemToUse;
            On_Projectile.AI_019_Spears_GetExtensionHitbox += AI_019_Spears_GetExtensionHitbox;
            On_Item.AffixName += AffixName;
            On_NPCUtils.TargetClosestBetsy += TargetClosestBetsy;
            On_Main.MouseText_DrawItemTooltip_GetLinesInfo += MouseText_DrawItemTooltip_GetLinesInfo;
            On_Player.HorsemansBlade_SpawnPumpkin += HorsemansBlade_SpawnPumpkin;
            On_Player.ItemCheck_Shoot += InterruptShoot;
            On_Main.DrawInterface_35_YouDied += DrawInterface_35_YouDied;


        }
        public void UnloadDetours()
        {
            On_Player.CheckSpawn_Internal -= LifeRevitalizer_CheckSpawn_Internal;
            On_Player.AddBuff -= AddBuff;
            On_Player.QuickHeal_GetItemToUse -= QuickHeal_GetItemToUse;
            On_Projectile.AI_019_Spears_GetExtensionHitbox -= AI_019_Spears_GetExtensionHitbox;
            On_Item.AffixName -= AffixName;
            On_NPCUtils.TargetClosestBetsy -= TargetClosestBetsy;
            On_Main.MouseText_DrawItemTooltip_GetLinesInfo -= MouseText_DrawItemTooltip_GetLinesInfo;
            On_Player.ItemCheck_Shoot -= InterruptShoot;
            On_Main.DrawInterface_35_YouDied -= DrawInterface_35_YouDied;
        }


        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(CombinedHooks_ModifyHitNPCWithProj_Method, CombinedHooks_ModifyHitNPCWithProj);
        }

        private static bool LifeRevitalizer_CheckSpawn_Internal(
    On_Player.orig_CheckSpawn_Internal orig,
    int x, int y)
        {
            if (orig(x, y))
                return true;

            //Main.NewText($"{x} {y}");

            int revitalizerType = ModContent.TileType<LifeRevitalizerPlaced>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -3; j <= -1; j++)
                {
                    int newX = x + i;
                    int newY = y + j;

                    if (!WorldGen.InWorld(newX, newY))
                        return false;

                    Tile tile = Framing.GetTileSafely(newX, newY);
                    if (tile.TileType != revitalizerType)
                        return false;
                }
            }

            return true;
        }

        private void AddBuff(
            Terraria.On_Player.orig_AddBuff orig,
            Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            FargoSoulsPlayer modPlayer = self.FargoSouls();
            if (Main.debuff[type]
                && timeToAdd > 3 //dont affect auras
                && !Main.buffNoTimeDisplay[type] //dont affect hidden time debuffs
                && !BuffID.Sets.NurseCannotRemoveDebuff[type] //only affect debuffs that nurse can cleanse
                && (modPlayer.ParryDebuffImmuneTime > 0
                    || modPlayer.ImmuneToDamage
                    || modPlayer.ShellHide
                    || modPlayer.MonkDashing > 0
                    || modPlayer.CobaltImmuneTimer > 0
                    || modPlayer.TitaniumDRBuff)
                && DebuffIDs.Contains(type))
            {
                return; //doing it this way so that debuffs previously had are retained, but existing debuffs also cannot be extended by reapplying
            }

            orig(self, type, timeToAdd, quiet, foodHack);
        }

        private Item QuickHeal_GetItemToUse(On_Player.orig_QuickHeal_GetItemToUse orig, Player self)
        {
            Item value = orig(self);

            int num3 = 58;
            if (self.useVoidBag())
            {
                num3 = 98;
            }
            for (int i = 0; i < num3; i++)
            {
                Item item = ((i >= 58) ? self.bank4.item[i - 58] : self.inventory[i]);
                if (item.stack <= 0 || item.type <= ItemID.None || !item.potion || item.healLife <= 0)
                {
                    continue;
                }
                if (self.HasEffect<ShroomiteMushroomPriority>() && item.type == ItemID.Mushroom)
                    return item;
            }

            return value;
        }

        // Tungsten Enchant extended spear hitbox fix
        public static bool AI_019_Spears_GetExtensionHitbox(On_Projectile.orig_AI_019_Spears_GetExtensionHitbox orig, Projectile self, out Rectangle extensionBox)
        {
            bool ret = orig(self, out extensionBox);
            if (ret)
            {
                Vector2 dif = extensionBox.Center.ToVector2() - self.Center;
                Vector2 extra = dif * (self.scale - 1);
                extensionBox.Location += extra.ToPoint();
                extensionBox.Inflate(0, (int)(extensionBox.Height * 0.5f * (self.scale - 1)));
            }
            return ret;
        }

        public static string AffixName(On_Item.orig_AffixName orig, Item self)
        {
            string text = orig(self);
            if (self.ModItem != null && self.ModItem is SoulsItem soulsItem)
            {
                text = text.ArticlePrefixAdjustmentString(soulsItem.Articles.ToArray());
            }
            return text;
        }
        public static void TargetClosestBetsy(On_NPCUtils.orig_TargetClosestBetsy orig, NPC searcher, bool faceTarget = true, Vector2? checkPosition = null)
        {
            if (searcher.TypeAlive(NPCID.DD2Betsy) && searcher.TryGetGlobalNPC(out Betsy betsy) && betsy.TargetPlayer)
            {
                NPCUtils.TargetClosestCommon(searcher, faceTarget, checkPosition);
                return;
            }
            orig(searcher, faceTarget, checkPosition);
        }
        public static void MouseText_DrawItemTooltip_GetLinesInfo(On_Main.orig_MouseText_DrawItemTooltip_GetLinesInfo orig, Item item, ref int yoyoLogo, ref int researchLine, float oldKB, ref int numLines, string[] toolTipLine, bool[] preFixLine, bool[] badPreFixLine, string[] toolTipNames, out int prefixlineIndex)
        {
            DrawingTooltips = true;
            orig(item, ref yoyoLogo, ref researchLine, oldKB, ref numLines, toolTipLine, preFixLine, badPreFixLine, toolTipNames, out prefixlineIndex);
            DrawingTooltips = false;
        }
        public static void HorsemansBlade_SpawnPumpkin(On_Player.orig_HorsemansBlade_SpawnPumpkin orig, Player self, int npcIndex, int dmg, float kb)
        {
            NPC npc = Main.npc[npcIndex];
            if (npc.type is NPCID.GolemFistLeft or NPCID.GolemFistRight && WorldSavingSystem.EternityMode  && npc.TryGetGlobalNPC(out GolemFist golemFist) && golemFist.RunEmodeAI)
                return;
            orig(self, npcIndex, dmg, kb);
        }

        private void InterruptShoot(On_Player.orig_ItemCheck_Shoot orig, Player self, int i, Item sItem, int weaponDamage)
        {

            if (SwordGlobalItem.IsBroadsword(sItem) && sItem.TryGetGlobalItem<SwordGlobalItem>(out SwordGlobalItem sword) && !sword.VanillaShoot)
            {
                FargoSoulsPlayer mplayer = self.FargoSouls();

                mplayer.shouldShoot = true;
                return;
            }
            orig(self, i, sItem, weaponDamage);
        }
        public static void DrawInterface_35_YouDied(On_Main.orig_DrawInterface_35_YouDied orig)
        {
            orig();
            if (Main.LocalPlayer.dead && Main.LocalPlayer.Eternity().PreventRespawn())
            {
                float num = -60f;
                string value = Lang.inter[38].Value;
                //DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.DeathText.Value, value, new Vector2((float)(screenWidth / 2) - FontAssets.DeathText.Value.MeasureString(value).X / 2f, (float)(screenHeight / 2) + num), player[myPlayer].GetDeathAlpha(Microsoft.Xna.Framework.Color.Transparent), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                if (Main.LocalPlayer.lostCoins > 0)
                {
                    num += 50f;
                    //string textValue = Language.GetTextValue("Game.DroppedCoins", player[myPlayer].lostCoinString);
                    //DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.MouseText.Value, textValue, new Vector2((float)(screenWidth / 2) - FontAssets.MouseText.Value.MeasureString(textValue).X / 2f, (float)(screenHeight / 2) + num), player[myPlayer].GetDeathAlpha(Microsoft.Xna.Framework.Color.Transparent), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                }
                num += (float)((Main.LocalPlayer.lostCoins > 0) ? 24 : 50);
                num += 20f;
                float num2 = 0.7f;
                //string textValue2 = Language.GetTextValue("Game.RespawnInSuffix", ((float)(int)(1f + (float)player[myPlayer].respawnTimer / 60f)).ToString());
                //DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.DeathText.Value, textValue2, new Vector2((float)(screenWidth / 2) - FontAssets.MouseText.Value.MeasureString(textValue2).X * num2 / 2f, (float)(screenHeight / 2) + num), player[myPlayer].GetDeathAlpha(Microsoft.Xna.Framework.Color.Transparent), 0f, default(Vector2), num2, SpriteEffects.None, 0f);

                // draw our maso you can't respawn text
                num += 60;
                num2 = 0.5f;
                string text = Language.GetTextValue("Mods.FargowiltasSouls.UI.NoRespawn");
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, text, 
                    new Vector2((float)(Main.screenWidth / 2) - FontAssets.DeathText.Value.MeasureString(text).X * num2 / 2, (float)(Main.screenHeight / 2) + num), 
                    Main.LocalPlayer.GetDeathAlpha(Microsoft.Xna.Framework.Color.Transparent), 0f, default, num2, SpriteEffects.None, 0f);
            }
        }
        public static void CombinedHooks_ModifyHitNPCWithProj(Orig_CombinedHooks_ModifyHitNPCWithProj orig, Projectile projectile, NPC nPC, ref NPC.HitModifiers modifiers)
        {
            // Whip tag damage nerf
            if (WorldSavingSystem.EternityMode)
            {
                int tags = 0;
                for (int i = 0; i < nPC.buffType.Length; i++)
                {
                    int type = nPC.buffType[i];
                    if (BuffID.Sets.IsATagBuff[type])
                        tags++;
                }
                if (tags > 1)
                {
                    float perStack = 0.5f;
                    float mult = (1 - perStack + perStack * tags) / tags;
                    ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type] *= mult;
                    // IMPORTANT that this is reset after the hit!
                    // This is done in FargoSoulsPlayer.OnHitNPCWithProj using the following variable
                    projectile.FargoSouls().TagStackMultiplier = mult;
                }
            }
            orig(projectile, nPC, ref modifiers);
        }
    }
}
