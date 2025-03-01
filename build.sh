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

build() {
    # $1 = project
    # $2 = platform
    # $3 = arch
    echo "[------------Building------------]"
    echo Starting building $1 for $2-$3...
    if [ "$1" = "Eclair.Desktop" ]; then
        execute "dotnet build $1 -c Release --os $2 -a $3 -v m -o publish/$2-$3 --sc"
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
        else
            execute "7z a -tzip publish/$2-$3.zip publish/$2-$3"
        fi
        echo "[-----------Packaging------------]"
    else
        move publish/android-$3/net.nonexistplayer.eclair-Signed.apk publish/android-$3.apk
    fi
    echo "[------------Cleaning------------]"
    execute "rm -r publish/$2-$3"
    echo $1 $2-$3 done!
    clear
}

clear

build Eclair.Desktop win x86
build Eclair.Desktop win x64
build Eclair.Desktop linux x64
build Eclair.Desktop linux arm64

build Eclair.Android android x64