project(dnvm-prompt CXX)
set (CMAKE_INSTALL_PREFIX ${CMAKE_CURRENT_SOURCE_DIR})
set (CMAKE_CXX_STANDARD 11)

include(../setup.cmake)
include_directories(../common)

set(SOURCES
    dnvm-prompt.cpp
    ../common/trace.cpp
    ../common/utils.cpp
    ../common/config_file.cpp)

if(WIN32)
else()
    list(APPEND SOURCES ../common/pal.unix.cpp)
endif()

add_executable(dnvm-prompt ${SOURCES})
target_link_libraries(dnvm-prompt yaml)
install(TARGETS dnvm-prompt
        DESTINATION .)
