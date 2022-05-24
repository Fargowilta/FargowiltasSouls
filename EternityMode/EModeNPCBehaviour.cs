using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public abstract class EModeNPCBehaviour
    {
        internal static Mod mod => FargowiltasSouls.Instance;

        public static List<EModeNPCBehaviour> AllEModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public NPCMatcher Matcher;

        public void Register()
        {
            AllEModeNpcBehaviours.Add(this);
        }

        public void NetSend(BinaryWriter writer)
        {
            Dictionary<Ref<object>, CompoundStrategy> netInfo = GetNetInfo();
            if (netInfo == default)
                return;

            for (int i = 0; i < netInfo.Count; i++)
            {
                KeyValuePair<Ref<object>, CompoundStrategy> query = netInfo.ElementAt(i);
                query.Value.Send(query.Key.Value, writer);
            }
        }

        public void NetRecieve(BinaryReader reader)
        {
            Dictionary<Ref<object>, CompoundStrategy> netInfo = GetNetInfo();
            if (netInfo == default)
                return;

            for (int i = 0; i < netInfo.Count; i++)
            {
                KeyValuePair<Ref<object>, CompoundStrategy> query = netInfo.ElementAt(i);
                query.Value.Recieve(ref query.Key.Value, reader);
            }
        }


        public virtual void Load()
        {
            Matcher = CreateMatcher();
            Register();
        }

        public static void Unload()
        {
            AllEModeNpcBehaviours.Clear();
        }

        public virtual Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() => default;

        public int GetBytesNeeded()
        {
            int byteLength = 0;

            Dictionary<Ref<object>, CompoundStrategy> netInfo = GetNetInfo();
            if (netInfo == default)
                return byteLength;

            foreach (CompoundStrategy strategy in netInfo.Values)
            {
                if (strategy.Equals(BoolStrategies.CompoundStrategy))
                    byteLength += 1;
                else if (strategy.Equals(IntStrategies.CompoundStrategy))
                    byteLength += 4;
                else if (strategy.Equals(FloatStrategies.CompoundStrategy))
                    byteLength += 4;
                else
                    FargowiltasSouls.Instance.Logger.Warn("didn't recognize strategy!");
            }

            return byteLength;
        }

        public abstract NPCMatcher CreateMatcher();
        /// <summary>
        /// Override this and return a new instance of your EModeNPCBehaviour subclass if you have any reference-type variables
        /// </summary>
        public virtual EModeNPCBehaviour NewInstance() => (EModeNPCBehaviour)MemberwiseClone();

        public virtual void SetDefaults(NPC npc) { }

        public virtual void OnSpawn(NPC npc, IEntitySource source) { }

        public virtual bool PreAI(NPC npc) => true;

        public virtual void OnFirstTick(NPC npc) { }

        public virtual void AI(NPC npc) { }

        public virtual void PostAI(NPC npc) { }

        /// <summary>
        /// ModifyNPCLoot runs before entering a world. Make sure drops are properly wrapped in the EMode drop condition!
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="npcLoot"></param>
        public virtual void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) { }

        public virtual void OnKill(NPC npc) { }

        public virtual bool SpecialOnKill(NPC npc) => false;

        public virtual bool CanHitPlayer(NPC npc, Player target, ref int CooldownSlot) => true;

        public virtual void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit) { }

        public virtual void OnHitPlayer(NPC npc, Player target, int damage, bool crit) { }

        public virtual void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit) { }

        public virtual void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit) { }

        public virtual void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) => ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

        public virtual void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) => ModifyHitByAnything(npc, Main.player[projectile.owner], ref damage, ref knockback, ref crit);

        public virtual void OnHitByAnything(NPC npc, Player player, int damage, float knockback, bool crit) { }

        public virtual void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) => OnHitByAnything(npc, player, damage, knockback, crit);

        public virtual void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) => OnHitByAnything(npc, Main.player[projectile.owner], damage, knockback, crit);

        public virtual bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) => true;

        public virtual void HitEffect(NPC npc, int hitDirection, double damage) { }

        public virtual void UpdateLifeRegen(NPC npc, ref int damage) { }

        public virtual bool CheckDead(NPC npc) => true;

        public virtual Color? GetAlpha(NPC npc, Color drawColor) => null;

        public virtual bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position) => null;

        public virtual bool? CanBeHitByItem(NPC npc, Player player, Item item) => null;

        public virtual bool? CanBeHitByProjectile(NPC npc, Projectile projectile) => null;

        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            npc.GetGlobalNPC<NewEModeGlobalNPC>().NetSync(npc);
        }

        public virtual void LoadSprites(NPC npc, bool recolor) { }

        #region Sprite Loading
        protected static Asset<Texture2D> LoadSprite(bool recolor, string texture)
        {
            return ModContent.Request<Texture2D>("FargowiltasSouls/NPCs/" + (recolor ? "Resprites/" : "Vanilla/") + texture, AssetRequestMode.ImmediateLoad);
        }

        protected static void LoadSpriteBuffered(bool recolor, int type, Asset<Texture2D>[] vanillaTexture, Dictionary<int, Asset<Texture2D>> fargoBuffer, string texturePrefix)
        {
            if (!fargoBuffer.ContainsKey(type))
                fargoBuffer[type] = vanillaTexture[type];

            vanillaTexture[type] = LoadSprite(recolor, $"{texturePrefix}{type}");
        }

        protected static void LoadSpecial(bool recolor, ref Asset<Texture2D> vanillaResource, ref Asset<Texture2D> fargoSoulsBuffer, string name)
        {
            if (fargoSoulsBuffer == null)
                fargoSoulsBuffer = vanillaResource;

            vanillaResource = LoadSprite(recolor, name);
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
