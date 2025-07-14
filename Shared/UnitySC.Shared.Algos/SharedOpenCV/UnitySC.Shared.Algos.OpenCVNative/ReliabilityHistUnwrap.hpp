#pragma once

#include <opencv2/opencv.hpp>
#pragma unmanaged 

namespace phase_unwrapping {

    /*
     * Describes input parameters of phase unwrapping class.
     */
    struct UnwrapParams {
        UnwrapParams(int thresh, int nbrOfSmallBins, int nbrOfLargeBins) : HistThresh(float(thresh)), NbrOfSmallBins(nbrOfSmallBins), NbrOfLargeBins(nbrOfLargeBins) {};
        UnwrapParams() : HistThresh(static_cast<float>(3.0 * CV_PI * CV_PI)), NbrOfSmallBins(10), NbrOfLargeBins(5) {};

        float HistThresh;   // many experiments show that the edges tend to swamp in the first few subintervals so the threshold is set to 3*PI*PI as a default
        int NbrOfSmallBins; // number of bins before thresh [0,histThresh] (default value is 10)
        int NbrOfLargeBins; // number of bins after thresh [histThresh,32*pi*pi] (default value is 5)
    };

    class ReliabilityHistUnwrap {

    public:

        explicit ReliabilityHistUnwrap(const UnwrapParams& parameters = UnwrapParams()) : _params(parameters) {};

        virtual ~ReliabilityHistUnwrap() {};

        /**
         * Two-dimensional phase unwrapping based on quality-guided phase unwrapping method and using histogram processing
         * Reference: [Hai Lei, Xin-yu Chang, Fei Wang, Xiao-Tang Hu, and Xiao-Dong Hu] A novel algorithm based on histogram processing of reliability for two-dimensional phase unwrapping.
         *
         * @param wrappedPhaseMap     - The phase map of type CV_32FC1 wrapped between [-pi, pi]
         *
         * @return the unwrapped phase map, stored in CV_32FC1 Mat.
         */
        cv::Mat UnwrapPhaseMap(cv::Mat& wrappedPhaseMap, cv::Mat& mask);

        /*
         * Create a Mat that shows pixel inverse reliabilities
         */
        cv::Mat InverseReliabilityMap();

    private:

        /*
         * Describes a pixel with a quality quantization
         */
        struct Pixel {
            Pixel(float value, int id, bool valid, float invReliability, int inc) : PhaseValue(value), Index(id), Valid(valid), InverseReliability(invReliability), Increment(inc), Unwrapped(false), GroupId(id), SinglePixelGroup(true), NbPixelInGroup(1) {}

            float PhaseValue;         // value from the wrapped phase map
            int Index;                // used to store pixel position, computed from its position in the Mat (index = row * colsNb + column)
            bool Valid;               //
            float InverseReliability; // pixels reliability stored as inverse reliability (D=1/R) for the purpose of reducing computational cost. Values between 0 and 16*pi*pi
            int Increment;            // number of 2pi  that needs to be added to the pixel to unwrap the phase map
            bool Unwrapped;           // indicate if the pixel has already been unwrapped or not
            int GroupId;              // group id to which the pixel belongs. At first, group id is the same value as pixel id (pixel is alone in its group)
            bool SinglePixelGroup;    //
            int NbPixelInGroup;       //
        };

        /*
         * Describes a pixel map
         */
        struct PixelsMap {
            std::vector<Pixel> Pixels; // use just one vector to store pixels map and split the pixel id in several variables, access like vector[(row * columns) + column].
            int Height;                // rows number of the pixels map
            int Width;                 // columns number of the pixels map
        };

        /*
         * Describes an edge defined by two pixels that are connected horizontally or vertically.
         */
        struct Edge {
            Edge(int firstPixelId, int secondPixelId, int increment) : FirstPixelId(firstPixelId), SecondPixelId(secondPixelId), Increment(increment) {}

            int FirstPixelId;  // Id of the first pixel that forms the edge
            int SecondPixelId; // Id of the second pixel that forms the edge
            int Increment;     // Number of 2pi that needs to be added to the second pixel to remove discontinuities
        };

        /*
         * Describes a bin from the histogram
         */
        struct Bin {
            Bin(float start, float end) : Start(start), End(end) {};

            float Start;
            float End;
            std::vector<Edge> Edges;
        };

        /*
         * Describes an histogram
         */
        struct Histogram {
            std::vector<Bin> Bins;
            float Thresh;
            float SmallBinsWidth;
            float LargeBinsWidth;
            int SmallBinsNb;
            int LargeBinsNb;
        };

        UnwrapParams _params; // Params for phase unwrapping
        PixelsMap _pixelsMap; // Pixels from the wrapped phase map
        Histogram _histogram; // Histogram used to unwrap

        /*
         * Compute the pixels map with reliability values of each pixel, from the wrapped phase map.
         * The reliability value R of a pixel can be calculated by the values of second difference D in the equation R = 1/D.
         * Pixels are more reliable as their reliability values R are higher which means their second differences values D are lower.
         *
         * @param wrappedPhaseMap      - 2D wrapped phase map
         * @param shadowMask           - mask invalid pixels
         */
        void PixelsReliability(cv::Mat& wrappedPhaseMap, cv::Mat& shadowMask);

        /*
         * Compute edge reliability as a histogram
         */
        void EdgesReliability();

        /*
         * Simple modulo 2PI operation to remove any 2PI steps between two consecutive pixels
         *
         * @param p1       - value of the first pixel
         * @param p2       - value of the second pixel
         *
         * @return Difference between the first and the second pixel, after removing any 2PI steps betweent them
         */
        float Wrap(float a, float b);

        /*
         * Create histogram of bins, which divided the interval of second differences values of edge [0, 32*PI*PI] into a number of subintervals.
         *
         * @return The histogram created
         */
        void CreateHistogram();

        /*
         * Create an edge
         *
         * @param idx1          - index in pixels vector of the first pixel of the edge
         * @param idx2          - index in pixels vector of the second pixel of the edge
         *
         * @return The edge created
         */
        Edge CreateEdge(int idx1, int idx2);

        /*
         * Add edge in appropriate bin of histogram
         *
         * @param edge        - edge to add into histogram
         */
        void AddEdgeInHist(Edge& edge);

        /*
         * Unwrap the phase map thanks to the histogram
         */
        void UnwrapHistogram();

        /*
         * Unwrap the phase by adding the appropriate 2 * PI increment at each pixel
         */
        cv::Mat ComputePixelsIncrement();
    };
} // namespace phase_unwrapping