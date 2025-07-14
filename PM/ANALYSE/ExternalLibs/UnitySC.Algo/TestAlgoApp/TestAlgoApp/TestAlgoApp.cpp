#include <iostream>
#include <algorithm>
#include <vector>
#include <filesystem>
#include <stdlib.h>
#include <fstream>

#include <Topography\PhaseShiftingInterferometry.hpp>

namespace fs = std::filesystem;

class InputParser {
public:
    InputParser(int& argc, char** argv) {
        for (int i = 1; i < argc; ++i)
            this->tokens.push_back(std::string(argv[i]));
    }

    const std::string& getCmdOption(const std::string& option) const {
        std::vector<std::string>::const_iterator itr;
        itr = std::find(this->tokens.begin(), this->tokens.end(), option);
        if (itr != this->tokens.end() && ++itr != this->tokens.end()) {
            return *itr;
        }
        static const std::string empty_string("");
        return empty_string;
    }

    bool cmdOptionExists(const std::string& option) const {
        return std::find(this->tokens.begin(), this->tokens.end(), option)
            != this->tokens.end();
    }

private:
    std::vector <std::string> tokens;
};

struct CommandLineException : public std::exception
{
    CommandLineException(std::string info){
        exceptionInfo = info;
    }

    const char* what() const throw ()
    {
        return exceptionInfo.c_str();
    }

    std::string exceptionInfo;
};

psi::UnwrapMode resolveUnwrappingMode(std::string input) {

    if(input == "Goldstein")
    {
        return psi::UnwrapMode::Goldstein;
    }
    else if (input == "HaiLei")
    {
        return psi::UnwrapMode::HistogramReliabilityPath;
    }
    else if (input == "Corr")
    {
        return psi::UnwrapMode::PseudoCorrelationQuality;
    }
    else if (input == "Var")
    {
        return psi::UnwrapMode::VarianceQuality;
    }
    else if (input == "Grad")
    {
        return psi::UnwrapMode::GradientQuality;
    }
    else{
        throw new CommandLineException("Unwrapping method unknown.");
    }
}

bool isNumber(const std::string& str)
{
    return str.find_first_not_of("0123456789") == std::string::npos;
}


void processTopology(fs::path imageFolderPath, double wavelengthNm, int stepNb, psi::UnwrapMode mode)
{
    std::vector<cv::Mat> imgs = std::vector<cv::Mat>();

    fs::directory_iterator endIterator;
    for (fs::directory_iterator itr(imageFolderPath); itr != endIterator; ++itr)
    {
        if (!is_directory(itr->status()))
        {
            fs::path filePath = itr->path();
            cv::Mat img = cv::imread(filePath.string());
            if (img.empty()) {
                std::cout << "Could not read the image: " << filePath.string() << std::endl;
            }
            imgs.push_back(img);
        }
    }

    fs::path resultPath = imageFolderPath.string() + "\\result";
    psi::PSIResultData result = psi::TopoReconstructionFromPhase(imgs, wavelengthNm, stepNb, mode, resultPath);
}


int main(int argc, char** argv) {

    InputParser input(argc, argv);
    bool errorInCmdLine = false;

    const char* helperInfos =
        "This application executes a phase shift interferometry algorithm on a given directory containing interferometric images.\n"
        "It create a resulting interferometric depth map on subdirectory 'result' added in the given directory.\n"
        "\n"
        "Usage: ./PSI.exe [STEP NUMBER] [WAVELENGHT] [DIRECTORY PATH] [OPTIONS]\n"
        "example: ./PSI.exe 7 618 ./testData -U Goldstein\n"
        "\n"
        "\n"
        "OPTIONS :\n"
        "  -U, --unwrapping [Goldstein, HaiLei, Corr, Var, Grad] : use specific unwrapping algorithm\n"
        "                   [Goldstein]  use goldstein algorithm for unwrapping (used by default)\n"
        "                   [HaiLei]     use Hai Lei histogram reliability path algorithms for unwrapping\n"
        "                   [Corr]       use pseudo correlation quality algorithms for unwrapping\n"
        "                   [Var]        use variance quality algorithms for unwrapping\n"
        "                   [Grad]       use gradient quality algorithms for unwrapping\n";

    if (input.cmdOptionExists("-h") || input.cmdOptionExists("--help")) {
        std::cout << helperInfos;
        return 0;
    }

    std::string stepNbWanted = argv[1];
    if (!isNumber(stepNbWanted.c_str()))
    {
        std::cerr << "The number of steps must be an integer";
        errorInCmdLine = true;
    }

    std::string wavelenghtWanted = argv[2];
    if (!isNumber(wavelenghtWanted.c_str()))
    {
        std::cerr << "The wavelenght must be an integer";
        errorInCmdLine = true;
    }

    std::string directoryPathWanted = argv[3];
    fs::path directoryPath = fs::path(directoryPathWanted);
    if (!exists(directoryPath)) {
        std::cout << "Directory doesn't exist: " << directoryPath << std::endl;
        errorInCmdLine = true;
    }

    std::string defaultUnwrappingMode = "Goldstein";
    std::string unwrappingModeWanted = defaultUnwrappingMode;
    if (input.cmdOptionExists("-U")) {
        unwrappingModeWanted = input.getCmdOption("-U");
    }
    else if (input.cmdOptionExists("--unwrapping"))
    {
        unwrappingModeWanted = input.getCmdOption("--unwrapping");
    }

    psi::UnwrapMode unwrappingMode;
    try
    {
        unwrappingMode = resolveUnwrappingMode(unwrappingModeWanted);
        processTopology(directoryPath, std::stod(wavelenghtWanted), std::stod(stepNbWanted), unwrappingMode);
    }
    catch(const CommandLineException& e)
    {
        errorInCmdLine = true;
        std::cerr << e.what();
    }
    catch (const std::exception& e)
    {
        std::cerr << e.what();
        return -1;
    }

    if (errorInCmdLine)
    {
        std::cout << helperInfos;
        return -1;
    }

    return 0;
}
