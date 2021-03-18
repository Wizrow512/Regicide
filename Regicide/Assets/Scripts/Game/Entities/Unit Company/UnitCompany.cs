
using System.Collections.Generic;
using UnityEngine;

namespace Regicide.Game.Entities
{
    public class UnitCompany : Entity
    {
        [SerializeField] protected County county = null;
        public County County { get => county; }

        public static Dictionary<uint, UnitCompany> UnitCompanies { get; private set; } = new Dictionary<uint, UnitCompany>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            UnitCompanies.Add(netId, this);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (isServer) { return; }
            UnitCompanies.Add(netId, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnitCompanies.Remove(netId);
        }
    }
}