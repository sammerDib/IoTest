// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"

#define NOMINMAX // Prevents conflict between windows macros and std min max implementation

// Windows Header Files:
#include <windows.h>

#include <vector>
#include <algorithm>
#include <numeric>
#include <cmath>
#include <iterator>
#include <limits>
#include <optional>
#include <string>
#include <fstream>
#include <type_traits>
#include <memory>

#endif //PCH_H
