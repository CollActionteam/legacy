#!/bin/sh
if [ "$TRAVIS_BRANCH" == "master" ]; then
    ECS_SERVICENAME="CollActionAcceptance"
elif [ "$TRAVIS_BRANCH" == "Friesland" ]; then
    ECS_SERVICENAME="CollActionFreonenAcceptance"
else
    echo "Not deploying branch '$TRAVIS_BRANCH' because it's not master or Friesland"
    exit 0
fi

echo "Deploying '$TRAVIS_BRANCH' to $ECS_SERVICENAME"
docker run -it --rm -e "AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID" -e "AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY" silintl/ecs-deploy -r eu-central-1 -n $ECS_SERVICENAME -i 156764677614.dkr.ecr.eu-central-1.amazonaws.com/collaction:$TRAVIS_BRANCH -c CollActionTest
