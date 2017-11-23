using System;
using System.Collections.Generic; // List
using System.Drawing; // Bitmap, Graphics

class Enemy : Unit
{
    static Bitmap	sBM = new Bitmap( "monster.png" );

    public Enemy() : base( JumpAct3.sRnd.Next( 13 ) + 1, 0.5f, JumpAct3.sRnd.Next( 2 ) / 16.0f - 1.0f / 32, 0 )
    {
    }

    public void draw( Graphics g )
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
