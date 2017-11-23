//	#4 ジャンプアクション３ Jump Action3 2017/11/23 T.Umezawa

using System;
using System.Collections.Generic;


class Util
{
    public static int GetAngle4i( double dx, double dy )
    {
        return( (int)( ( GetAngle360( dx, dy ) + 45 ) / 90 ) & 0x03 );
    }

    public static double GetAngle360( double dx, double dy )
    {
        double	r = Math.Atan2( dy, dx ) * 180 / Math.PI;
        if( r < 0 ){
            r += 360;
        }
        return( r );
    }
}


class Map
{
    static readonly System.Drawing.Bitmap	sBM = new System.Drawing.Bitmap( "block.png" );

    public static byte[,]		sMap = new byte[ 16, 15 ];

    public static void draw( System.Drawing.Graphics g )
    {
        for( int y = 0; y < sMap.GetLength( 0 ); y++ ){
            for( int x = 0; x < sMap.GetLength( 1 ); x++ ){
                if( sMap[ y, x ] != 0 ){
                    g.DrawImage( sBM, x * 8, y * 8 );
                }
            }
        }
    }

    public static bool IsArea( int x, int y )
    {
        return( x >= 0 && x < sMap.GetLength( 1 ) &&
                y >= 0 && y < sMap.GetLength( 0 ) );
    }

    public static bool IsBlock( float x, float y )
    {
        x -= 0.5f;
        y -= 0.5f;

        int		x0 = (int)Math.Floor( x );
        int		y0 = (int)Math.Floor( y );
        int		x1 = (int)Math.Ceiling( x );
        int		y1 = (int)Math.Ceiling( y );
        return( IsBlock( x0, y0 ) ||
                IsBlock( x1, y0 ) ||
                IsBlock( x0, y1 ) ||
                IsBlock( x1, y1 ) );
    }

    public static bool IsBlock( int x, int y )
    {
        return( !IsArea( x, y ) || sMap[ y, x ] != 0 );
    }
}


class Unit
{
    public static readonly int		DOT = 8;
    public static readonly int		DTH = DOT / 2;

    protected float	mX, mY;
    protected float	mDX, mDY;
    protected bool	mGround = true;

    public Unit( float x, float y, float dx, float dy )
    {
        mX = x;
        mY = y;
        mDX = dx;
        mDY = dy;
    }

    public int getAngle4i( Unit u )
    {
        return( Util.GetAngle4i( u.mX - mX, u.mY - mY ) );
    }

    public bool isBlock()
    {
        return( Map.IsBlock( mX, mY ) );
    }

    public void jump( float dy )
    {
        mGround = false;
        mDY = dy;
    }

    public void mightChangeDirection(Unit u)
    {
        if( u == this || !isCollision( u ) ){
            return;
        }
        mDX = Math.Sign( mX - u.mX ) / 32.0f;
        if( mDX == 0 ){
            mDX = 1.0f / 32;
        }
        u.mDX = -mDX;
    }

    public virtual void step()
    {
        mX += mDX;

        if( isBlock() ){
            mDX = -mDX;
            mX += mDX;
        }

        mY += mDY;

        if( !mGround && mDY < 6.0f / DOT ){	//	重力
            mDY += 1.0f / 32;
        }

        if( mDY < 0 && isBlock() ){			//	上昇中
            if( !Map.IsBlock( mX + 1.0f / 16, mY ) ){
                mX += 1.0f / 16;
            }else if( !Map.IsBlock( mX - 1.0f / 16, mY ) ){
                mX -= 1.0f / 16;
            }else{							//	天井にぶつかる
                mDY = 0;
                mY = (int)( mY - 0.5f ) + 1.5f;
            }
        }

        if( mDY > 0 && isBlock() ){			//	下降中
            mDY = 0;
            mY = (int)( mY - 0.5f ) + 0.5f;
            mGround = true;
        }

        if( mGround && !Map.IsBlock( mX, mY + 1.0f / DOT ) ){	//	地面から落ちる
            mGround = false;
            mY += 1.0f / DOT;
            mDY = 1.0f / 16;
        }
    }

    public bool isCollision( Unit u )
    {
        return( Math.Abs( mX - u.mX ) < 6.0f / DOT && Math.Abs( mY - u.mY ) < 6.0f / DOT );
    }
}


class Player : Unit
{
    static System.Drawing.Bitmap[]	sBM = {
        new System.Drawing.Bitmap( "player.png" ),
        new System.Drawing.Bitmap( "player2.png" )
    };
    static System.Drawing.Rectangle	sRect = new System.Drawing.Rectangle( 0, 0, 8, 8 );

    public int		mType;

    public Player( int type ) : base( 0.5f, 14.5f, 1.0f / 16, 0 )
    {
        mType = type;
    }

    public void draw( System.Drawing.Graphics g )
    {
        sRect.X = ( (int)( mX * 8 ) & 1 ) * DOT;
        sRect.Y = Math.Sign( mDX ) * DTH + DTH;
        g.DrawImage( sBM[ mType ], mX * DOT - DTH, mY * DOT - DTH, sRect, System.Drawing.GraphicsUnit.Pixel );
    }

    public void jump()
    {
        if( mGround ){			//	接地中の場合
            jump( -7.0f / 16 );
        }
    }

    public override void step()
    {
        base.step();

        if( mX <= 0.5f || mX >= 14.5f ){
            mDX = Math.Sign( Map.sMap.GetLength( 1 ) / 2 - mX ) / 16.0f;
        }

        if( mY < 1 ){
            JumpAct3.sStageClear = true;
        }
    }
}


class Enemy : Unit
{
    static System.Drawing.Bitmap	sBM = new System.Drawing.Bitmap( "monster.png" );

    public Enemy() : base( JumpAct3.sRnd.Next( 13 ) + 1, 0.5f, JumpAct3.sRnd.Next( 2 ) / 16.0f - 1.0f / 32, 0 )
    {
    }

    public void draw( System.Drawing.Graphics g )
    {
        g.DrawImage( sBM, mX * DOT - DTH, mY * DOT - DTH );
    }

    public void step( List<Enemy> le )
    {
        step();

        if( mX <= 0.5f || mX >= 14.5f ){
            mDX = Math.Sign( Map.sMap.GetLength( 1 ) / 2 - mX ) / 32.0f;
        }

        le.ForEach( en => mightChangeDirection(en));
    }
}


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

    protected override void OnLoad( EventArgs e )
    {
        base.OnLoad( e );
        mTimer.Interval = 25;
        mTimer.Start();
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

    void input( int type, bool res )
    {
        if( mScene == Scene.Title ){
            mStage = 1;
            start();
            mLPlayer.Add( new Player( type ) );
        }else if( sStageClear ){
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
