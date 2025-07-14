cmake_minimum_required(VERSION 3.18)


set(_VERSION 0.0.50)

if( NOT "${BUILD_ID}" STREQUAL "" )
    # SNAPSHOT build

    string(TIMESTAMP BUILD_ID "%Y%m%d%H%M%S")

    set(_PACKAGE_VERSION ${_VERSION}-b${BUILD_ID})
    message("Building SNAPSHOT v${_PACKAGE_VERSION}")
else()
    # RELEASE build
    set(_PACKAGE_VERSION ${_VERSION})
    message("Building RELEASE v${_PACKAGE_VERSION}")
endif()

set(CPACK_NUGET_PACKAGE_NAME UnitySC.AlgorithmLibrary)
project(${CPACK_NUGET_PACKAGE_NAME} VERSION ${_VERSION} LANGUAGES CSharp)

include(files.cmake)

add_library(${CPACK_NUGET_PACKAGE_NAME} SHARED files.cmake ${H_FILES} ${CPP_FILES})
target_link_libraries(${CPACK_NUGET_PACKAGE_NAME} ${NATIVE_LIBRARY_TARGET})

set_property(TARGET ${CPACK_NUGET_PACKAGE_NAME} 
    PROPERTY VS_PACKAGE_REFERENCES ${VS_PACKAGE_REFERENCES})

set(DEPENDENCIES 
    "Microsoft.CSharp"
    "System;System.Core"
    "System.Data"
    "System.Drawing"
)

set_property(TARGET ${CPACK_NUGET_PACKAGE_NAME} PROPERTY VS_DOTNET_REFERENCES ${DEPENDENCIES})

###############################################################################
# Unit tests sub project
add_subdirectory(tests)

###############################################################################
# NuGET package

 install (TARGETS ${CPACK_NUGET_PACKAGE_NAME} DESTINATION "lib/net471")
 set_property(TARGET ${CPACK_NUGET_PACKAGE_NAME} PROPERTY CMAKE_DOTNET_TARGET_FRAMEWORK "net471;netstandard2.0")
 set_property(TARGET ${CPACK_NUGET_PACKAGE_NAME} PROPERTY COMMON_LANGUAGE_RUNTIME "")
 set(CPACK_NUGET_PACKAGE_NAME "${CPACK_NUGET_PACKAGE_NAME}")
 set(CPACK_NUGET_PACKAGE_DEPENDENCIES "Microsoft.CSharp System System.Core System.Data System.Drawing System.Collections.Generic")
 set(CPACK_NUGET_PACKAGE_TITLE "UnitySC algorithm LIBRARY_OUTPUT_DIRECTORY project")
 set(CPACK_NUGET_PACKAGE_DESCRIPTION "This package contains native algorithm used in UnitySC projects.")
 set(CPACK_NUGET_PACKAGE_AUTHORS "UnitySC development team")
 set(CPACK_NUGET_COMPONENT_INSTALL TRUE)
 set(CPACK_PACKAGE_VERSION ${_PACKAGE_VERSION})
 set(CPACK_NUGET_PACKAGE_VERSION ${_PACKAGE_VERSION})
 set(CPACK_GENERATOR "NuGet")

 include(CPack)
