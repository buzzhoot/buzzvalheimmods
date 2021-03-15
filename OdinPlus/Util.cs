using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace OdinPlus
{
    class Util
    {
        public static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
        private static Sprite LoadCustomTexture(String image)
        {
            Sprite cartography;
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(path, image);
            Texture2D temp = LoadTexture(filepath);
            cartography = Sprite.Create(temp, new Rect(0, 0, 32, 32), Vector2.zero);
            return cartography;
        }

        private static Texture2D LoadTexture(string filepath)
        {
            bool flag = cachedTextures.ContainsKey(filepath);
            Texture2D result;
            if (flag)
            {
                result = cachedTextures[filepath];
            }
            else
            {
                Texture2D texture2D = new Texture2D(0, 0);
                texture2D.LoadRawTextureData(File.ReadAllBytes(filepath));
                result = texture2D;
            }
            return result;
        }

    }
}
