#!/bin/bash

set -e

EXPECTED=0
if [[ "$1" != "" ]] ; then
  EXPECTED="$1"
fi

echo
echo -e -n '\033[0;33mTESTING '
pwd
echo -e -n '\033[m'
mkdir -p .hot_build
rm -rf .hot_build/*

~/hot/mono/hotc . # > .hot_build/hotc-out.txt

cd .hot_build
cat hot-types.ll hot-constants.ll hot-code.ll > all.ll
cat all.ll | /usr/local/opt/llvm/bin/llc -O0 -filetype obj -o me.o
ld -static \
  /usr/local/lib/hot/hot_char.o \
  /usr/local/lib/hot/hot_io.o \
  /usr/local/lib/hot/hot_print.o \
  /usr/local/lib/hot/hot_str.o \
  /usr/local/lib/wee.o \
  /usr/local/lib/pjmalloc.o \
  /usr/local/lib/xisop/xisop_io.o \
  /usr/local/lib/xisop/xisop_print.o \
  /usr/local/lib/xisop/xisop_sys.o \
  /usr/local/lib/xisop/xisop_mman.o \
  /usr/local/lib/hotr.o \
  /usr/local/lib/hotr/rc.o \
  /usr/local/lib/hot/hot.o \
  me.o
echo
echo RUNNING
set +e
./a.out > foo
R=$?
cat foo
echo RESULT $R
set -e
if grep 'used bytes' foo; then
  echo FAILING DUE TO MEMORY LEAK!
  exit 1
fi

if [[ $R != $EXPECTED ]] ; then
  cd ..
  echo
  echo -e -n '\033[0;31mFAILED '$R
  pwd
  echo -e -n '\033[m'
  exit 1
fi

cd ..
set +e
diff main.hot main.hot-fmt
R=$?
set -e

if [[ $R != 0 ]] ; then
  echo
  diff main.hot main.hot-fmt | cat -te
  echo
  echo -e -n '\033[0;31mFORMAT '
  pwd
  echo -e -n '\033[m'
  exit 1
fi
