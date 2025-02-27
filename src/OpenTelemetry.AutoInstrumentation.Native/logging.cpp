// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

#include "logging.h"

#include "pal.h"

#include "spdlog/sinks/null_sink.h"
#include "spdlog/sinks/rotating_file_sink.h"

#ifndef _WIN32
typedef struct stat Stat;
#endif

namespace trace
{

bool debug_logging_enabled   = false;
bool dump_il_rewrite_enabled = false;

std::string Logger::GetLogPath(const std::string& file_name_suffix)
{
    const auto path = ToString(DatadogLogFilePath(file_name_suffix));

    const auto log_path = std::filesystem::path(path);

    if (log_path.has_parent_path())
    {
        const auto parent_path = log_path.parent_path();

        if (!std::filesystem::exists(parent_path))
        {
            std::filesystem::create_directories(parent_path);
        }
    }

    return path;
}

Logger::Logger()
{
    spdlog::set_error_handler([](const std::string& msg) {
        // By writing into the stderr was changing the behavior in a CI scenario.
        // There's not a good way to report errors when trying to create the log
        // file.
        // But we never should be changing the normal behavior of an app.
        // std::cerr << "Logger Handler: " << msg << std::endl;
    });

    spdlog::flush_every(std::chrono::seconds(3));

    static auto current_process_name = ToString(GetCurrentProcessName());
    static auto current_process_id   = GetPID();
    static auto current_process_without_extension =
        current_process_name.substr(0, current_process_name.find_last_of("."));

    static auto file_name_suffix = "-" + current_process_without_extension + "-" + std::to_string(current_process_id);

    try
    {
        m_fileout = spdlog::rotating_logger_mt("Logger", GetLogPath(file_name_suffix), 1048576 * 5, 10);
    }
    catch (...)
    {
        std::cerr << "Logger Handler: Error creating native log file." << std::endl;
        m_fileout = spdlog::null_logger_mt("Logger");
    }

    m_fileout->set_level(spdlog::level::debug);

    m_fileout->set_pattern("%D %I:%M:%S.%e %p [%P|%t] [%l] %v");

    m_fileout->flush_on(spdlog::level::info);
};

Logger::~Logger()
{
    m_fileout->flush();
    spdlog::shutdown();
};

void Logger::Debug(const std::string& str)
{
    if (debug_logging_enabled)
    {
        m_fileout->debug(str);
    }
}
void Logger::Info(const std::string& str)
{
    m_fileout->info(str);
}
void Logger::Warn(const std::string& str)
{
    m_fileout->warn(str);
}
void Logger::Error(const std::string& str)
{
    m_fileout->error(str);
}
void Logger::Critical(const std::string& str)
{
    m_fileout->critical(str);
}
void Logger::Flush()
{
    m_fileout->flush();
}
} // namespace trace
