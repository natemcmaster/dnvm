// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#ifndef UTILS_H
#define UTILS_H

#include "pal.h"

bool ends_with(const pal::string_t &value, const pal::string_t &suffix, bool match_case);
bool starts_with(const pal::string_t &value, const pal::string_t &prefix, bool match_case);
pal::string_t get_executable(const pal::string_t &filename);
pal::string_t get_directory(const pal::string_t &path);
pal::string_t strip_file_ext(const pal::string_t &path);
pal::string_t get_filename(const pal::string_t &path);
pal::string_t get_filename_without_ext(const pal::string_t &path);
void append_path(pal::string_t *path1, const pal::char_t *path2);
bool is_valid_filename(const pal::string_t &filename);
bool skip_utf8_bom(pal::ifstream_t* stream);

#endif // UTILS_H