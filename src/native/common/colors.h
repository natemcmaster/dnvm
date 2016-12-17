#ifndef COLORS_H
#define COLORS_H

#include "pal.h"

#define _RESET_X() _X("\033[0m")
#define _GRAY_X(s) _X("\033[90m")_X(s)_RESET_X()
#define _RED_X(s) _X("\033[1;31m")_X(s)_RESET_X()
#define _YELLOW_X(s) _X("\033[1;33m")_X(s)_RESET_X()

#endif // COLORS_H