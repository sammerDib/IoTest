#include <Logger.hpp>
#include <Topography/ReliabilityHistUnwrap.hpp>

using namespace std;
using namespace cv;

namespace phase_unwrapping {

    Mat ReliabilityHistUnwrap::UnwrapPhaseMap(Mat& wrappedPhaseMap, Mat& mask) {
        if (mask.empty()) {
            mask.create(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_8UC1);
            mask = Scalar::all(255);
        }

        if (wrappedPhaseMap.type() != CV_32FC1) {
            stringstream strStrm;
            strStrm << "[Reliability histogram unwrap] Input phase map should be stored into a single channel 32-bits float.";
            string message = strStrm.str();
            Logger::Error(message);
            return Mat();
        }

        if (mask.type() != CV_8UC1) {
            stringstream strStrm;
            strStrm << "[Reliability histogram unwrap] Input mask should be stored into a single channel 8-bits unsigned char.";
            string message = strStrm.str();
            Logger::Error(message);
            return Mat();
        }

        double minMax = CV_PI + 0.01;
        bool validRangeValues = checkRange(wrappedPhaseMap, true, 0, -minMax, minMax) && !checkRange(wrappedPhaseMap, true, 0, 0, 1);
        if (!validRangeValues) {
            stringstream strStrm;
            strStrm << "[Reliability histogram unwrap] Input phase map should be wrapped between [-pi, pi]";
            string message = strStrm.str();
            Logger::Error(message);
            return Mat();
        }

        // First, computes a reliability map (store as 1D vector) from second differences between a pixel and its eight neighbours.
        PixelsReliability(wrappedPhaseMap, mask);

        // Then, this reliability map is used to compute the reliabilities of "edges", sorted in a histogram based on their reliability values.
        EdgesReliability();

        // This histogram is then used to unwrap pixels, starting from the highest quality pixel and continues to the lower quality ones until it finishes.
        UnwrapHistogram();

        Mat unwrappedPhaseMap = ComputePixelsIncrement();

        return unwrappedPhaseMap;
    }

    void ReliabilityHistUnwrap::PixelsReliability(Mat& wrappedPhaseMap, Mat& mask) {
        int height = wrappedPhaseMap.rows;
        int width = wrappedPhaseMap.cols;
        _pixelsMap.Height = height;
        _pixelsMap.Width = width;

        // To calculate the reliability value for a pixel in an image, 8-neighbors is required
        Point upLeft, upMiddle, upRight, middleLeft, middleRight, lowLeft, lowMiddle, lowRight;

        for (int i = 0; i < height; ++i) {
            for (int j = 0; j < width; ++j) {
                int idx = i * width + j;
                if (mask.at<uchar>(i, j) != 0) {
                    // Pixel is in a valid region
                    bool valid = true;
                    if (i == 0 || i == height - 1 || j == 0 || j == width - 1) {
                        // reliability values of the pixels at the borders of the image are set to maximum 16*PI^2
                        Pixel p(wrappedPhaseMap.at<float>(i, j), idx, valid, static_cast<float>(16 * CV_PI * CV_PI), 0);
                        _pixelsMap.Pixels.push_back(p);
                    }
                    else {
                        Mat neighbourhood = mask(Rect(j - 1, i - 1, 3, 3));
                        Scalar meanValue = mean(neighbourhood);
                        if (meanValue[0] != 255) {
                            // one of the neighbouring pixel is not valid -> pixel(i, j) is considered as being on the border (inverse reliability is set to the 16*PI^2)
                            Pixel p(wrappedPhaseMap.at<float>(i, j), idx, valid, static_cast<float>(16 * CV_PI * CV_PI), 0);
                            _pixelsMap.Pixels.push_back(p);
                        }
                        else {
                            // reliability values of the other pixels are calculated with neighbors and second differences
                            upLeft = Point(j - 1, i - 1);
                            upMiddle = Point(j, i - 1);
                            upRight = Point(j + 1, i - 1);
                            middleLeft = Point(j - 1, i);
                            middleRight = Point(j + 1, i);
                            lowLeft = Point(j - 1, i + 1);
                            lowMiddle = Point(j, i + 1);
                            lowRight = Point(j + 1, i + 1);

                            // Horizontal difference :
                            float H = Wrap(wrappedPhaseMap.at<float>(middleLeft.y, middleLeft.x), wrappedPhaseMap.at<float>(i, j)) - Wrap(wrappedPhaseMap.at<float>(i, j), wrappedPhaseMap.at<float>(middleRight.y, middleRight.x));
                            // Vertical difference :
                            float V = Wrap(wrappedPhaseMap.at<float>(upMiddle.y, upMiddle.x), wrappedPhaseMap.at<float>(i, j)) - Wrap(wrappedPhaseMap.at<float>(i, j), wrappedPhaseMap.at<float>(lowMiddle.y, lowMiddle.x));
                            // Diagonal difference 1 :
                            float D1 = Wrap(wrappedPhaseMap.at<float>(upLeft.y, upLeft.x), wrappedPhaseMap.at<float>(i, j)) - Wrap(wrappedPhaseMap.at<float>(i, j), wrappedPhaseMap.at<float>(lowRight.y, lowRight.x));
                            // Diagonal difference 2 :
                            float D2 = Wrap(wrappedPhaseMap.at<float>(upRight.y, upRight.x), wrappedPhaseMap.at<float>(i, j)) - Wrap(wrappedPhaseMap.at<float>(i, j), wrappedPhaseMap.at<float>(lowLeft.y, lowLeft.x));
                            // Calculate the second difference (this definition of second difference reducing the computational cost) :
                            float D = H * H + V * V + D1 * D1 + D2 * D2; // float R = 1/D;

                            Pixel p(wrappedPhaseMap.at<float>(i, j), idx, valid, D, 0);
                            _pixelsMap.Pixels.push_back(p);
                        }
                    }
                }
                else {
                    // pixel is not in a valid region -> it's inverse reliability is set to the 16*PI^2
                    bool valid = false;
                    Pixel p(wrappedPhaseMap.at<float>(i, j), idx, valid, static_cast<float>(16 * CV_PI * CV_PI), 0);
                    _pixelsMap.Pixels.push_back(p);
                }
            }
        }
    }

    void ReliabilityHistUnwrap::EdgesReliability() {
        // Create histogram
        CreateHistogram();

        // create edges by considering a pixel and it's right-neighbour and down-neighbour, and add them in histogram
        int width = _pixelsMap.Width;
        int height = _pixelsMap.Height;

        for (int i = 0; i < _pixelsMap.Pixels.size(); ++i) {
            int row = _pixelsMap.Pixels.at(i).Index / width;
            int col = _pixelsMap.Pixels.at(i).Index % width;

            int rightId, downId;
            if (row != height - 1) {
                downId = (row + 1) * width + col; // Index of pixel under pixel i.
                Edge edge = CreateEdge(i, downId);
                AddEdgeInHist(edge);
            }
            if (col != width - 1) {
                rightId = row * width + col + 1; // Index of pixel to the right
                Edge edge = CreateEdge(i, rightId);
                AddEdgeInHist(edge);
            }
        }
    }

    float ReliabilityHistUnwrap::Wrap(float p1, float p2) {
        float PI = static_cast<float>(CV_PI);
        float grad = p1 - p2;
        if (grad > PI)
            grad -= 2 * PI;
        else if (grad < -PI)
            grad += 2 * PI;
        return grad;
    }

    void ReliabilityHistUnwrap::CreateHistogram() {
        // Many experiments show that the number of edges with hight second differences values is few, compared to the total number of edges.
        // In order to divide the second differences value range accurately but not increase the number of subintervals dramatically,
        // the proposed algorithm divides the second differences value range in which values are less than a threshold into narrow subintervals,
        // and values greater than the threshold into relatively wide subintervals.

        _histogram.Thresh = _params.HistThresh;
        _histogram.SmallBinsNb = _params.NbrOfSmallBins;                                                                                      // number of bins before thresh
        _histogram.LargeBinsNb = _params.NbrOfLargeBins;                                                                                      // number of bins after thresh
        _histogram.SmallBinsWidth = _params.HistThresh / _params.NbrOfSmallBins;                                                              // bins before "thresh" are smaller
        _histogram.LargeBinsWidth = static_cast<float>(32 * CV_PI * CV_PI - _params.HistThresh) / static_cast<float>(_params.NbrOfLargeBins); // bins after "thresh" are bigger

        for (int i = 0; i < _histogram.SmallBinsNb; ++i) {
            _histogram.Bins.push_back(Bin(i * _histogram.SmallBinsWidth, (i + 1) * _histogram.SmallBinsWidth));
        }
        for (int i = 0; i < _histogram.LargeBinsNb; ++i) {
            _histogram.Bins.push_back(Bin(_histogram.Thresh + i * _histogram.LargeBinsWidth, _histogram.Thresh + (i + 1) * _histogram.LargeBinsWidth));
        }
    }

    ReliabilityHistUnwrap::Edge ReliabilityHistUnwrap::CreateEdge(int idx1, int idx2) {
        // compute the wrap value (number of 2pi that needs to be added to the second pixel to remove discontinuities)
        float PI = static_cast<float>(CV_PI);
        float grad = _pixelsMap.Pixels.at(idx1).PhaseValue - _pixelsMap.Pixels.at(idx2).PhaseValue;
        int wrapValue = 0;
        if (grad > PI)
            wrapValue = -1;
        else if (grad < -PI)
            wrapValue = 1;

        // create edge
        Edge e(idx1, idx2, wrapValue);
        return e;
    }

    void ReliabilityHistUnwrap::AddEdgeInHist(Edge& edge) {
        if (_pixelsMap.Pixels.at(edge.SecondPixelId).Valid) {
            // Edge is stored into the corresponding subintervals in  the _histogram
            float edgeReliability = _pixelsMap.Pixels.at(edge.FirstPixelId).InverseReliability + _pixelsMap.Pixels.at(edge.SecondPixelId).InverseReliability;
            int binId = 0;
            if (edgeReliability < _histogram.Thresh) {
                binId = static_cast<int>(ceil(edgeReliability / _histogram.SmallBinsWidth) - 1);
                binId = (binId == -1) ? 0 : binId;
            }
            else {
                binId = _histogram.SmallBinsNb + static_cast<int>(ceil((edgeReliability - _histogram.Thresh) / _histogram.LargeBinsWidth)) - 1;
            }

            _histogram.Bins.at(binId).Edges.push_back(edge);
        }
    }

    void ReliabilityHistUnwrap::UnwrapHistogram() {
        int nbrOfPixels = static_cast<int>(_pixelsMap.Pixels.size());
        int nbrOfBins = _histogram.LargeBinsNb + _histogram.SmallBinsNb;
        /* This vector is used to keep track of the number of pixels in each group and avoid useless group.
           For example, if lastPixelAddedToGroup[10] is equal to 5, it means that pixel "5" was the last one
           to be added to group 10. So, pixel "5" is the only one that has the correct value for parameter
           "numberOfPixelsInGroup" in order to avoid a loop on all the pixels to update this number*/
        vector<int> lastPixelAddedToGroup(nbrOfPixels, 0);
        for (int i = 0; i < nbrOfBins; ++i) {
            vector<Edge> currentEdges = _histogram.Bins.at(i).Edges;
            int nbrOfEdgesInBin = static_cast<int>(currentEdges.size());

            for (int j = 0; j < nbrOfEdgesInBin; ++j) {
                int pOneId = currentEdges[j].FirstPixelId;
                int pTwoId = currentEdges[j].SecondPixelId;
                // Both pixels are in a single group.
                if (_pixelsMap.Pixels[pOneId].SinglePixelGroup && _pixelsMap.Pixels[pTwoId].SinglePixelGroup) {
                    float invRel1 = _pixelsMap.Pixels[pOneId].InverseReliability;
                    float invRel2 = _pixelsMap.Pixels[pTwoId].InverseReliability;
                    // Quality of pixel 2 is better than that of pixel 1 -> pixel 1 is added to group 2
                    if (invRel1 > invRel2) {
                        int newGroupId = _pixelsMap.Pixels[pTwoId].GroupId;
                        int newInc = _pixelsMap.Pixels[pTwoId].Increment + currentEdges[j].Increment;
                        _pixelsMap.Pixels[pOneId].GroupId = newGroupId;
                        _pixelsMap.Pixels[pOneId].Increment = newInc;
                        lastPixelAddedToGroup[newGroupId] = pOneId; // Pixel 1 is the last one to be added to group 2
                    }
                    else {
                        int newGroupId = _pixelsMap.Pixels[pOneId].GroupId;
                        int newInc = _pixelsMap.Pixels[pOneId].Increment - currentEdges[j].Increment;
                        _pixelsMap.Pixels[pTwoId].GroupId = newGroupId;
                        _pixelsMap.Pixels[pTwoId].Increment = newInc;
                        lastPixelAddedToGroup[newGroupId] = pTwoId;
                    }
                    _pixelsMap.Pixels[pOneId].NbPixelInGroup = 2;
                    _pixelsMap.Pixels[pTwoId].NbPixelInGroup = 2;
                    _pixelsMap.Pixels[pOneId].SinglePixelGroup = false;
                    _pixelsMap.Pixels[pTwoId].SinglePixelGroup = false;
                }
                // p1 is in a single group, p2 is not -> p1 added to p2
                else if (_pixelsMap.Pixels[pOneId].SinglePixelGroup && !_pixelsMap.Pixels[pTwoId].SinglePixelGroup) {
                    int newGroupId = _pixelsMap.Pixels[pTwoId].GroupId;
                    int lastPix = lastPixelAddedToGroup[newGroupId];
                    int newNbrOfPixelsInGroup = _pixelsMap.Pixels[lastPix].NbPixelInGroup + 1;
                    int newInc = _pixelsMap.Pixels[pTwoId].Increment + currentEdges[j].Increment;

                    _pixelsMap.Pixels[pOneId].GroupId = newGroupId;
                    _pixelsMap.Pixels[pOneId].NbPixelInGroup = newNbrOfPixelsInGroup;
                    _pixelsMap.Pixels[pTwoId].NbPixelInGroup = newNbrOfPixelsInGroup;
                    _pixelsMap.Pixels[pOneId].Increment = newInc;
                    _pixelsMap.Pixels[pOneId].SinglePixelGroup = false;

                    lastPixelAddedToGroup[newGroupId] = pOneId;
                }
                // p2 is in a single group, p1 is not -> p2 added to p1
                else if (!_pixelsMap.Pixels[pOneId].SinglePixelGroup && _pixelsMap.Pixels[pTwoId].SinglePixelGroup) {
                    int newGroupId = _pixelsMap.Pixels[pOneId].GroupId;
                    int lastPix = lastPixelAddedToGroup[newGroupId];
                    int newNbrOfPixelsInGroup = _pixelsMap.Pixels[lastPix].NbPixelInGroup + 1;
                    int newInc = _pixelsMap.Pixels[pOneId].Increment - currentEdges[j].Increment;

                    _pixelsMap.Pixels[pTwoId].GroupId = newGroupId;
                    _pixelsMap.Pixels[pTwoId].NbPixelInGroup = newNbrOfPixelsInGroup;
                    _pixelsMap.Pixels[pOneId].NbPixelInGroup = newNbrOfPixelsInGroup;
                    _pixelsMap.Pixels[pTwoId].Increment = newInc;
                    _pixelsMap.Pixels[pTwoId].SinglePixelGroup = false;

                    lastPixelAddedToGroup[newGroupId] = pTwoId;
                }
                // p1 and p2 are in two different groups
                else if (_pixelsMap.Pixels[pOneId].GroupId != _pixelsMap.Pixels[pTwoId].GroupId) {
                    int pOneGroupId = _pixelsMap.Pixels[pOneId].GroupId;
                    int pTwoGroupId = _pixelsMap.Pixels[pTwoId].GroupId;

                    float invRel1 = _pixelsMap.Pixels[pOneId].InverseReliability;
                    float invRel2 = _pixelsMap.Pixels[pTwoId].InverseReliability;

                    int lastAddedToGroupOne = lastPixelAddedToGroup[pOneGroupId];
                    int lastAddedToGroupTwo = lastPixelAddedToGroup[pTwoGroupId];

                    int nbrOfPixelsInGroupOne = _pixelsMap.Pixels[lastAddedToGroupOne].NbPixelInGroup;
                    int nbrOfPixelsInGroupTwo = _pixelsMap.Pixels[lastAddedToGroupTwo].NbPixelInGroup;

                    int totalNbrOfPixels = nbrOfPixelsInGroupOne + nbrOfPixelsInGroupTwo;

                    if (nbrOfPixelsInGroupOne < nbrOfPixelsInGroupTwo || (nbrOfPixelsInGroupOne == nbrOfPixelsInGroupTwo && invRel1 >= invRel2)) // group p1 added to group p2
                    {
                        _pixelsMap.Pixels[pTwoId].NbPixelInGroup = totalNbrOfPixels;
                        _pixelsMap.Pixels[pOneId].NbPixelInGroup = totalNbrOfPixels;
                        int inc = _pixelsMap.Pixels[pTwoId].Increment + currentEdges[j].Increment - _pixelsMap.Pixels[pOneId].Increment;
                        lastPixelAddedToGroup[pTwoGroupId] = pOneId;

                        for (int k = 0; k < nbrOfPixels; ++k) {
                            if (_pixelsMap.Pixels[k].GroupId == pOneGroupId) {
                                _pixelsMap.Pixels[k].GroupId = pTwoGroupId;
                                _pixelsMap.Pixels[k].Increment = inc;
                            }
                        }
                    }
                    else if (nbrOfPixelsInGroupOne > nbrOfPixelsInGroupTwo || (nbrOfPixelsInGroupOne == nbrOfPixelsInGroupTwo && invRel2 > invRel1)) // group p2 added to group p1
                    {
                        int oldGroupId = pTwoGroupId;
                        _pixelsMap.Pixels[pOneId].NbPixelInGroup = totalNbrOfPixels;
                        _pixelsMap.Pixels[pTwoId].NbPixelInGroup = totalNbrOfPixels;
                        int inc = _pixelsMap.Pixels[pOneId].Increment - currentEdges[j].Increment - _pixelsMap.Pixels[pTwoId].Increment;
                        lastPixelAddedToGroup[pOneGroupId] = pTwoId;

                        for (int k = 0; k < nbrOfPixels; ++k) {
                            if (_pixelsMap.Pixels[k].GroupId == oldGroupId) {
                                _pixelsMap.Pixels[k].GroupId = pOneGroupId;
                                _pixelsMap.Pixels[k].Increment = inc;
                            }
                        }
                    }
                }
            }
        }
    }

    Mat ReliabilityHistUnwrap::ComputePixelsIncrement() {
        int cols = _pixelsMap.Width;
        int rows = _pixelsMap.Height;
        Mat unwrappedPhaseMap = Mat::zeros(rows, cols, CV_32FC1);

        for (int i = 0; i < _pixelsMap.Pixels.size(); ++i) {
            int row = _pixelsMap.Pixels.at(i).Index / cols;
            int col = _pixelsMap.Pixels.at(i).Index % cols;
            if (_pixelsMap.Pixels.at(i).Valid) {
                unwrappedPhaseMap.at<float>(row, col) = _pixelsMap.Pixels.at(i).PhaseValue + static_cast<float>(2 * CV_PI * _pixelsMap.Pixels.at(i).Increment);
            }
        }

        return unwrappedPhaseMap;
    }

    Mat ReliabilityHistUnwrap::InverseReliabilityMap() {
        int cols = _pixelsMap.Width;
        int rows = _pixelsMap.Height;
        Mat reliabilityMap = Mat::zeros(rows, cols, CV_32FC1);
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                int idx = i * cols + j;
                int rel = _pixelsMap.Pixels.at(idx).InverseReliability;
                reliabilityMap.at<float>(i, j) = _pixelsMap.Pixels.at(idx).InverseReliability;
            }
        }
        return reliabilityMap;
    }
} // namespace phase_unwrapping