using UnityEngine;

/// <summary>
/// 
/// </summary>
public struct PlayerOrientation
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation Up = new PlayerOrientation(0, 1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation UpLeft = new PlayerOrientation(-1, 1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation UpRight = new PlayerOrientation(1, 1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation Down = new PlayerOrientation(0, -1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation DownLeft = new PlayerOrientation(-1, -1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation DownRight = new PlayerOrientation(1, -1);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation Left = new PlayerOrientation(-1, 0);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation Right = new PlayerOrientation(1, 0);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation None = new PlayerOrientation(0, 0);

    /// <summary>
    /// 
    /// </summary>
    public static readonly PlayerOrientation[] All = { Up, UpLeft, UpRight, Down, DownLeft, DownRight, Left, Right, None };

    public static PlayerOrientation GetOrientationFromConfig(PlayerOrientationConfig l_config)
    {
        switch(l_config)
        {
            case PlayerOrientationConfig.UP: return Up;
            case PlayerOrientationConfig.UP_LEFT: return UpLeft;
            case PlayerOrientationConfig.UP_RIGHT: return UpRight;
            case PlayerOrientationConfig.DOWN: return Down;
            case PlayerOrientationConfig.DOWN_LEFT: return DownLeft;
            case PlayerOrientationConfig.DOWN_RIGHT: return DownRight;
            case PlayerOrientationConfig.LEFT: return Left;
            case PlayerOrientationConfig.RIGHT: return Right;
            default:
                return None;
        }
    }

    public static PlayerOrientationConfig GetConfigFromOrientation(Vector2 l_orientation)
    {
        for(int i = 0; i < All.Length; i++) 
        {
            PlayerOrientation l_iter = All[i];
            if (l_orientation == l_iter.GetLookDir())
            {
                return (PlayerOrientationConfig)i;
            }
        }
        return PlayerOrientationConfig.NONE;
    }

    public static PlayerOrientationConfig GetConfigFromOrientation(PlayerOrientation l_orientation)
    {
        return GetConfigFromOrientation(l_orientation.GetLookDir());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_lookDir"></param>
    public PlayerOrientation(Vector2 p_lookDir)
    {
        m_lookDirection = p_lookDir;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_x"></param>
    /// <param name="p_y"></param>
    public PlayerOrientation(float p_x, float p_y)
    {
        m_lookDirection = new Vector2(p_x, p_y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector2 GetLookDir() {  return m_lookDirection; }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Vector2 m_lookDirection;
}