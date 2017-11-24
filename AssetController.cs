using System.Collections.Generic;
using System.Drawing;
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
