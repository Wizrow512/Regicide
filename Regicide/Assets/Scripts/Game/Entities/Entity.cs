using Mirror;
using Regicide.Game.Player;
using System;
using System.Collections.Generic;

namespace Regicide.Game.Entities
{
    public abstract class Entity : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnEntityOwnerChange))]
        private int _playerId = -1;

        protected GamePlayer _playerOwner = null;

        public static Dictionary<uint, Entity> Entities { get; private set; } = new Dictionary<uint, Entity>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            Entities.Add(netId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            Entities.Add(netId, this);
        }

        protected virtual void OnDestroy()
        {
            Entities.Remove(netId);
        }

        protected virtual void OnEntityOwnerChange(int _, int playerId)
        {
            if (playerId >= 0 && GamePlayer.Players.TryGetValue((uint)playerId, out GamePlayer player))
            {
                _playerOwner = player;
            }
            else
            {
                _playerId = -1;
                _playerOwner = null;
            }
        }

        [Server]
        public void AssignEntityOwnership(GamePlayer owner)
        {
            if (owner != null)
            {
                netIdentity.AssignClientAuthority(owner.netIdentity.connectionToClient);
                _playerId = (int)owner.netId;
            }
        }

        [Server]
        public void RemoveEntityOwnership()
        {
            netIdentity.RemoveClientAuthority();
            _playerId = -1;
        }

        public bool HasOwner()
        {
            if (_playerOwner == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}