#!/usr/bin/bash

RED='\033[0;31m'
GREEN='\033[0;32m'

SOURCEFILE=$(mktemp)
trap "rm -f $SOURCEFILE" 0 1 2 3 15

dd if=/dev/urandom of=$SOURCEFILE bs=1M count=1

HASHSOURCE=($(md5sum $SOURCEFILE))

JSONUPLOADRESPONSE=$(CloudCopy upload ServiceRequest:\#2 $SOURCEFILE -o json)

C4CFILENAME=$(jq '.[0].Filename' <<< $JSONUPLOADRESPONSE | tr -d '"')

JSONDOWNLOADRESPONSE=$(CloudCopy download ServiceRequest:\#2 -p $C4CFILENAME -o json)

HASHDOWNLOAD=($(md5sum $C4CFILENAME))

rm -f $C4CFILENAME

if [ $HASHDOWNLOAD = $HASHSOURCE ]; then
  printf "${GREEN}Test of upload and download successful\n"
  exit 0
else
  printf "${RED}File download error\n"
  exit 1000
fi