#!/bin/bash
# This script is needed to compile for supported platforms for release in GitHub

execute() {
    echo "$1"
    echo
    eval "$1"

    exitcode=$?
    if [ $exitcode -ne 0 ]; then
        cmd=$(echo $1 | cut -d ' ' -f 1)
        echo "$cmd exited with code: $exitcode"
        read -n 1 -s
        exit 1
    fi
}

appimage_packing() {
    echo "[-------AppImage Packaging-------]"

    execute "mkdir -p publish/AppDir/usr/share/eclairplayer"

    execute "cp -r publish/linux-$1/* publish/AppDir/usr/share/eclairplayer/"

    execute "cp Eclair.Android/Icon.png publish/AppDir/icon.png"
    
    execute "cp AppRun publish/AppDir/"

    execute "cp EclairPlayer.desktop publish/AppDir/"

    if [ -x "appimagetool.AppImage" ]; then
        if [ $1 = "x64" ]; then
            export ARCH="x86_64"
        else
            export ARCH=$1
        fi
        execute "./appimagetool.AppImage publish/AppDir/ publish/linux-$1.AppImage"
    else
        echo The file appimagetool.AppImage does not exist or is not executable.
        read -p "Continue publishing without AppImage? [y/N] " user_input
        
        if [ "$user_input" = "y" ]; then
            return 1
        else
            if [ "$user_input" != "n" ]; then
                echo "Incorrect input... Exiting..."
            fi

            exit 1
        fi
    fi
    
    echo "[-------AppImage Packaging-------]"
}

# ATTENTION: calling this function assumes that the appimage_packing function has already been called,
# and therefore the directory (publish/AppDir/) with the corresponding structure has already been created.    
deb_packing() {
    echo "[----------.deb PACKING----------]"

    rm publish/AppDir/AppRun
    rm publish/AppDir/icon.png

    execute "mkdir publish/AppDir/DEBIAN/"
    execute "mkdir publish/AppDir/usr/bin/"
    execute "mkdir publish/AppDir/usr/share/applications"

    execute "sed -i '4s;.*;Icon=/usr/share/eclairplayer/icon.png;' publish/AppDir/EclairPlayer.desktop"

    execute "cp Eclair.Android/Icon.png publish/AppDir/usr/share/eclairplayer/icon.png"
    execute "mv publish/AppDir/EclairPlayer.desktop publish/AppDir/usr/share/applications/"
    
    if [ $1 = "x64" ]; then
        export ARCH="amd64"
    fi
    echo "Package: eclairplayer
Version: 0.3.0
Architecture: $ARCH
Maintainer: Ilya Korobov <nonexisplayer@gmail.com>
Description: An open-source .NET audio player." >> publish/AppDir/DEBIAN/control
    
    execute "ln -s /usr/share/eclairplayer/eclairplayer publish/AppDir/usr/bin/eclairplayer"

    if [ -x "/bin/dpkg-deb" ]; then
        execute "dpkg-deb --build publish/AppDir/ publish/linux-$1.deb"
    else
        echo The dpkg-deb executable was not found.
        echo "Install dpkg-deb (or dpkg) in your package manager and try again."
        read -p "Continue publishing without .deb packing? [y/N] " user_input
        
        if [ "$user_input" = "y" ]; then
            return 1
        else
            if [ "$user_input" != "n" ]; then
                echo "Incorrect input... Exiting..."
            fi

            exit 1
        fi
    fi

    echo "[----------.deb PACKING----------]"
}

build() {
    # $1 = project
    # $2 = platform
    # $3 = arch
    echo "[------------Building------------]"
    echo Starting building $1 for $2-$3...
    if [ "$1" = "Eclair.Desktop" ]; then
        execute "dotnet publish $1 -c Release -r $2-$3 -p:PublishSingleFile=true --sc -o publish/$2-$3"
        if [ $2 = "linux" ]; then
            execute "cp Eclair.Desktop/runtimes/linux-$3/native/*.* publish/$2-$3"
        fi
    else
        execute "dotnet build $1 -c Release -o publish/$2-$3/"
    fi
    echo "[------------Building------------]"
    echo
    if [ "$1" = "Eclair.Desktop" ]; then
        echo "[-----------Packaging------------]"
        if [ "$2" = "linux" ]; then
            execute "7z a -ttar publish/$2-$3.tar publish/$2-$3"
            execute "7z a -tgzip publish/$2-$3.tar.gz publish/$2-$3.tar"
            execute "rm publish/$2-$3.tar"
            appimage_packing $3
            deb_packing $3
        else
            execute "7z a -tzip publish/$2-$3.zip publish/$2-$3"
        fi
        echo "[-----------Packaging------------]"
    else
        mv publish/android-$3/net.nonexistplayer.eclair-Signed.apk publish/android-$3.apk
    fi
    echo "[------------Cleaning------------]"
    execute "rm -r publish/$2-$3"
    execute "rm -r publish/AppDir"
    echo $1 $2-$3 done!
    clear
}

clear

build Eclair.Desktop win x86
build Eclair.Desktop win x64
build Eclair.Desktop linux x64
build Eclair.Desktop linux arm64

build Eclair.Android android x64