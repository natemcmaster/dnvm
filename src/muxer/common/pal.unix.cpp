#include "pal.h"
#include "trace.h"
#include <unistd.h>
#include <sys/stat.h>

#if defined(__APPLE__)
#include <mach-o/dyld.h>
#include <sys/param.h>
#include <sys/sysctl.h>
#endif

#if defined(__LINUX__)
#define symlinkEntrypointExecutable "/proc/self/exe"
#elif !defined(__APPLE__)
#define symlinkEntrypointExecutable "/proc/curproc/exe"
#endif

pal::string_t pal::to_lower(const pal::string_t& in)
{
    pal::string_t ret = in;
    std::transform(ret.begin(), ret.end(), ret.begin(), ::tolower);
    return ret;
}

int pal::xtoi(const char_t *input)
{
    return atoi(input);
}

bool pal::getcwd(pal::string_t *recv)
{
    recv->clear();
    pal::char_t *buf = ::getcwd(nullptr, PATH_MAX + 1);
    if (buf == nullptr)
    {
        if (errno == ENOENT)
        {
            return false;
        }
        perror("getcwd()");
        return false;
    }
    recv->assign(buf);
    ::free(buf);
    return true;
}

#if defined(__APPLE__)
bool pal::get_own_executable_path(pal::string_t *recv)
{
    uint32_t path_length = 0;
    if (_NSGetExecutablePath(nullptr, &path_length) == -1)
    {
        char path_buf[path_length];
        if (_NSGetExecutablePath(path_buf, &path_length) == 0)
        {
            recv->assign(path_buf);
            return true;
        }
    }
    return false;
}
#elif defined(__FreeBSD__)
bool pal::get_own_executable_path(pal::string_t *recv)
{
    int mib[4];
    mib[0] = CTL_KERN;
    mib[1] = KERN_PROC;
    mib[2] = KERN_PROC_PATHNAME;
    mib[3] = -1;
    char buf[PATH_MAX];
    size_t cb = sizeof(buf);
    if (sysctl(mib, 4, buf, &cb, NULL, 0) == 0)
    {
        recv->assign(buf);
        return true;
    }
    // ENOMEM
    return false;
}
#else
bool pal::get_own_executable_path(pal::string_t *recv)
{
    // Just return the symlink to the exe from /proc
    // We'll call realpath on it later
    recv->assign(symlinkEntrypointExecutable);
    return true;
}
#endif

// Returns true only if an env variable can be read successfully to be non-empty.
bool pal::getenv(const pal::char_t *name, pal::string_t *recv)
{
    recv->clear();

    auto result = ::getenv(name);
    if (result != nullptr)
    {
        recv->assign(result);
    }

    return (recv->length() > 0);
}

bool pal::realpath(pal::string_t *path)
{
    auto resolved = ::realpath(path->c_str(), nullptr);
    if (resolved == nullptr)
    {
        if (errno == ENOENT)
        {
            return false;
        }
        perror("realpath()");
        return false;
    }
    path->assign(resolved);
    ::free(resolved);
    return true;
}

bool pal::is_path_rooted(const pal::string_t &path)
{
    return path.front() == '/';
}

bool pal::file_exists(const pal::string_t &path)
{
    if (path.empty())
    {
        return false;
    }
    struct stat buffer;
    return (::stat(path.c_str(), &buffer) == 0);
}

bool pal::is_directory(const pal::string_t &path)
{
    if (path.empty())
    {
        return false;
    }
    if (path.back() == PATH_SEPARATOR)
    {
        return true;
    }
    struct stat buffer;
    if ((::stat(path.c_str(), &buffer) != 0))
    {
        return false;
    }
    return (S_ISDIR(buffer.st_mode) != 0);
}

int pal::exec_process(const pal::string_t &file, const int argc, pal::char_t *argv[])
{
    pid_t child_pid;

    const pal::char_t *arg0 = file.c_str();
    pal::char_t **new_argv = new pal::char_t *[argc + 1];
    new_argv[0] = (pal::char_t *)arg0;
    for (int i = 1; i < argc; i++)
    {
        new_argv[i] = argv[i];
    }
    new_argv[argc] = nullptr;

    int status;
    int ret;
    if ((child_pid = ::fork()) < 0)
    {
        trace::error(_X("Failed to fork main process"));
        return -1;
    }
    else if (child_pid == 0)
    {
        trace::verbose(_X("--------> begin execution corehost"));
        if (::execvp(new_argv[0], new_argv) < 0)
        {
            trace::error(_X("Failed to exec new process"));
        }
        ::exit(errno);
    }
    else
    {
        ::waitpid(child_pid, &status, 0);
        trace::verbose(_X("--------> resume execution of dnvm muxer"));
    }
    delete[] new_argv;
    int rc = WEXITSTATUS(status);
    return rc;
}