//	#4 汎用的なユニットを表すクラス A class for generic unit 2017/11/23 T.Umezawa
using System;
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
