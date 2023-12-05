using _Game.Scripts.ModelV4;

namespace _Game.Scripts.BurnMark.Game {
    public class Map {
        private readonly GameDataReadAPI _readAPI;
        private readonly GameDataEventsAPI _eventsAPI;

        public Map(GameDataReadAPI readAPI, GameDataEventsAPI eventsAPI) {
            _readAPI = readAPI;
            _eventsAPI = eventsAPI;
        }
    }
}