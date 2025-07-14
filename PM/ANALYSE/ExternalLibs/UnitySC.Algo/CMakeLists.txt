cmake_minimum_required(VERSION 3.18)
project(UnitySC.AlgorithmLibrary)

set (CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

set(CMAKE_EXPORT_COMPILE_COMMANDS "ON")
set(CMAKE_INCLUDE_CURRENT_DIR ON)

################################################################################
# Conan setup
include(${CMAKE_BINARY_DIR}/conanbuildinfo.cmake)
conan_basic_setup()

################################################################################
# DEPENDENCIES

if(${LOCAL_OPENCV})
find_package(OpenCV REQUIRED)
include_directories(
    ${CMAKE_SOURCE_DIR}/include
    /opt/include/opencv4/
)
endif(${LOCAL_OPENCV})

# Mathplotlib, used in dev only to display various data
if(USE_MATHPLOTLIB)
    list(append H_FILES include/matplotlibcpp.h)
    add_definitions(-DUSE_MATHPLOTLIB)
    function (bindMathPlotLibToTarget targetToBind)
        find_package(Python3 REQUIRED COMPONENTS Development NumPy)
        target_include_directories(${targetToBind} PRIVATE ${Python3_INCLUDE_DIRS} ${Python3_NumPy_INCLUDE_DIRS})
        target_link_libraries(${targetToBind} Python3::Python Python3::NumPy)
    endfunction(bindMathPlotLibToTarget)
endif(USE_MATHPLOTLIB)

################################################################################
#INCLUDES

include_directories(${CMAKE_SOURCE_DIR}/include/)
include(GenerateExportHeader)
include(${CMAKE_SOURCE_DIR}/files.cmake)

################################################################################
# C++ Library
#
set(NATIVE_LIBRARY_TARGET "UnitySC_AlgorithmLibrary_Native")
add_library(${NATIVE_LIBRARY_TARGET} STATIC files.cmake ${CPP_FILES} ${H_FILES})
set_property(TARGET ${NATIVE_LIBRARY_TARGET} PROPERTY CXX_STANDARD 17)
GENERATE_EXPORT_HEADER(${NATIVE_LIBRARY_TARGET})
target_compile_features(${NATIVE_LIBRARY_TARGET} PUBLIC cxx_std_17)
conan_target_link_libraries(${NATIVE_LIBRARY_TARGET})
if(USE_MATHPLOTLIB)
bindMathPlotLibToTarget(${NATIVE_LIBRARY_TARGET})
endif(USE_MATHPLOTLIB)


################################################################################
#TESTS
#
enable_testing()
include(GoogleTest)
add_definitions(-DTEST_DATA_PATH="${CMAKE_SOURCE_DIR}/tests/data/")
set(TEST_RUNNER_NAME UnitySC_AlgorithmLibrary_Native_Tests)
add_executable(${TEST_RUNNER_NAME} files.cmake ${TESTS})
target_link_libraries(${TEST_RUNNER_NAME} ${NATIVE_LIBRARY_TARGET})
target_compile_features(${TEST_RUNNER_NAME} PUBLIC cxx_std_17) 
set_property(TARGET ${TEST_RUNNER_NAME} PROPERTY CXX_STANDARD 17)
conan_target_link_libraries(${TEST_RUNNER_NAME})
if(USE_MATHPLOTLIB)
bindMathPlotLibToTarget(${TEST_RUNNER_NAME})
endif(USE_MATHPLOTLIB)
gtest_discover_tests(${TEST_RUNNER_NAME})

###############################################################################
# C# Wrapper
add_subdirectory(Wrapper)

################################################################################
