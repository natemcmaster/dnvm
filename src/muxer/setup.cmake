# TODO auto-configure release vs debug settings
# add_definitions(-DCMAKE_BUILD_TYPE=Debug)
# add_compile_options(-g)

if(WIN32)

else()
    add_compile_options(-Wno-unused-local-typedef)
endif()