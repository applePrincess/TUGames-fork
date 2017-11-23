//	#4 ジャンプアクション３ Jump Action3 2017/11/23 T.Umezawa

using System;
using System.Collections.Generic;

class JumpAct3 : MyForm
{
    public enum Scene
    {
        Title,
        Stage,
        GameClear,
        GameOver
    }
    public static Random      sRnd = new Random();

    System.Drawing.Font       mFont = new System.Drawing.Font( "MS Gothic", 4 );
    int                       mCount;
    List<Player>              mLPlayer;
    List<Enemy>               mLEnemy;
    public static bool        sStageClear, sGameOver;
    int                       mStage = 1;
    Scene                     mScene = Scene.Title;

    SortedList<Scene, Action<System.Drawing.Graphics>> drawList
        = new SortedList<Scene, Action<System.Drawing.Graphics>>();
    SortedList<Scene, Action<int, bool>> inputTrigger
        = new SortedList<Scene, Action<int, bool>>();

    protected override void OnLoad( EventArgs e )
    {
        base.OnLoad( e );
        mTimer.Interval = 25;
        mTimer.Start();
        inputTrigger.Add(Scene.Title, TitleInput);
        inputTrigger.Add(Scene.Stage, StageInput);
        drawList.Add(Scene.Title, TitleDraw);
        drawList.Add(Scene.Stage, StageDraw);
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
        g.DrawString( "ジャンプアクション３ Jump Action3", mFont, mSBWhite, 15, 20 );
        g.DrawString( "PRESS ANY KEY", mFont, mSBWhite, 40, 40 );
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

        g.DrawString( "TIME " + mCount, mFont, mSBWhite, 0, 0 );
        g.DrawString( "STAGE " + mStage, mFont, mSBWhite, 40, 0 );
        if( sStageClear ){
            g.DrawString( "STAGE CLEAR!", mFont, mSBWhite, 40, 40 );
        }

        if( sGameOver ){
            g.DrawString( "GAME OVER", mFont, mSBWhite, 40, 40 );
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
            for( int i = mLEnemy.Count - 1; i >= 0; i-- ){
                if( pl.isCollision( mLEnemy[ i ] ) ){
                    if( pl.getAngle4i( mLEnemy[ i ] ) == 1 ){
                        mLEnemy.RemoveAt( i );
                        pl.jump( -4.0f / 16 );
                    }else{
                        sGameOver = true;
                    }
                }
            }
        }

        for( int i = mLEnemy.Count - 1; i >= 0; i-- ){
            Enemy	en = mLEnemy[ i ];
            en.step( mLEnemy );
        }

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

        for( int x = 0; x < Map.sMap.GetLength( 1 ); x++ ){
            Map.sMap[ Map.sMap.GetLength( 0 ) - 1, x ] = 1;
        }

        byte   v = 1;
        int    n = 1;
        for( int y = 3; y <= 12; y += 3 ){
            for( int x = 0; x < Map.sMap.GetLength( 1 ); x++, n-- ){
                if( n == 0 ){
                    v = (byte)( 1 - v );
                    n = sRnd.Next( 2 ) + v + 1;
                }
                Map.sMap[ y, x ] = v;
            }
        }
    }

    [STAThread]
    static void Main()
    {
        System.Windows.Forms.Application.Run( new JumpAct3() );
    }
}
