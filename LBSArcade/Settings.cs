using IniParser;
using IniParser.Model;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LBSArcade
{
    internal static class Settings
    {
        private static string configPath = "./config.ini";
        private static FileIniDataParser parser = new FileIniDataParser();
        private static IniData data;

        private static Dictionary<string, string> uiDefaultSettings = new()
        {
            {"focusedGameScale","1.5"},
            {"gamesDirectory","./Games/"},
            {"gamesDirectoryDriveLetter","F"},
            {"fallbackGamesDirectory","./Games/"},
            {"spacing","0.95"},
            {"imageSize","222,296"},
            {"gameAnimationLerpDuration","0.1"},
            {"cursorAnimationLerpDuration", "0.1"},
            {"borderFadeSpeed","3"},
            {"selectedGamePosition","100,60"},
            {"justCorruptedTimerMax","2.5"},
            {"noGamesTextOffset","0,200"},
            {"errorColor","255,73,74"},
            {"borderOffset","5,37"},
            {"amountOfWaves","4"},
            {"scale", "1" },
            {"yPos", "750"},
            {"height", "300"},
            {"padding", "50"},
            {"pallete1", "57,90,96" },
            {"pallete2", "132,210,246" },
            {"pallete3", "89,165,216" },
            {"pallete4", "56,111,164" },
            {"pallete5", "19,60,85" },
            {"introLength", "7"},
            {"transitionLength", "2550" },
            {"konamiTimer", "400" },
            {"transitionAnimationLength", "3" },
            {"introBackgroundColor", "28,28,28" },
            {"screenSize", "1920,1080" },
            {"imageSize2", "300,400"},
            {"nameLength", "19"},
            {"bigLength", "50"},
            {"bigXOffset", "50"},
            {"bigYOffset", "64"},
            {"closeKey", "65" }
        };

        private static void VerifyConfigFile()
        {
            if (!File.Exists(configPath))
                File.Create(configPath).Close();
        }

        public static void WriteDefaults(string specificName = "")
        {
            IniData newData = new();

            if (specificName != "")
            {
                VerifyConfigFile();
                data ??= parser.ReadFile(configPath);

                specificName = char.ToLower(specificName[0]) + specificName[1..];

                data["settings"][specificName] = uiDefaultSettings[specificName];
                parser.WriteFile(configPath, data);
                return;
            }

            foreach (KeyValuePair<string, string> kvp in uiDefaultSettings)
                newData["settings"][kvp.Key] = kvp.Value;

            parser.WriteFile(configPath, newData);
        }

        public static Color GetColor(string name)
        {
            name = char.ToLower(name[0]) + name[1..];
            float[] rgb = GetData<float[]>(name);
            int r = (int)rgb[0];
            int g = (int)rgb[1];
            int b = (int)rgb[2];
            return new Color(r, g, b);
        }

        public static Vector2 GetVector2(string name)
        {
            name = char.ToLower(name[0]) + name[1..];
            float[] vec = GetData<float[]>(name);
            return new(vec[0], vec[1]);
        }
        public static T GetData<T>(string name)
        {
            try
            {
                VerifyConfigFile();
                data ??= parser.ReadFile(configPath);

                name = char.ToLower(name[0]) + name[1..];
                string value = data["settings"][name];

                if (value == null)
                    throw new NullReferenceException(nameof(value));

                if (typeof(T) == typeof(float[]))
                {
                    float[] split = value.Split(",").Select((num) => Convert.ToSingle(num)).ToArray();
                    return (T)Convert.ChangeType(split, typeof(T), CultureInfo.InvariantCulture); // this is stupid but can't think of a better way
                }

                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }

            catch (Exception e)
            {
                Logger.Error(e);
                Logger.Log($"Rewriting the key {name} in settings inside config since it errored");
                WriteDefaults(name);
                return GetData<T>(name);
            }
        }
    }
}
