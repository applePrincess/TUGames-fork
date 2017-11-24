//	#4 ゲーム内のマップ A map class 2017/11/23 T.Umezawa
using System;
using System.Drawing; // Bitmap, Graphics
class Map
{
    static readonly Bitmap	sBM = new Bitmap( "block.png" );

    public static byte[,]		sMap = new byte[ 16, 15 ];

    public static void GenerateMap(Random r)
    {
        for( int x = 0; x < sMap.GetLength( 1 ); x++ ){
            sMap[ sMap.GetLength( 0 ) - 1, x ] = 1;
        }

        byte   v = 1;
        int    n = 1;
        for( int y = 3; y <= 12; y += 3 ){
            for( int x = 0; x < sMap.GetLength( 1 ); x++, n-- ){
                if( n == 0 ){
                    v = (byte)( 1 - v );
                    n = r.Next( 2 ) + v + 1;
                }
                sMap[ y, x ] = v;
            }
        }
    }

    public static void draw( Graphics g )
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
