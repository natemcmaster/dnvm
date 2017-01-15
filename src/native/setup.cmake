if(CMAKE_BUILD_TYPE MATCHES Debug)
    # include debug symbols
    add_compile_options(-g)
endif()

add_compile_options(-Werror) # make warnings errors
add_compile_options(-Wreturn-type) # ensure functions return a type

if(WIN32)
    add_definitions(-D_WIN32)
else()
    add_compile_options(-Wno-unused-local-typedef)
    if (APPLE)
        add_definitions(-D__APPLE__)
    endif()
endif()
