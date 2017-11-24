using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.IO;

public class AssetController
{
    public readonly SortedList<string, Font> mFonts;
    public readonly SortedList<string, Bitmap> mBM;

    public Font DefaultFont
    {
        get
        {
            return mFonts["default"];
        }
    }

    public readonly SortedList<string, string> mStringForDraw = new SortedList<string, string>();
    public AssetController(SortedList<string, Font> fonts, SortedList<string, Bitmap> bitmaps)
    {
        mFonts = fonts;
        mBM = bitmaps;

        // DEBUG: will be removed
        mStringForDraw.Add("title1","ジャンプアクション３ Jump Action3");
        mStringForDraw.Add("title2","PRESS ANY KEY");
        mStringForDraw.Add("stageclear","STAGE CLEAR");
        mStringForDraw.Add("gameover","GAME OVER");
        mStringForDraw.Add("time","TIME");
        mStringForDraw.Add("stage","STAGE");
        System.Console.WriteLine("mFonts = {0}\n mBM = {1}\nmStringForDraw = {2}"
                                 , mFonts, mBM, mStringForDraw);
    }

    public AssetController(string assetDir = "Assets", string propertyFile="Resources.xml")
    {
        if(!Directory.Exists(assetDir) || !File.Exists(Path.Combine(assetDir, propertyFile)))
            throw new System.Exception("File is not found.");
        XmlTextReader reader = new XmlTextReader(Path.Combine(assetDir, propertyFile));
        mFonts = new SortedList<string, Font>();
        mBM    = new SortedList<string, Bitmap>();
        while(reader.Read())
        {
            if(reader.NodeType == XmlNodeType.Element)
            {
                switch(reader.LocalName)
                {
                    case "string":
                        //System.Console.WriteLine(reader.Value.ToString());
                        mStringForDraw.Add(reader.GetAttribute("id"), reader.ReadString());
                        // do something;
                        break;
                    case "font":
                        {
                            string id = reader.GetAttribute("id");
                            if (id == null) id = "default";
                            string fs = reader.GetAttribute("size");
                            string family = reader.GetAttribute("family");
                            if(family == null) family = "Aerial";
                            float fontSize;
                            bool suceed = float.TryParse(fs, out fontSize);
                            if(!suceed) fontSize = 10.0f;
                            mFonts.Add(id, new Font(family, fontSize));
                            break;
                        }
                    case "image":
                        {
                            string id = reader.GetAttribute("id");
                            if (id == null) id = "default";
                            string src = reader.GetAttribute("src");
                            if(src == null || !File.Exists(Path.Combine(assetDir, src)))
                                throw new System.Exception("No file is specified");
                            mBM.Add(id, new Bitmap(src));
                            break;
                        }
                }
            }
        }
        System.Console.WriteLine("{0}, {1}, {2}", mStringForDraw.Count, mFonts.Count, mBM.Count);
    }

    public AssetController(Font font, SortedList<string, Bitmap> bitmaps)
    {
        mFonts = new SortedList<string, Font>();
        mFonts.Add("default", font);
        mBM = bitmaps;

        // DEBUG: will be removed
        mStringForDraw.Add("title1","ジャンプアクション３ Jump Action3");
        mStringForDraw.Add("title2","PRESS ANY KEY");
        mStringForDraw.Add("stageclear","STAGE CLEAR");
        mStringForDraw.Add("gameover","GAME OVER");
        mStringForDraw.Add("time","TIME");
        mStringForDraw.Add("stage","STAGE");

    }

    public string GetStringAt(string key)
    {
        return mStringForDraw[key];
    }
}
