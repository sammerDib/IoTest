#pragma once

#pragma unmanaged
namespace metroCD
{
    enum class MStatus : int
    {
        COMPUTE_ERROR = -3,
        UNKNOWN_ERROR = -2,
        INIT_ERROR = -1,
        NOT_INITIALIZE = 0,
        INITIALIZED = 1,
        IDLE = 2,
        COMPUTED = 3,
        ABORTED = 4,
    };

    enum class MType : int
    {
        NotDefined = 0,
        Seeker = 1,
        Circle = 2,
        Ellipse = 3,
        Line = 4,
    };

    // check coherence with Wrapper MetroCDDrawFlag
    enum class MetroDrawFlag : uint // 32 bit word
    {
        DrawEmpty,
        DrawSeekers = 1 << 0,    //   1
        DrawDetection = 1 << 1,    //   2
        DrawSkipDetection = 1 << 2,    //   4
        DrawFit = 1 << 3,    //   8
        DrawCenterFit = 1 << 4,    //  16
        /*
        //reserved
        Flag6                   = 1 << 5,    //  32
        Flag7                   = 1 << 6,    //  64
        Flag8                   = 1 << 7,    // 128
        Flag9                   = 1 << 8,    // 256
        Flag10                  = 1 << 9,    // 512
        Flag11                  = 1 << 10,   //1024
        Flag12                  = 1 << 11,   //2048
        Flag13                  = 1 << 12,   //4096
        Flag14                  = 1 << 13,   //8192
        Flag15                  = 1 << 14,   //16384
        Flag16                  = 1 << 15,   //32768
        // WORD
        Flag17      = 1 << 16,
        Flag18      = 1 << 17,
        Flag19      = 1 << 18,
        Flag20      = 1 << 19,
        // //...
        Flag32      = 1 << 31,
        */
    }; 
    const uint DefaultMetroDrawFlags = (uint)MetroDrawFlag::DrawDetection | (uint)MetroDrawFlag::DrawFit;
    const uint AllMetroDrawFlags = 0xFFFFFFFF;
    static bool HasDrawFlag(uint flags, MetroDrawFlag drawflag)
    {
        // & is AND bitwise operator
        return ((flags & (uint)drawflag) == (uint)drawflag);
    }
}