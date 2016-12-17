# TODO auto-configure release vs debug settings
# add_definitions(-DCMAKE_BUILD_TYPE=Debug)
# add_compile_options(-g)

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
