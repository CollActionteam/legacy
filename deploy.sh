#!/bin/sh
if [[ -z "$1" ]] || [[ -z "$2" ]] || [[ -z "$3" ]]; then
   echo "Usage: $0 image_tag AWS_ACCESS_KEY_ID AWS_SECRET_ACCESS_KEY"
   echo
   echo "For example: $0 CA-401 key secret"
   echo "Key and secret can be found on https://my.1password.com/vaults/wnyzxunff2daamk5rkdovq7lly/allitems/uzcgzv4z4e55w36wyyvjfer5lq"
   echo
   exit -1;
fi

docker run -it --rm -e "AWS_ACCESS_KEY_ID=$2" -e "AWS_SECRET_ACCESS_KEY=$3" silintl/ecs-deploy -r eu-central-1 -n CollActionTest -i 156764677614.dkr.ecr.eu-central-1.amazonaws.com/collaction:$1 -c CollActionTest