cmake_minimum_required (VERSION 3.6)

project(dotnet CXX)
set (CMAKE_INSTALL_PREFIX ${CMAKE_CURRENT_SOURCE_DIR})
set (CMAKE_CXX_STANDARD 11)

include(../setup.cmake)
include_directories(../common)

set(SOURCES
    dotnet.cpp
    ../common/trace.cpp
    ../common/utils.cpp)

if (WIN32)
    add_definitions(-D_WIN32)
else()
    list(APPEND SOURCES ../common/pal.unix.cpp)
    if (APPLE)
        add_definitions(-D__APPLE__)
    endif()
endif()

add_executable(dotnet ${SOURCES})
install(TARGETS dotnet DESTINATION bin)