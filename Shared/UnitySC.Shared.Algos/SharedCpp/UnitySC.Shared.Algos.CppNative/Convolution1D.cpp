#include "pch.h"
#include "Convolution1D.h"

std::vector<double> FullConvolution1D(const std::vector<double>& f, const std::vector<double>& g)
{
    const auto nf = f.size();
    const auto ng = g.size();
    const auto n = nf + ng - 1;
    std::vector<double> out(n);
    for (auto i(0); i < n; ++i) {
        const auto jmn = (i >= ng - 1) ? i - (ng - 1) : 0;
        const auto jmx = (i < nf - 1) ? i : nf - 1;
        for (auto j(jmn); j <= jmx; ++j) {
            out[i] += (f[j] * g[i - j]);
        }
    }
    return out;
}

std::vector<double> ValidConvolution1D(const std::vector<double>& f, const std::vector<double>& g)
{
    const auto nf = f.size();
    const auto ng = g.size();
    const auto& min_v = (nf < ng) ? f : g;
    const auto& max_v = (nf < ng) ? g : f;
    const auto n = std::max(nf, ng) - std::min(nf, ng) + 1;
    std::vector<double> out(n);
    for (auto i(0); i < n; ++i) {
        for (int j(min_v.size() - 1), k(i); j >= 0; --j) {
            out[i] += min_v[j] * max_v[k];
            ++k;
        }
    }
    return out;
}
