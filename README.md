# VRCrypt

## Prerequisites

- Unity Editor version: `2022.3.22f1`

### Link Dlls & Output Directory

`sh link.sh`

### Link FFI

- Update reference to `vrcrypt_lib.cs` in VRCrypt.csproj

## Debug Build

- `dotnet build`

### Build & Replace

- `b.bat`

> A rebuild is required after each rust build

## Release build

- change lib reference in csproj

## TODOs

- encrypt gameobjects
- Process fbx & meshes in rust
  - https://github.com/I3ck/rust-3d
  - https://github.com/lo48576/fbxcel
  - https://github.com/lo48576/fbxcel-dom
  - https://github.com/lo48576/fbx-viewer
