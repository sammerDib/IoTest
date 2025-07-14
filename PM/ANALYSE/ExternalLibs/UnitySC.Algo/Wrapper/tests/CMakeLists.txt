cmake_minimum_required(VERSION 3.18)


set(_VERSION 0.0.14)

project(${CPACK_NUGET_PACKAGE_NAME}_Tests VERSION ${_VERSION} LANGUAGES CSharp)

include(CSharpUtilities)
include(files.cmake)

set(TARGET_NAME ${CPACK_NUGET_PACKAGE_NAME}_Tests)

add_library(${TARGET_NAME} SHARED files.cmake ${TEST_FILES})
target_link_libraries(${TARGET_NAME} PRIVATE ${NATIVE_LIBRARY_TARGET})

target_compile_options(${TARGET_NAME} PUBLIC "/unsafe")

# Find Nuget (install the latest CLI here: https://www.nuget.org/downloads).
find_program(NUGET nuget)
if(NOT NUGET)
    message(FATAL "CMake could not find the nuget command line tool. Please install it!")
else()
    
add_custom_command(TARGET ${TARGET_NAME}
    PRE_BUILD
    COMMAND nuget restore ${CMAKE_BINARY_DIR}
)
endif()

####################################
# NuGet packages dependencies
#
set(VS_PACKAGE_REFERENCES
    "MSTest.TestFramework_2.1.0"
    "MSTest.TestAdapter_2.1.0"
    "Microsoft.NET.Test.Sdk_16.5.0"
    "coverlet.collector_1.2.0"
)
set_property(TARGET ${TARGET_NAME} 
    PROPERTY VS_PACKAGE_REFERENCES ${VS_PACKAGE_REFERENCES})


####################################
# Assembly dependencies
#
set(DEPENDENCIES
    "Microsoft.CSharp"
    "System"
    "System.Core"
    "System.Data"
    "System.Drawing"
    "System.Collections"
    "UnitySC.AlgorithmLibrary"
)
set_property(TARGET ${TARGET_NAME} 
    PROPERTY VS_DOTNET_REFERENCES ${DEPENDENCIES})

set_property(TARGET ${TARGET_NAME} PROPERTY CMAKE_DOTNET_TARGET_FRAMEWORK "net471")
set_property(TARGET ${TARGET_NAME} PROPERTY COMMON_LANGUAGE_RUNTIME "")

# This project is a test project
set_target_properties(${TARGET_NAME} PROPERTIES
    VS_GLOBAL_PROJECT_TYPES "{3AC096D0-A1C2-E12C-1390-A8335801FDAB};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
)

set_property(TARGET ${TARGET_NAME} PROPERTY VS_DOTNET_TARGET_ISPACKABLE "no")
set_property(TARGET ${TARGET_NAME} PROPERTY VS_DOTNET_TARGET_NATIVEDEBUGGING "true")
# Set the .NET Framework version for the executable.
set_property(TARGET ${TARGET_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.1")

# Set the executable to be 32-bit.
set_property(TARGET ${TARGET_NAME} PROPERTY WIN32_EXECUTABLE FALSE)

# Set the C# language version (defaults to 3.0).
set(CMAKE_CSharp_FLAGS "/langversion:latest")

set_property(TARGET ${TARGET_NAME} PROPERTY VS_GLOBAL_Prefer32Bit "false")

find_program(DOTNET dotnet)
if(NOT DOTNET)
    message(FATAL "CMake could not find the dotnet command line tool. Please install it!")
else()
    # does not work yet
    # add_test(${TARGET_NAME}Runner ${DOTNET} test ${TARGET_NAME}.sln)
endif()
