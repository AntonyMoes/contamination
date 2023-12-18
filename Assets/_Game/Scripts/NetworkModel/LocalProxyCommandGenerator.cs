using System.Collections.Generic;
using GeneralUtils;

namespace _Game.Scripts.NetworkModel {
    public class LocalProxyCommandGenerator {
        private readonly Dictionary<int, ProxyCommandGenerator> _generators = new Dictionary<int, ProxyCommandGenerator>();

        public ProxyCommandGenerator Get(int playerId) =>
            _generators.GetValue(playerId, () => new ProxyCommandGenerator());
    }
}