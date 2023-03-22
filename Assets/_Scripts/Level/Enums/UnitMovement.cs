
using System;

namespace Game2D
{
    [Flags]
    public enum UnitMovement : uint
    {
        Idle = 0,
        MoveRight = 1,
        MoveLeft = 2,
        Jump = 4,
        Falling = 8,
        Crawl = 16,
        Climb = 32,
        Attack = 64,
        MoveHorizontal = MoveRight | MoveLeft,
        JumpRight = Jump | MoveRight,
        JumpLeft = Jump | MoveLeft,
        FallRight = Falling | MoveRight,
        FallLeft = Falling | MoveLeft,
        CrawlRight = Crawl | MoveRight,
        CrawlLeft = Crawl | MoveLeft,
        ClimbUp = Climb | Jump,
        ClimbDown = Climb | Crawl,
        AttackRight = Attack | MoveRight,
        AttackLeft = Attack | MoveLeft,
        AttackHorizontal = AttackRight | AttackLeft
    }
}