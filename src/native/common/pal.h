// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#ifndef PAL_H
#define PAL_H

#include <string>
#include <iostream>
#include <fstream>

#if defined(_WIN32)

#else

#include <cstdlib>

#define xerr std::cerr
#define xout std::cout
#define DIR_SEPARATOR '/'
#define PATH_SEPARATOR ':'
#define _X(s) s

#endif

namespace pal
{

#if defined(_WIN32)
#else
typedef char char_t;
typedef std::string string_t;

#endif

#if defined(_WIN32)
#else
inline void err_vprintf(const char_t *format, va_list vl)
{
    ::vfprintf(stderr, format, vl);
    ::fputc('\n', stderr);
}
inline void out_vprintf(const char_t *format, va_list vl)
{
    ::vfprintf(stdout, format, vl);
    ::fputc('\n', stdout);
}

inline string_t exe_suffix() { return _X(""); }


inline int cstrcasecmp(const char *str1, const char *str2) { return ::strcasecmp(str1, str2); }
inline int strcmp(const char_t *str1, const char_t *str2) { return ::strcmp(str1, str2); }
inline int strcasecmp(const char_t *str1, const char_t *str2) { return ::strcasecmp(str1, str2); }
inline int strncmp(const char_t *str1, const char_t *str2, int len) { return ::strncmp(str1, str2, len); }
inline int strncasecmp(const char_t *str1, const char_t *str2, int len) { return ::strncasecmp(str1, str2, len); }

typedef std::basic_ifstream<char> ifstream_t;

#endif

pal::string_t to_lower(const pal::string_t& in);

bool is_directory(const string_t &path);
bool file_exists(const string_t &path);
bool get_own_executable_path(string_t *recv);
bool getenv(const char_t *name, string_t *recv);
int xtoi(const char_t *input);
bool getcwd(pal::string_t *recv);
bool realpath(pal::string_t *recv);
bool is_path_rooted(const string_t &path);
int exec_process(const string_t &file, const int argc, char_t *argv[]);
}

#endif // PAL_H