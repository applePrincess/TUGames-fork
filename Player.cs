using System;
using System.Drawing; // Bitmap, Graphics,GraphicsUnit, Rectangle
class Player : Unit
{
    static Bitmap[]	sBM = { new Bitmap( "player.png" ), new Bitmap( "player2.png" ) };
    static Rectangle	sRect = new Rectangle( 0, 0, 8, 8 );

    public int		mType;

    public Player( int type ) : base( 0.5f, 14.5f, 1.0f / 16, 0 )
    {
        mType = type;
    }

    public void draw( Graphics g )
    {
        sRect.X = ( (int)( mX * 8 ) & 1 ) * DOT;
        sRect.Y = Math.Sign( mDX ) * DTH + DTH;
        g.DrawImage( sBM[ mType ], mX * DOT - DTH, mY * DOT - DTH, sRect, GraphicsUnit.Pixel );
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
