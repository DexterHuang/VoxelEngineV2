using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class WebHandler : MonoBehaviour {
    public static void getTextures(List<Texture> textures, string search, int amount = -1, bool transparent = false) {
        textures.Clear();
        getTexturesc(textures, search, amount, transparent);
    }
    public static void getTexturesc(List<Texture> textures, string search, int amount, bool transparent) {
        search = search.Replace(" ", "+");
        int width = 32;
        int height = 32;
        string transparentString = "";
        if (transparent) {
            transparentString = ",ic:trans";
        }
        string url = "http://images.google.com/search?q=" + search + "Texture+&newwindow=1&sout=1&tbm=isch&oq=" + search + "+&gs_l=img.3..0l10.3386.3774.0.4952.2.2.0.0.0.0.39.75.2.2.0....0...1ac.1.34.img..0.2.75.WJ6ZqESrkN8&tbs=isz:ex,iszw:" + width + ",iszh:" + height + transparentString;
        //string url = "https://www.google.com.sg/search?q=" + search + "Texture+&newwindow=1&biw=1920&bih=950&tbm=isch&source=lnt&tbs=isz:ex,iszw:32,iszh:32#q=" + search + "Texture+&newwindow=1&tbs=isz:ex,iszw:" + width + ",iszh:" + height + transparentString;
        WWW www = new WWW(url);
        int count = 0;
        while (www.isDone == false && count < 100) {
            Thread.Sleep(200);
        }
        Match match = Regex.Match(www.text, " src=\"http:\\google.com.tw\" width=\""); //" src =\"(.*)\" width = \"");
        string text = www.text;
        List<string> matchList = new List<string>();
        string startString = "<img height=\"" + height + "\" src=\"";
        string endString = "\" width=\"" + width + "\" alt";
        //string startString = "\"ou\":\"";
        //string endString = "\",\"ow\":\"" + width + "\"";
        while (text.Contains(startString) && text.Contains(endString) && count <= 100) {
            int start = text.IndexOf(startString) + startString.Length;
            int end = text.IndexOf(endString);
            string str = text.Substring(start, end - start);
            text = text.Replace(startString + str + endString, "");
            matchList.Add(str);
            count++;
        }
        Debug.Log(www.text);
        Debug.Log("there are: " + matchList.Count);

        List<Texture> textureList = new List<Texture>();
        if (amount <= 0) {
            amount = matchList.Count;
        }
        for (int i = 1; i <= amount; i++) {
            WWW w = new WWW(matchList[i - 1]);
            count = 0;
            while (w.isDone == false && count < 100) {
                Thread.Sleep(200);
            }
            Texture t = w.texture;
            t.filterMode = FilterMode.Point;
            textures.Add(t);
        }
        //foreach (string str in matchList) {
        //   WWW w = new WWW(str);
        //    while (w.isDone == false && count <= 100) {
        //        Thread.Sleep(200);
        //        count++;
        //    }
        //    textureList.Add(w.texture);
        //}
    }
}