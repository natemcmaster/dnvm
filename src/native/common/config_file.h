#ifndef CONFIGFILE_H
#define CONFIGFILE_H

#include "pal.h"

class config_file_t
{
  public:
    config_file_t();
    config_file_t(bool hide_warnings);

    bool load(pal::string_t &cwd);
    const pal::string_t get_env_name();
    const pal::string_t get_filepath();

    static const pal::string_t default_env_name;

  private:
    const bool _hide_warnings;
    pal::string_t _filepath;
    pal::string_t _env_name;
    bool resolve_env_name();
    bool _env_read;
};

#endif // CONFIGFILE_H
