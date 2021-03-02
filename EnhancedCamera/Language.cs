using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using CustomCamera.langs;

namespace CustomCamera {
    class Language {
        public static string get(string key) {
            if (MainMenu.langData != null) {
                if (MainMenu.langData.ContainsKey(key)) {
                    return (string)MainMenu.langData[key];
                }
            }
            return key;
        }

    }
}
