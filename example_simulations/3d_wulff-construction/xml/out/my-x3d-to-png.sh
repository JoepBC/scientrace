#!/bin/bash
FILES=$@
for f in $FILES
do
  echo "Processing $f file... to `basename $f x3d`png"
  # take action on each file. $f store current file name
  view3dscene --screenshot 0 ./`basename $f x3d`png $f
done
