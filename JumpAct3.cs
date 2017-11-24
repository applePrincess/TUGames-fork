//	#4 ジャンプアクション３ Jump Action3 2017/11/23 T.Umezawa

using System;
using System.Collections.Generic;
using System.Linq;
using DrawFunc  = System.Action<System.Drawing.Graphics>;
using InputFunc = System.Action<int, bool>;

class JumpAct3 : MyForm
{
    public enum Scene
    {
        Title,
        Stage,
        StageClear,
        GameOver
    }

    public static AssetController               sAssets;

    public static readonly Random               sRnd = new Random();

    public static readonly System.Drawing.Font mFont = new System.Drawing.Font( "MS Gothic", 4 );
    int                       mCount;
    List<Player>              mLPlayer;
    List<Enemy>               mLEnemy;
    public static bool        sStageClear, sGameOver;
    int                       mStage = 1;
    Scene                     mScene = Scene.Title;

    SortedList<Scene, DrawFunc> drawList = new SortedList<Scene, DrawFunc>();
    SortedList<Scene, InputFunc> inputTrigger = new SortedList<Scene, InputFunc>();

    protected override void OnLoad( EventArgs e )
    {
        base.OnLoad( e );
        // DEBUG: will be initialized in some other way.
        SortedList<string, System.Drawing.Bitmap> imgs = new SortedList<string, System.Drawing.Bitmap>();
        imgs.Add("player1", new System.Drawing.Bitmap("player.png"));
        imgs.Add("player2", new System.Drawing.Bitmap("player2.png"));
        imgs.Add("enemy",   new System.Drawing.Bitmap( "monster.png" ));
        sAssets = new AssetController(new System.Drawing.Font( "MS Gothic", 4 ), imgs);
        mTimer.Interval = 25;
        mTimer.Start();
        inputTrigger.Add(Scene.Title, TitleInput);
        inputTrigger.Add(Scene.Stage, StageInput);
        drawList.Add(Scene.Title, TitleDraw);
        drawList.Add(Scene.Stage, StageDraw);
        drawList.Add(Scene.StageClear, StageClearDraw);
        drawList.Add(Scene.GameOver, GameOverDraw);
    }

    protected override void OnKeyDown( System.Windows.Forms.KeyEventArgs e )
    {
        input( 1, e.KeyCode == System.Windows.Forms.Keys.R );
        base.OnKeyDown( e );
    }

    protected override void OnMouseDown( System.Windows.Forms.MouseEventArgs e )
    {
        input( 0, e.Button == System.Windows.Forms.MouseButtons.Right );
        base.OnMouseDown( e );
    }

    protected void TitleDraw( System.Drawing.Graphics g )
    {
        g.DrawString( sAssets.GetStringAt("title1"), sAssets.DefaultFont, mSBWhite, 15, 20 );
        g.DrawString( sAssets.GetStringAt("title2"), sAssets.DefaultFont, mSBWhite, 40, 40 );
    }

    protected void StageClearDraw( System.Drawing.Graphics g )
    {
        g.DrawString(sAssets.GetStringAt("stageclear"), sAssets.DefaultFont, mSBWhite, 40, 40 );
    }

    protected void GameOverDraw( System.Drawing.Graphics g )
    {
            g.DrawString( sAssets.GetStringAt("gameover"), sAssets.DefaultFont, mSBWhite, 40, 40 );
    }

    protected void StageDraw( System.Drawing.Graphics g )
    {
        g.TranslateTransform( 0, -50 + mCount / 16.0f );
        Map.draw( g );
        foreach( Player pl in mLPlayer ){
            pl.draw( g );
        }
        foreach( Enemy en in mLEnemy ){
            en.draw( g );
        }

        g.TranslateTransform( 0, 50 - mCount / 16.0f );

        g.DrawString( sAssets.GetStringAt("time") + mCount, sAssets.DefaultFont, mSBWhite, 0, 0 );
        g.DrawString( sAssets.GetStringAt("stage") + mStage, sAssets.DefaultFont, mSBWhite, 40, 0 );
        if( sStageClear ){
            g.DrawString( sAssets.GetStringAt("stageclear"), sAssets.DefaultFont, mSBWhite, 40, 40 );
        }

        if( sGameOver ){
            g.DrawString( sAssets.GetStringAt("gameover"), sAssets.DefaultFont, mSBWhite, 40, 40 );
        }
    }

    protected override void onMyPaint( System.Drawing.Graphics g )
    {
        drawList[mScene](g);
    }

    protected override void onMyTimer( object sender, System.Timers.ElapsedEventArgs e )
    {
        if( sStageClear || sGameOver ){
            return;
        }

        mCount++;

        foreach( Player pl in mLPlayer ){
            pl.step();
            mLEnemy = mLEnemy.Aggregate(new List<Enemy>(), (lst, en) => {
                        if(!pl.isCollision(en))  lst.Add(en);
                        else
                            if( pl.getAngle4i( en ) == 1 )  pl.jump( -4.0f / 16 );
                            else sGameOver = true;
                        return lst;
                });
        }

        mLEnemy.ForEach(en => en.step(mLEnemy));
        Invalidate();
    }

    void TitleInput(int type, bool res)
    {
        mStage = 1;
        start();
        mLPlayer.Add( new Player( type ) );
    }

    void StageInput(int type, bool res)
    {
        if( sStageClear ){
            mStage++;
            start();
            mLPlayer.Add( new Player( type ) );
        }else if( res ){
            mStage = 1;
            start();
            mLPlayer.Add( new Player( type ) );
        }else if( mLPlayer[ 0 ].mType != type ){
            if( mLPlayer.Count == 1 ){
                mLPlayer.Add( new Player( type ) );
            }else{
                mLPlayer[ 1 ].jump();
            }
        }else{
            mLPlayer[ 0 ].jump();
        }
    }

    void input( int type, bool res )
    {
        inputTrigger[mScene](type, res);
    }

    void start()
    {
        mScene = Scene.Stage;
        sStageClear = false;
        sGameOver = false;
        mCount = 0;
        mLPlayer = new List<Player>();
        mLEnemy = new List<Enemy>();
        for( int i = 0; i < mStage; i++ ){
            mLEnemy.Add( new Enemy() );
        }

        Map.GenerateMap(sRnd);
    }

    [STAThread]
    static void Main()
    {
        System.Windows.Forms.Application.Run( new JumpAct3() );
    }
}
