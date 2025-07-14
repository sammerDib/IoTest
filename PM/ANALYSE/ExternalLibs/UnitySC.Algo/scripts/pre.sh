export REPO=_UnityControl/

echo "----------------------------------------------------------------------"
echo "                         ANALYSE algorithm library release"
echo ""
echo "- Library dev version to release                : $RELEASE_VERSION"
echo "- Library used in ANALYSE, to be bump           : $PREVIOUS_VERSION"
echo "- Bump library development version to           : $DEV_VERSION"
echo ""
echo "----------------------------------------------------------------------"

echo "1. Ensure mandatory variables are there"
if [ "$RELEASE_VERSION" == "" ]  ||  [ "$PREVIOUS_VERSION"  == "" ] || [ "$DEV_VERSION" == "" ]; then
  echo " ERROR: missing mandatory variables"
  exit 1
else
  echo " Ok"
fi

echo "2. Check development version in library"
grep -F "$RELEASE_VERSION" "$REPO/PM/ANALYSE/ExternalLibs/UnitySC.Algo/Wrapper/CMakeLists.txt" &>/dev/null
if [ $? -ne 0 ]; then
  echo " ERROR: Version to be released not found in library"
  exit 1
else
  echo " Found previous library version"
fi

echo  "3. Check dependency to upgrade"
CSPROJ_TO_UPDATE=`find . -type f -name '*.csproj' -exec grep -F -l "$PREVIOUS_VERSION" {} \; | wc -l`
if [ $CSPROJ_TO_UPDATE -gt 0 ]; then
  echo " Found $CSPROJ_TO_UPDATE projects to upgrade"
else
  echo " ERROR: Found no project to update, PREVIOUS_VERSION should be bad"
  exit 1
fi

