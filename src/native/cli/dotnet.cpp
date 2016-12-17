#include "pal.h"
#include "trace.h"
#include "utils.h"
#include "colors.h"
#include "corehost_error_codes.h"

static pal::string_t s_own_path;
static pal::string_t s_global_env_name = _X("default");

bool find_config_file(pal::string_t &cwd, pal::string_t *recv)
{
    for (pal::string_t parent_dir, cur_dir = cwd; true; cur_dir = parent_dir)
    {
        pal::string_t file = cur_dir;
        append_path(&file, _X(".dnvm"));

        trace::verbose(_X("Probing path [%s] for .dnvm"), file.c_str());
        if (pal::file_exists(file) && !pal::is_directory(file))
        {
            recv->assign(file.c_str());
            trace::verbose(_X("Found .dnvm [%s]"), recv->c_str());
            return true;
        }
        parent_dir = get_directory(cur_dir);
        if (parent_dir.empty() || parent_dir.size() == cur_dir.size())
        {
            trace::verbose(_X("Terminating .dnvm search at [%s]"), parent_dir.c_str());
            break;
        }
    }
    return false;
}

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

bool resolve_env_name(pal::ifstream_t &file, pal::string_t *name)
{
    pal::string_t key;
    pal::string_t value;
    file >> key;
    file >> value;
    pal::string_t env = _X("env:");
    if (pal::strncmp(env.c_str(), key.c_str(), env.length()) == 0)
    {
        name->assign(value);
        return true;
    }
    return false;
}

void find_muxer_from_config(pal::string_t &config, pal::string_t *muxer, pal::string_t *env_name)
{
    if (!pal::file_exists(config) || pal::is_directory(config))
    {
        return;
    }

    pal::ifstream_t file(config);
    if (!file.good())
    {
        trace::verbose(_X("[%s] could not be opened"), config.c_str());
        return;
    }

    if (skip_utf8_bom(&file))
    {
        trace::verbose(_X("UTF-8 BOM skipped while reading [%s]"), config.c_str());
    }

    if (resolve_env_name(file, env_name))
    {
        find_muxer_for_env(*env_name, muxer);
    }
    else
    {
        trace::error(_YELLOW_X("warn: Could not resolve environment name from '%s'."), config.c_str());
        trace::error(_YELLOW_X("      Falling back to environment 'default'."));
    }
}

void find_global_muxer(pal::string_t *muxer)
{
    find_muxer_for_env(s_global_env_name, muxer);
}

int main(const int argc, pal::char_t *argv[])
{
    trace::setup();

    trace::verbose(_X("--------> start execution of dnvm muxer"));

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
        pal::string_t config;
        pal::string_t env_name;
        if (find_config_file(cwd, &config))
        {
            pal::string_t config_dir = get_directory(config);
            find_muxer_from_config(config, &muxer, &env_name);
        }
        else
        {
            find_global_muxer(&muxer);
        }

        bool invalid_muxer;
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
            if (config.empty())
            {
                trace::error(_RED_X("The default dotnet environment has not been installed.\nRun `dnvm install --global`."));
            }
            else
            {
                trace::error(_GRAY_X("Using '%s'"), config.c_str());
                trace::error(_RED_X("The environment '%s' has not been installed.\nRun `dnvm install`."),
                             env_name.c_str());
            }
            return 1;
        }

        if (config.empty())
        {
            trace::verbose(_X("Using default dotnet dnvironment"));
        }

        int rc = pal::exec_process(muxer, argc, argv);
        trace::verbose(_X("Sub-process returned %d"), rc);

        // TODO ensure this doesn't happen for other failures, such as bad host policy files
        if (rc == (StatusCode::CoreHostLibMissingFailure & 0xff))
        {
            // TODO check if there is a 'fx' section in the config file

            // extends the 'install the shared fx' error
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
            trace::println(_X("      dnvm install fx [version]")); // TODO detect the right version?
            trace::println();
            trace::println(_X("    See `dnvm install fx --help` for more info."));
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