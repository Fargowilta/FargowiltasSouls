using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public abstract class EModeNPCBehaviour : GlobalNPC
    {
        public NPCMatcher Matcher;

        public override bool InstancePerEntity => true;

        //gameMenu check so that npcloot actually populates
        //TODO: find better way to make behaviour actually emode only but npcloot populate properly
        public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            if (Matcher.Satisfies(entity.type))
            {
                TryLoadSprites(entity);

                return FargoSoulsWorld.EternityMode || Main.gameMenu;
            }
            return false;
        }

        public override void Load()
        {
            Matcher = CreateMatcher();
            base.Load();
        }

        public abstract NPCMatcher CreateMatcher();
        /// <summary>
        /// Override this and return a new instance of your EModeNPCBehaviour subclass if you have any reference-type variables
        /// </summary>
        public virtual EModeNPCBehaviour NewInstance() => (EModeNPCBehaviour)MemberwiseClone();


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


        public virtual void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit) { }

        public virtual void SafeModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) { }
        public sealed override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            SafeModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
            ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);
        }

        public virtual void SafeModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) { }
        public sealed override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            base.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);

            if (!FargoSoulsWorld.EternityMode)
                return;

            SafeModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            ModifyHitByAnything(npc, Main.player[projectile.owner], ref damage, ref knockback, ref crit);
        }


        public virtual void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit) { }

        public virtual void SafeOnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) { }
        public sealed override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            base.OnHitByItem(npc, player, item, damage, knockback, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            SafeOnHitByItem(npc, player, item, damage, knockback, crit);
            ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);
        }

        public virtual void SafeOnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) { }
        public sealed override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            base.OnHitByProjectile(npc, projectile, damage, knockback, crit);

            if (!FargoSoulsWorld.EternityMode)
                return;

            SafeOnHitByProjectile(npc, projectile, damage, knockback, crit);
            ModifyHitByAnything(npc, Main.player[projectile.owner], ref damage, ref knockback, ref crit);
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
                bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.EternityMode;
                if (recolor || FargowiltasSouls.Instance.LoadedNewSprites)
                {
                    FargowiltasSouls.Instance.LoadedNewSprites = true;
                    LoadSprites(npc, recolor);
                }
            }
        }

        public virtual void LoadSprites(NPC npc, bool recolor) { }

        #region Sprite Loading
        protected static Asset<Texture2D> LoadSprite(bool recolor, string texture)
            => ModContent.Request<Texture2D>("FargowiltasSouls/NPCs/" + (recolor ? "Resprites/" : "Vanilla/") + texture, AssetRequestMode.ImmediateLoad);

        protected static void LoadSpriteBuffered(bool recolor, int type, Asset<Texture2D>[] vanillaTexture, Dictionary<int, Asset<Texture2D>> fargoBuffer, string texturePrefix)
        {
            if (recolor)
            {
                if (!fargoBuffer.ContainsKey(type))
                {
                    fargoBuffer[type] = vanillaTexture[type];
                    vanillaTexture[type] = LoadSprite(recolor, $"{texturePrefix}{type}");
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
                    vanillaResource = LoadSprite(recolor, name);
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
