#pragma once

#include <string>

namespace Algorithms {
  // The algorithm collection will be used as a DLL wrapped into
  // C# code.
  // To avoid complicated exception management, simple Status stucture
  // is available for interface methods.

  enum class StatusCode : int { OK = 0, UNKNOWN_ERROR = -1, MISSING_EDGE_IMAGE = -2, MISSING_NOTCH_IMAGE = -3, BWA_FIT_FAILED = -4 };

  struct Status {
    StatusCode code;
    std::string message;
    // 1 is better
    double confidence;

    Status() : code(StatusCode::UNKNOWN_ERROR), message("Unknown error"), confidence(0){};
  };

} // namespace Algorithms
