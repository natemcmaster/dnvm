#include "trace.h"
#include "pal.h"
#include "variables.h"
#include "config_file.h"

int main(const int argc, pal::char_t *argv[])
{
    trace::setup();

    trace::verbose(_X("dnvm-prompt version " BUILD_VERSION " " GIT_COMMIT_HASH));

    try
    {
        pal::string_t cwd;
        if (!pal::getcwd(&cwd))
        {
            trace::error(_X("Could not identify current working directory"));
            return 1;
        }

        config_file_t config(/* hide_warning */ true);
        config.load(cwd);
        if (config.load(cwd))
        {
            if (config.get_env_name().empty())
            {
                std::cout << config_file_t::default_env_name;
            }
            else
            {
                std::cout << config.get_env_name();
            }
            std::cout << std::endl;
        }

        return 0;
    }
    catch (const std::runtime_error &ex)
    {
        trace::error(ex.what());
        return 1;
    }
    return 0;
}
