using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.Globals
{
    public abstract class PillarBehaviour : GlobalNPC
    {
        public NPCMatcher Matcher;

        public override bool InstancePerEntity => true;

        public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (entity.type != NPCID.LunarTowerNebula && entity.type != NPCID.LunarTowerSolar && entity.type != NPCID.LunarTowerStardust && entity.type != NPCID.LunarTowerVortex)
            {
                return false;
            }
            return lateInstantiation && Matcher.Satisfies(entity.type);
        }

        public override void Load()
        {
            Matcher = CreateMatcher();
            base.Load();
        }

        public abstract NPCMatcher CreateMatcher();

        // This entire file is a bandaid fix for the new AppliesToEntity and NewInstance system. Put an Eternity Mode check in every overriden method instead.
        public override GlobalNPC NewInstance(NPC target)
        {
            TryLoadSprites(target);
            return Matcher.Satisfies(target.type) ? base.NewInstance(target) : null;
        }

        public bool FirstTick = true;
        public virtual void OnFirstTick(NPC npc) { }

        public virtual bool SafePreAI(NPC npc) => base.PreAI(npc);
        public sealed override bool PreAI(NPC npc)
        {
            if (FirstTick)
            {
                FirstTick = false;

                OnFirstTick(npc);
            }

            return SafePreAI(npc);
        }


        public virtual void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers) { }

        public virtual void SafeModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) { }
        public sealed override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByItem(npc, player, item, ref modifiers);

            if (!WorldSavingSystem.EternityMode)
                return;

            SafeModifyHitByItem(npc, player, item, ref modifiers);
            ModifyHitByAnything(npc, player, ref modifiers);
        }

        public virtual void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) { }
        public sealed override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);

            if (!WorldSavingSystem.EternityMode)
                return;

            SafeModifyHitByProjectile(npc, projectile, ref modifiers);
            ModifyHitByAnything(npc, Main.player[projectile.owner], ref modifiers);
        }


        public virtual void OnHitByAnything(NPC npc, Player player, NPC.HitInfo hit, int damageDone) { }

        public virtual void SafeOnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) { }
        public sealed override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(npc, player, item, hit, damageDone);

            if (!WorldSavingSystem.EternityMode)
                return;

            SafeOnHitByItem(npc, player, item, hit, damageDone);
            // ModifyHitByAnything(npc, player, hit);
        }

        public virtual void SafeOnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) { }
        public sealed override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(npc, projectile, hit, damageDone);

            if (!WorldSavingSystem.EternityMode)
                return;

            SafeOnHitByProjectile(npc, projectile, hit, damageDone);
            // ModifyHitByAnything(npc, Main.player[projectile.owner], ref damage, ref knockback, ref crit);
        }


        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            //npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync(npc);
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        /// <summary>
        /// Checks if loading sprites is necessary and does it if so.
        /// </summary>
        public void TryLoadSprites(NPC npc)
        {
            if (!Main.dedServ)
            {
                bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
                if (recolor || FargowiltasSouls.Instance.LoadedNewSprites)
                {
                    FargowiltasSouls.Instance.LoadedNewSprites = true;
                    LoadSprites(npc, recolor);
                }
            }
        }

        public virtual void LoadSprites(NPC npc, bool recolor) { }

        #region Sprite Loading
        protected static Asset<Texture2D> LoadSprite(string texture)
            => ModContent.Request<Texture2D>("FargowiltasSouls/Assets/ExtraTextures/Resprites/" + texture, AssetRequestMode.ImmediateLoad);

        protected static void LoadSpriteBuffered(bool recolor, int type, Asset<Texture2D>[] vanillaTexture, Dictionary<int, Asset<Texture2D>> fargoBuffer, string texturePrefix)
        {
            if (recolor)
            {
                if (!fargoBuffer.ContainsKey(type))
                {
                    fargoBuffer[type] = vanillaTexture[type];
                    vanillaTexture[type] = LoadSprite($"{texturePrefix}{type}");
                }
            }
            else
            {
                if (fargoBuffer.ContainsKey(type))
                {
                    vanillaTexture[type] = fargoBuffer[type];
                    fargoBuffer.Remove(type);
                }
            }
        }

        protected static void LoadSpecial(bool recolor, ref Asset<Texture2D> vanillaResource, ref Asset<Texture2D> fargoSoulsBuffer, string name)
        {
            if (recolor)
            {
                if (fargoSoulsBuffer == null)
                {
                    fargoSoulsBuffer = vanillaResource;
                    vanillaResource = LoadSprite(name);
                }
            }
            else
            {
                if (fargoSoulsBuffer != null)
                {
                    vanillaResource = fargoSoulsBuffer;
                    fargoSoulsBuffer = null;
                }
            }
        }

        protected static void LoadNPCSprite(bool recolor, int type)
        {
            LoadSpriteBuffered(recolor, type, TextureAssets.Npc, FargowiltasSouls.TextureBuffer.NPC, "NPC_");
        }

        protected static void LoadBossHeadSprite(bool recolor, int type)
        {
            LoadSpriteBuffered(recolor, type, TextureAssets.NpcHeadBoss, FargowiltasSouls.TextureBuffer.NPCHeadBoss, "NPC_Head_Boss_");
        }

        protected static void LoadGore(bool recolor, int type)
        {
            LoadSpriteBuffered(recolor, type, TextureAssets.Gore, FargowiltasSouls.TextureBuffer.Gore, "Gores/Gore_");
        }

        protected static void LoadGoreRange(bool recolor, int type, int lastType)
        {
            for (int i = type; i <= lastType; i++)
                LoadGore(recolor, i);
        }

        protected static void LoadExtra(bool recolor, int type)
        {
            LoadSpriteBuffered(recolor, type, TextureAssets.Extra, FargowiltasSouls.TextureBuffer.Extra, "Extra_");
        }

        protected static void LoadGolem(bool recolor, int type)
        {
            LoadSpriteBuffered(recolor, type, TextureAssets.Golem, FargowiltasSouls.TextureBuffer.Golem, "GolemLights");
        }
        #endregion
    }
}
