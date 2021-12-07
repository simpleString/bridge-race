using System.Collections.Generic;

namespace Store {

    public static class Store {
        public static int Coins;
        public static int Diamonds;
        public static GameManager.MyColor PlayerColor = GameManager.MyColor.blue;
        public static int CountOfPlayers;
        public static bool IsVibrationOn = true;
        public static bool IsSoundOn = true;
        public static List<UI.ColorShopScript> bougthColors = new List<UI.ColorShopScript>();



    }
}