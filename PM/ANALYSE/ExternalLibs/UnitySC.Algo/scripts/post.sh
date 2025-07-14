export REPO=_UnityControl/
export LIB_CMAKE="$REPO/PM/ANALYSE/ExternalLibs/UnitySC.Algo/Wrapper/CMakeLists.txt"e

git config --global user.name "ANALYSE release bot"
git config --global user.email "noreply-analyseteam@unity-sc.com"
git config --global http.extraheader "AUTHORIZATION: bearer $(System.AccessToken)"

echo "----------------------------------------------------------------------"
echo ""
echo "             ANALYSE algorithm library post release job               "
echo ""
echo "----------------------------------------------------------------------"

echo "1. Get Develop branch"
cd  "$REPO"
git checkout Develop
git pull --rebase

echo "2. Replace development version v$RELEASE_VERSION by new v$DEV_VERSION"
sed --in-place "s/$RELEASE_VERSION/$DEV_VERSION/" "$LIB_CMAKE"
if [ $? == 1 ]; then
  echo " ERROR: Cannot replace development version v$RELEASE_VERSION by new v$DEV_VERSION"
  exit 1
fi

echo "3. Replace current v$PREVIOUS_VERSION by just released v$RELEASE_VERSION"
 find . -type f -name '*.csproj' -exec grep -F -l "$PREVIOUS_VERSION" {} \; \
    | xargs sed -i "s/$PREVIOUS_VERSION/$RELEASE_VERSION/"

echo "4. GIT operations"
git add "$LIB_CMAKE"
git commit -m "[ANA][ALGO] chore: Bump development version to v$DEV_VERSION" "$LIB_CMAKE"

git add "$REPO"
git commit -m "[ANA][ALGO] chore: Bump algorithm library dependency to v$RELEASE_VERSION"
git push


