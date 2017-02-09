project(dotnet CXX)
set (CMAKE_INSTALL_PREFIX ${CMAKE_CURRENT_SOURCE_DIR})
set (CMAKE_CXX_STANDARD 11)

include(../setup.cmake)
include_directories(../common)

set(SOURCES
    dotnet.cpp
    ../common/trace.cpp
    ../common/utils.cpp
    ../common/config_file.cpp)

if(WIN32)
else()
    list(APPEND SOURCES ../common/pal.unix.cpp)
endif()

add_executable(dotnet ${SOURCES})
target_link_libraries(dotnet yaml)
install(TARGETS dotnet
        DESTINATION .)
