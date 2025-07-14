#pragma once
#include <chrono>
#include <string>
#include <vector>

class fde
{
public:
    static std::string printf(_In_z_ _Printf_format_string_ char const* const _Format, ...)
    {
        va_list vl;
        va_start(vl, _Format);
        int nsize = vsnprintf(NULL, 0, _Format, vl);
        nsize++;

        char* buffer = new char[nsize];
        vsnprintf(buffer, nsize, _Format, vl);

        va_end(vl);
        std::string str = buffer;
        delete[] buffer;
        return str;
    }

    static int exists(const char *name)
    {
        struct stat   buffer;
        return (stat(name, &buffer) == 0);
    }

    class chrono
    {
    protected:
        std::chrono::time_point<std::chrono::high_resolution_clock> begin, end;
        std::string message;
    public:
        chrono::chrono()
        {
            restart();
        }

        chrono::chrono(const std::string& message)
        {
            this->message = message;
            restart();
        }

        chrono::~chrono()
        {
            if (!message.empty())
                std::printf("%s : %.3f s\n", message.c_str(), elapsed());
        }

        void chrono::restart()
        {
            begin = std::chrono::high_resolution_clock::now();
        }

        // En nanoscecondes
        double elapsed()
        {
            end = std::chrono::high_resolution_clock::now();
            double elapsed = std::chrono::duration_cast<std::chrono::nanoseconds>(end - begin).count() / 1E9;
            return elapsed;
        }
    };

    class string : public std::string 
    {
    public:
        string() {}
        string(char *s) : std::string(s) { }
        string(const string& str) : std::string(str) {}
        string(const string& str, size_t pos, size_t len = npos) : std::string(str, pos, len) {}

        bool start_with(const std::string& prefix)
        {
            for (int i = 0; i < prefix.size(); i++)
            {
                if ((*this)[i] != prefix[i])
                    return false;
            }
            return true;
        }

        // split: receives a char delimiter; returns a vector of strings
        // By default ignores repeated delimiters
        std::vector<string> split(char delim, bool ignore_repeated_seperator = true)
        {
            std::vector<string> flds;

            string buf = "";
            int i = 0;
            while (i < length()) 
            {
                if ((*this)[i] != delim)
                {
                    buf += (*this)[i];
                }
                else if (ignore_repeated_seperator) 
                {
                    flds.push_back(buf);
                    buf = "";
                }
                else if (buf.length() > 0) 
                {
                    flds.push_back(buf);
                    buf = "";
                }
                i++;
            }
            if (!buf.empty())
                flds.push_back(buf);

            // TODO FDE c'est particulièrement inefficace de retourner une copie. Mais tellement pratique !
            return flds;
        }
    };

};
