#!/bin/bash
FILES=*.png
for f in $FILES
do
  echo -n "Processing crop operation via convert on $f ..."
  # take action on each file. $f store current file name
  # echo "convert -crop $3x$4+$1+$2\! $f ./c_$f"
  convert -crop $3x$4+$1+$2\! $f ./c_$f
  echo -e ['\E[32m'done'\E[0m']
done

