#!/bin/sh

# stable basis for relative paths
MY_DIR=`dirname $0`
MY_DIR=`cd "$MY_DIR" > /dev/null; pwd`

BUILD="$MY_DIR/../../../build"
DEBUG_DIR="$BUILD/bin/Debug"
OUT_DIR="$BUILD/doc"
HTML_DIR="$BUILD/html"
rm -rf "$OUT_DIR" "$HTML_DIR"
mkdir -p "$OUT_DIR"
cp -r "$MY_DIR/core" "$MY_DIR/constraints-nunit2" "$MY_DIR/constraints-nunit3" "$OUT_DIR"

monodocer -pretty -importslashdoc:"$DEBUG_DIR/xmlunit-core.XML"\
          -assembly:"$DEBUG_DIR/xmlunit-core.dll"\
          -path:"$OUT_DIR/core"
monodocer -pretty -importslashdoc:"$DEBUG_DIR/xmlunit-nunit3-constraints.XML"\
          -assembly:"$DEBUG_DIR/xmlunit-nunit3-constraints.dll"\
          -path:"$OUT_DIR/constraints-nunit3"
monodocer -pretty -importslashdoc:"$DEBUG_DIR/xmlunit-nunit2-constraints.XML"\
          -assembly:"$DEBUG_DIR/xmlunit-nunit2-constraints.dll"\
          -path:"$OUT_DIR/constraints-nunit2"

monodocs2html -out "$HTML_DIR" "$OUT_DIR/core" "$OUT_DIR/constraints-nunit3" "$OUT_DIR/constraints-nunit2"

