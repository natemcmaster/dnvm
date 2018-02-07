#include "pal.h"
#include "trace.h"
#include "utils.h"
#include "colors.h"
#include "corehost_error_codes.h"
#include "variables.h"
#include "config_file.h"

#include <stdlib.h>

static pal::string_t s_own_path;

bool resolve_env_root(const pal::string_t &env_name, pal::string_t *path)
{
    if (!is_valid_filename(env_name))
    {
        trace::error(_X("Detected invalid environment name '%s'"), env_name.c_str());
        return false;
    }
    // TODO consider adding a .dnvmrc file to move this. May be required on machines
    // where users don't have sudo access
    pal::string_t env_path = _X("/usr/local/share/dnvm/environments");
    trace::verbose(_X("Probing for dotnet environment '%s' in [%s]"), env_name.c_str(), env_path.c_str());
    append_path(&env_path, pal::to_lower(env_name).c_str());

    if (!pal::file_exists(env_path) || !pal::is_directory(env_path))
    {
        trace::warning(_X("Failed to locate env name in [%s]"), env_path.c_str());
        return false;
    }
    path->assign(env_path);
    trace::verbose(_X("Found env root in [%s]"), env_path.c_str());
    return true;
}

void find_muxer_for_env(const pal::string_t &env_name, pal::string_t *muxer)
{
    pal::string_t root;
    if (resolve_env_root(env_name, &root))
    {
        append_path(&root, _X("dotnet"));
        muxer->assign(get_executable(root));
        return;
    }
}

void find_global_muxer(pal::string_t *muxer)
{
    find_muxer_for_env(config_file_t::default_env_name, muxer);
}


void find_muxer_from_config(config_file_t &config_file, pal::string_t *muxer)
{
    const pal::string_t env_name = config_file.get_env_name();
    if (env_name.empty())
    {
        trace::verbose(_X("Could not resolve environment name from '%s'."), config_file.get_filepath().c_str());
        trace::verbose(_X("Falling back to environment 'default'."));
        find_global_muxer(muxer);
    }
    else
    {
        find_muxer_for_env(env_name, muxer);
    }
}

int main(const int argc, pal::char_t *argv[])
{
    trace::setup();

    trace::verbose(_X("--------> start execution of dnvm muxer"));
    trace::verbose(_X("dnvm version " BUILD_VERSION " " GIT_COMMIT_HASH));

    try
    {
        if (!pal::get_own_executable_path(&s_own_path) || !pal::realpath(&s_own_path))
        {
            trace::error(_X("Could not identifier on executable path"));
            return 1;
        }

        trace::verbose(_X("Executing pre-muxer from [%s]"), s_own_path.c_str());

        pal::string_t cwd;
        if (!pal::getcwd(&cwd))
        {
            trace::error(_X("Could not identify current working directory"));
            return 1;
        }

        pal::string_t muxer;
        config_file_t config_file;

        if (config_file.load(cwd))
        {
            find_muxer_from_config(config_file, &muxer);
        }
        else
        {
            find_global_muxer(&muxer);
        }

        bool invalid_muxer = false;
        if (muxer.empty())
        {
            invalid_muxer = true;
            trace::verbose(_X("Could not find a candidate muxer path [%s]"), muxer.c_str());
        }

        if (!pal::file_exists(muxer))
        {
            invalid_muxer = true;
            trace::verbose(_X("Candidate muxer path does not exist [%s]"), muxer.c_str());
        }

        if (invalid_muxer)
        {
            if (config_file.get_filepath().empty())
            {
                trace::error(_RED_X("The default dotnet environment has not been installed.\nRun `dnvm install --help` for more information on how to install .NET Core."));
            }
            else
            {
                trace::error(_GRAY_X("Using '%s'"), config_file.get_filepath().c_str());
                trace::error(_RED_X("The environment '%s' has not been installed.\nRun `dnvm install`."),
                             config_file.get_env_name().c_str());
            }
            return 1;
        }

        if (config_file.get_env_name().empty())
        {
            trace::verbose(_X("Using default dotnet dnvironment"));
        }

        // extend PATH to include the environment's bin folder
        // to allow the .NET Core SDK to find and launch tools
        pal::string_t local_bin = get_directory(muxer);
        append_path(&local_bin, _X("bin"));
        pal::string_t pathext = local_bin + PATH_SEPARATOR + ::getenv("PATH");
        trace::verbose(_X("Setting PATH to '%s'"), pathext.c_str());
        ::setenv("PATH", pathext.c_str(), 1);

        int rc = pal::exec_process(muxer, argc, argv);
        trace::verbose(_X("Sub-process returned %d"), rc);

        // TODO ensure this doesn't happen for other failures, such as bad host policy files
        if (rc == (StatusCode::CoreHostLibMissingFailure & 0xff))
        {
            // TODO check if there is a 'runtime' section in the config file

            // extends the 'install the shared runtime' error
            // see https://github.com/dotnet/core-setup/blob/eca152525e19a41d489cd5845afa14982653cc0a/src/corehost/cli/fxr/fx_muxer.cpp#L22

            /*              $ dotnet run
                            |  The specified framework 'Microsoft.NETCore.App', version 'x.y.z' was not found.
                            |    - Check application dependencies and target a framework version installed at:
                            |        /home/user/.dnvm/environments/default/shared/Microsoft.NETCore.App
                            |    - The following versions are installed:
                            |        1.0.1
                            |    - Alternatively, install the framework version 'x.y.z'. */
            trace::println(_X("    This can be done using the 'dnvm' command."));
            trace::println();
            trace::println(_X("      dnvm install runtime [version]")); // TODO detect the right version?
            trace::println();
            trace::println(_X("    See `dnvm install runtime --help` for more info."));
        }

        return rc;
    }
    catch (const std::runtime_error &ex)
    {
        trace::error(ex.what());
        return 1;
    }
    return 0;
}
