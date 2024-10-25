# VRCrypt

## Prepare

### Link Dlls

- `ln -s <Unity Editor>\Data\Managed\UnityEditor.dll refs\UnityEditor.dll`
- `ln -s <Unity Editor>\Data\Managed\UnityEngine.dll refs\UnityEngine.dll`
- `ln -s <Project>\Packages\com.vrchat.avatars\Runtime\VRCSDK\Plugins\VRCSDK3A.dll refs\VRCSDK3A.dll`
- `ln -s <Project>\Packages\com.vrchat.base\Runtime\VRCSDK\Plugins\VRCSDKBase.dll refs\VRCSDKBase.dll`
- `ln -s <Project>\Assets\RustNative\vrcrypt_lib\vrcrypt_lib.cs refs\vrcrypt_lib.cs`

### Link Output Directory

- `mkdir -p <Project>\Assets/VRCrypt`
- `ln -s <Project>\Assets\VRCrypt out`

## Build

- `dotnet build`

### Build & Replace

- `build.bat`
