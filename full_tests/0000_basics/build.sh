#!/bin/bash

set -e

cd "${BASH_SOURCE%/*}/"

for I in `ls | grep 0` ; do
  cd $I
  bash ./build.sh
done

echo
echo SUCCESS
