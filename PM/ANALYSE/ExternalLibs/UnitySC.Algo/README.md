# imageAnalysis

The ImageAnalysis solution is an hybrid project which contain C++ and CSharp code. 
The C++ code will use third party libraries, like OpenCV for image processing. The dependency management on these libraries is left to a tool named [Conan](https://conan.io). Conan is able to download binaries of third parties, as well as download and compile source code if no binary is available for specific operating system or compiler. Its control file `conanfile.txt` contains the list of third party libraries to be used in the project.
That way, ImageAnalysis can be built under Windows or Linux with minimal worries, without the need to compile third party manually.
The Conan tool is written using the [Python](https://www.python.org/) language. To install it easilly under Windows, we decided to use a Windows package manager, [Chocolatey](https://chocolatey.org/). It will be able to install Python and Conan without the hassle of running installation assistant and prevent many "next button" clicks.

To make project management simpler, we used CMake as build system. CMake uses `CMakeLists.txt` control files: one in the top level directory and one into each included folders (top level, CSharp wrapper, CSharp wrapper tests).

## Prerequisites

### Windows

*The supported toolchain is Visual Studio 2019 (Community or better).*

First, open a PowerShell terminal as administrator and install chocolatey :
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```
You may need to shut down and restart a Powershell terminal before use the installer provided to install all prerequisites dependencies (Python, Conan, CMake):

```
./installPrerequisites.ps1
```
You may need to shut down and restart powershell.

Try to run the cmake command.
```
cmake --version
```
In case of failure, if the error message says it cannot find cmake, you must add its location to your path from the prompt like this:
```
set PATH="C:\Program Files\CMake\bin\";%PATH%
```

### Ubuntu

Use the installer provided to install all prerequisites dependencies (Python, Conan, CMake):

```
./installPrerequisites
```

#### Ubuntu Conan setup

```
conan profile update settings.compiler.libcxx=libstdc++11 default
```



#### Notes about OpenCV

Conan version of OpenCV is automatically provided.
If you need a custom version of OpenCV (eg. with Qt binding), you have to provide it by yourself.

##### Compiling OpenCV

[OPTIONAL] If opencv is already installed you must uninstall it:
```
  sudo apt-get purge '*opencv*'
```

Using the following command to get the OpenCV source code and prepare the build:
```
git clone https://github.com/opencv/opencv.git
cd opencv && mkdir build && cd build
```

We don't include the examples in the build, but feel free to include them. Also feel free to set other flags and customise your build as you see fit.
```
cmake -D CMAKE_BUILD_TYPE=RELEASE \
      -D CMAKE_INSTALL_PREFIX=/usr/local \
      -D OPENCV_GENERATE_PKGCONFIG=ON \
      -D WITH_TBB=ON \
      -D WITH_V4L=ON \
      -D WITH_QT=ON  \
      -D WITH_GTK=ON  \
      -D WITH_OPENGL=ON ..
```

If CMake didn't report any errors or missing libraries, continue with the build.
```
make -j$(nproc)
```

If no errors were produced, we can carry on with installing OpenCV to the system:
```
sudo make install
```

Now OpenCV should be available to your system. You can use the following lines to know where OpenCV was installed and which libraries were installed:
```
pkg-config --cflags opencv  # get the include path (-I)
pkg-config --libs opencv    # get the libraries path (-L) and the libraries (-l)
```

```
cd ../..
```

## Build project

**NOTE**: Commands here must be entered in Windows terminal (`cmd`) or Git Bash windows.
First `cd`into the algorithm library source code folder, in the Git clone.

If no errors were produced, we can build the project.
The project should be build in a dedicated folder (namely a shadow build). We choosen a subfolder named `build`.
This folder can be deleted and recreated without the fear of loosing any source code.

### On Linux
(Note the cmake special flag `-DLOCAL_OPENCV=true` in case of custom OpenCV) :

#### Release setup
```
mkdir build && cd build                                   # create separate build directory
conan install .. -s build_type=Release --build=missing    # install binary dependencies
cmake .. -DCMAKE_BUILD_TYPE=Release -DLOCAL_OPENCV=true   # generate Makefiles
make -j$(nproc)                                           # compile the code
```

#### Debug setup
```
mkdir build && cd build                               # create separate build directory
conan install .. -s build_type=Debug --build=missing  # install binary dependencies
cmake .. -DCMAKE_BUILD_TYPE=Debug -DLOCAL_OPENCV=true # generate Makefiles
make -j$(nproc)                                       # compile the code
```

### On Windows

#### Release setup
```

rem create separate build directory
mkdir build && cd build

rem install binary dependencies
conan install .. -s build_type=Release --build=missing

rem Create Visual Studio control files
cmake .. -T v142,host=x64 -A x64 -G "Visual Studio 16 2019"

rem build the project
cmake --build . --config Release -j4
```

#### Debug setup
```
rem create separate build directory
mkdir build && cd build

rem install binary dependencies
conan install .. -s build_type=Debug --build=missing

rem Create Visual Studio control files
cmake .. -T v142,host=x64 -A x64 -G "Visual Studio 16 2019"

rem build the project
cmake --build . --config Debug -j4
```

## Create the NuGet package

```
cd build && cpack -G NuGet
```

## Launch unit tests
```
cd build && ctest
```
