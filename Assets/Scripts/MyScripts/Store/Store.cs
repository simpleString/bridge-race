using System;
using System.Collections.Generic;
using UnityEngine;

namespace Store {

    public static class Store {
        static GameManager.MyColor _playerColor;
        public static GameManager.MyColor PlayerColor {
            set {
                PlayerPrefs.SetInt("PlayerColor", (int)value);
                _playerColor = value;
            }
            get => _playerColor;
        }
        static bool _isVibrationOn;
        public static bool IsVibrationOn {
            set {
                PlayerPrefs.SetInt("IsVibrationOn", Convert.ToInt32(value));
                _isVibrationOn = value;
            }
            get => _isVibrationOn;
        }
        static bool _isSoundOn;
        public static bool IsSoundOn {
            set {
                PlayerPrefs.SetInt("IsSoundOn", Convert.ToInt32(value));
                _isSoundOn = value;
            }
            get => _isSoundOn;
        }

        public static void Init() {
            PlayerColor = (GameManager.MyColor)PlayerPrefs.GetInt("PlayerColor", (int)GameManager.MyColor.blue);
            _isSoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("IsSoundOn", 1));
            _isVibrationOn = Convert.ToBoolean(PlayerPrefs.GetInt("IsVibrationOn", 1));
            _currentLevel = PlayerPrefs.GetString("CurrentLevel", "Level_1");
        }

        public static string MaxLevel = "Level_3";
        static string _currentLevel;
        public static string CurrentLevel {
            set {
                PlayerPrefs.SetString("CurrentLevel", value);
                _currentLevel = value;
            }
            get => _currentLevel;
        }



    }
}