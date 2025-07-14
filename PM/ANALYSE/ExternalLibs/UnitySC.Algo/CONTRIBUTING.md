# UnitySC ANALYSE algorithm


## Development process


The development workflow of the library is explained below:

1. Add or update test file for an algorithm and implement test
2. Add or update header and cpp file for the algorithm
If the new code should be used by a CSharp application:
3. Add or update Wrapper test file
4. Add or update Wrapper source code
Once this is done, push the branch using `git push` command. The continuous integration system then compiles the library and executes the tests. 
If everything goes well, a NuGet package is created and uploaded to the [staging area](https://unitysc.visualstudio.com/UnityControl/_packaging?_a=feed&feed=ANALYSE_staging).
Now a CSharp project can use the new library snapshot for a short time duration (around one month). Dependency in the CSharp project has to be set up manually.
5. If the goal of the update is reached, then a release of the library should be made, to make it permanently available.
The release process will also create a NuGet package, but it will sent to the [release area](https://unitysc.visualstudio.com/UnityControl/_packaging?_a=feed&feed=ANALYSE_release) and will be kept until manual deletion.
The Release shlould be executed using [the Devops dedicated page](https://unitysc.visualstudio.com/UnityControl/_release?_a=releases&view=mine&definitionId=2).
   * Click on "create release" button
   * Choose the "version" to build. Devops expects a Git commit identifier.
   * Configure the three version used for pakaging:
     * `DEV_VERSION`: the **next** library version, which will be the development version. This version can be found in the  `Wrapper/CMakeList.txt` file, for example `set(_VERSION 0.0.32)`.
     * `PREVIOUS_VERSION`: the currently published version. All reference to that version in the ANALYSE project will be automatically bump to the just released version
     * `RELEASE_VERSION`: the current development version, to be released. Will replace PREVIOUS_VERSION in ANALYSE project
  
    * For example, the version 0.0.32 is the current stable release. It means  0.0.33 is the development version, and that number appears in `Wrapper/CMakeList.txt` file. One want to publish 0.0.33 as stable version. The release process will need 0.0.34 as DEV_VERSION (new dev. version), 0.0.32 as PREVIOUS_VERSION, and 0.0.33 as RELEASE_VERSION.

9. Add a description for the release if needed and click "Create" button. That will create the release: all information needed for package creation will be kept. But no compilation starts for now. The release has to be "deployed" for that.
10.  Click now on you new release. A new screen appears, with "Release" and "Stages" sections. Hover "Build and publish" element in "Stages", and click the "Deploy" button which appears. Finally click the "deploy" button on the last window. That trigger the actual release process.

## Branch workflow convention
`Development <-> pull request -> merge`

* Each development is made on its own branch. Branch have to be prefixed by `algos/`.
* A Work In Progress, aka `draft` pull request should not be reviewed.
* Not too much reviewer must be set, but at least one other developer should review the pull request content before its merge into `Develop`.
* An image could be added to the pull request description so that its build status can be tracked. The format is:
```
![Status](https://dev.azure.com/unitysc/UnityControl/_apis/build/status/15?api-version=6.0-preview.1&branchName=branch name here)
```
NOTE: replace 'branch name here' by you branch!

IMPORTANT: packages in the staging area will be deleted after a retention of 30 days.

### Links
* [Open pull requests](https://unitysc.visualstudio.com/_git/UnityControl/pullrequests?_a=active)
* [algorithms pull requests build](https://unitysc.visualstudio.com/UnityControl/_build?definitionId=15)

## How to add source file or tests file

As the build system is CMake, and not directly Visual Studio, files (.hpp, .h, .cpp) should not be added to any solution or project using Visual Studio itself.
Files names must be added in CMake control files, and then project and solution has to be re-generated. Re-generation is automatically made by Visual Studio when recompiling the solution.
It is also possible to do it manually using using following command in `build` directory:

`cmake ..`


### C++ library

To make source code management simpler, top-level directory `files.cmake` text files contains explicit file list for C++ code (CPP_FILES), C++ include files (H_FILES) and C++ libray unit test files (TESTS). To register a new source file to the project, one must add it into the right list and launch `cmake ..`.

### CSharp wrapper

For the CSharp wrapper, the same technique has been applied, and `files.cmake` files can be found in the `Wrapper/` directory. But there is a difference due to the special nature of the Wrapper project. In the `Wrapper` directory, one can found the C++/CLI code doing interface between the C++ and the CSharp worlds. In the `Wrapper/tests` folder, which contains its own `CMakeLists.txt` and  `files.cmake`, one will found CSharp test project, using the wrapper code to ensure data conversion is working.
This test project allow to test the wrapper without using it with another CSharp solution.

## Links

* [Open algorithms pull requests](https://unitysc.visualstudio.com/_git/UnityControl/pullrequests?_a=active&targetRefName=refs%2Fheads%2Falgo)
* [algorithms pull requests build](https://unitysc.visualstudio.com/UnityControl/_build?definitionId=15)

* [staging area](https://unitysc.visualstudio.com/UnityControl/_packaging?_a=feed&feed=ANALYSE_staging)
* [release area](https://unitysc.visualstudio.com/UnityControl/_packaging?_a=feed&feed=ANALYSE_release) 
* [Release page](https://unitysc.visualstudio.com/UnityControl/_release?_a=releases&view=mine&definitionId=2)

## Development

### Coding conventions

By default, we follow the [Google C++ code convention](https://google.github.io/styleguide/cppguide.html).


* General naming convention is CamelCase
* C++ namespaces names are SnakeCase, i.e. *my_namespace*.
* `class` and `struct`, `enum` name first letter is uppercase
* Method first letter is uppercase
* `static const` property are all uppercase
* `#define` names are all uppercase
* `#include` pathes use Unix separator, '/'
* private properties of classes are prefixed by underscore (`_`)
* variables names, as well as method parameters, begin by a lowercase letter
* When possible, methods parameters are passed `const reference` (i.e. `const type& name` )
* C++11 algorithm and libraries could be extensively used
* public content of classes come before protected and private content
* Public documentation take place in header files, where implementation-specific documentation can be added in implementation files
* inside implementation files, internal content can be added into the anonymous namespace ([why?](https://www.learncpp.com/cpp-tutorial/unnamed-and-inline-namespaces/)). In the case of internal code, declaration and documentation should be added at the top of the file, where implementation goes at bottom.
```cpp
// EntryPoint.cpp
namespace {

    /**
    * This is a documentation
    */
    void StepA();

    /**
    * This is a documentation
    */
    void StepB();
}

void EntryPoint::entryPoint()
{
    StepA();
    StepB();
}


namespace {

 void StepA(){

     // impl

 }

 void StepB(){

     // impl

 }
}

```

* use of anonymous namespace (for hidden content) should be avoided in headers.

### References

When an implementation is based on a research paper, reference to the paper should appear in the implementation file (`.cpp` file).


### Logging
A logging system has been implemented, allowing to forward logging message to another application layer.
An event queue is managed as a singleton in the library, and logging are sent to it. In another hand, consumers can be registered in this event queue to receive events from algorithm. For example, the CSharp application on top of algorithm library uses this mecanism to collect and display algorithm events. Also, when in Debug mode, all algorithm messages are automatically sent to `stdout`. 
Roughly, algorithms can send messages at various levels by `#include <Logger.hpp>` and call one of:
* Logger::Verbose(std::string const &message) 
* Logger::Debug(std::string const &message)
* Logger::Warning(std::string const &message)
* Logger::Info(std::string const &message)
* Logger::Error(std::string const &message)
* Logger::Fatal(std::string const &message)

Note: severity levels are the same as in CSharp application.
For details, see `include/EventQueue.hpp` and `include/Logging.hpp`.

