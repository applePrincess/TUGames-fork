using System.Collections.Generic;
using System.Drawing;

public class AssetController
{
    public readonly SortedList<string, Font> mFonts; // accessneed to be fixed.
    public readonly SortedList<string, Bitmap> mBM; //   ditto

    public Font DefaultFont
    {
        get
        {
            return mFonts["default"];
        }
    }

    public readonly SortedList<string, string> mStringForDraw = new SortedList<string, string>();

    public AssetController(string assetDir = "Assets", string propertyFile="Resources.xml")
    {
        if(!System.IO.Directory.Exists(assetDir) ||
           !System.IO.File.Exists(System.IO.Path.Combine(assetDir, propertyFile)))
            throw new System.Exception("File is not found.");
        System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(System.IO.Path.Combine(assetDir, propertyFile));
        mFonts = new SortedList<string, Font>();
        mBM    = new SortedList<string, Bitmap>();
        while(reader.Read())
        {
            if(reader.NodeType == System.Xml.XmlNodeType.Element)
            {
                switch(reader.LocalName)
                {
                    case "string":
                        mStringForDraw.Add(reader.GetAttribute("id"), reader.ReadString());
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
                            if(src == null || !System.IO.File.Exists(System.IO.Path.Combine(assetDir, src)))
                                throw new System.Exception("No file is specified");
                            mBM.Add(id, new Bitmap(System.IO.Path.Combine(assetDir, src)));
                            break;
                        }
                }
            }
        }
    }

    public string GetStringAt(string key)
    {
        return mStringForDraw[key];
    }
}
