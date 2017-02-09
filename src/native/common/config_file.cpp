#include "colors.h"
#include "config_file.h"
#include "trace.h"
#include "utils.h"

#include <stdlib.h>
#include <yaml.h>

const pal::string_t config_file_t::default_env_name = _X("default");

config_file_t::config_file_t(bool hide_warnings)
    : _hide_warnings(hide_warnings), _env_read(false)
{
}

config_file_t::config_file_t()
    : _hide_warnings(false), _env_read(false)
{
}

const pal::string_t config_file_t::get_env_name()
{
    if (!_env_read)
    {
        resolve_env_name();
    }
    return _env_name;
}

const pal::string_t config_file_t::get_filepath()
{
    return _filepath;
}

bool config_file_t::load(pal::string_t &cwd)
{
    for (pal::string_t parent_dir, cur_dir = cwd; true; cur_dir = parent_dir)
    {
        pal::string_t file = cur_dir;
        append_path(&file, _X(".dnvm"));

        trace::verbose(_X("Probing path [%s] for .dnvm"), file.c_str());
        if (pal::file_exists(file) && !pal::is_directory(file))
        {
            _filepath.assign(file.c_str());
            trace::verbose(_X("Found .dnvm [%s]"), _filepath.c_str());
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

/* Private */

bool config_file_t::resolve_env_name()
{
    if (!_env_name.empty())
    {
        return true;
    }

    if (!pal::file_exists(_filepath) || pal::is_directory(_filepath))
    {
        return false;
    }

    FILE *input = ::fopen(_filepath.c_str(), "r");

    if (input == NULL)
    {
        trace::verbose(_X("[%s] could not be opened"), _filepath.c_str());
        return false;
    }

    _env_read = true;

    yaml_parser_t parser;
    yaml_document_t document;

    bool success = false;
    bool done = false;
    bool env_found = false;

    yaml_parser_initialize(&parser);
    yaml_parser_set_input_file(&parser, input);

    if (!yaml_parser_load(&parser, &document))
    {
        goto finish;
    }

    if (yaml_document_get_root_node(&document))
    {
        yaml_node_t *node;
        for (node = document.nodes.start;
             node < document.nodes.top; node++)
        {
            switch (node->type)
            {
            case YAML_MAPPING_NODE:
                for (int k = 0;
                     k < (node->data.mapping.pairs.top - node->data.mapping.pairs.start);
                     k++)
                {
                    yaml_node_t *key = yaml_document_get_node(&document, node->data.mapping.pairs.start[k].key);

                    if (key->data.scalar.length == 3 &&
                        ::strncmp((char *)key->data.scalar.value, "env", 3) == 0)
                    {
                        env_found = true;
                        yaml_node_t *value = yaml_document_get_node(&document, node->data.mapping.pairs.start[k].value);
                        if (value->type != YAML_SCALAR_NODE)
                        {
                            if (!_hide_warnings)
                            {
                                trace::error(_YELLOW_X("warn: The value for 'env' must be a single, scalar value."));
                            }
                            break;
                        }

                        _env_name = pal::string_t((char *)value->data.scalar.value);
                        success = true;
                        break;
                    }
                }
                break;
            default:
                break;
            }
        }
    }

finish:
    fclose(input);
    yaml_document_delete(&document);
    yaml_parser_delete(&parser);

    return success;
}
