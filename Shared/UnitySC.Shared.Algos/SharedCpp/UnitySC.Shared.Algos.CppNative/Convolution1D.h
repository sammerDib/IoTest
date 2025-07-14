#pragma once

// Performs a convolution
// Returns a std::vector of size f.size() + g.size() - 1
std::vector<double> FullConvolution1D(const std::vector<double>& f, const std::vector<double>& g);

// Performs a convolution without padding zeros.
// Returns a std::vector of size std::max(f.size(), g.size()) - std::min(f.size(), g.size()) + 1
std::vector<double> ValidConvolution1D(const std::vector<double>& f, const std::vector<double>& g);

// Performs a convolution without padding zeros.
// f.size() must be greater or equal to kernel.size()
template <typename ItF, typename ItK>
std::vector<double> ValidConvolution1D(const ItF fBegin, const ItF fEnd, const ItK kernelBegin, const ItK kernelEnd)
{
    const auto nf = std::distance(fBegin, fEnd);
    const auto nk = std::distance(kernelBegin, kernelEnd);

    if (nf < nk)
    {
        return {};
    }

    const auto n = nf - nk + 1;
    std::vector<double> out(n);
    for (auto i(0); i < n; ++i) {
        for (int j(nk - 1), k(i); j >= 0; --j) {
            out[i] += *(kernelBegin+j) * *(fBegin+k);
            ++k;
        }
    }
    return out;
}