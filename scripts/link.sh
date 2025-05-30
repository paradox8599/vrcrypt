UNITY="D:/Unity/Editors/Unity 2022.3.22f1"
PROJECT="D:/Unity/Projects/mate"

rm -rf refs out "$PROJECT/RustNative/vrcrypt_lib"
mkdir -p refs
ln -s "$UNITY/Editor/Data/Managed/UnityEditor.dll" "refs/UnityEditor.dll"
ln -s "$UNITY/Editor/Data/Managed/UnityEngine.dll" "refs/UnityEngine.dll"
ln -s "$PROJECT/Packages/com.vrchat.avatars/Runtime/VRCSDK/Plugins/VRCSDK3A.dll" "refs/VRCSDK3A.dll"
ln -s "$PROJECT/Packages/com.vrchat.base/Runtime/VRCSDK/Plugins/VRCSDKBase.dll" "refs/VRCSDKBase.dll"

mkdir -p "$PROJECT/RustNative" && ln -s "$(pwd)/libs/vrcrypt_lib" "$PROJECT/RustNative/vrcrypt_lib"
mkdir -p "$PROJECT/Assets/VRCrypt" && ln -s "$PROJECT/Assets/VRCrypt" out

echo "Done"
