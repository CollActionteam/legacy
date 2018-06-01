#! /bin/bash
if [ -z "$TRAVIS_PULL_REQUEST" ] || [ "$TRAVIS_PULL_REQUEST" == "false" ]; then 
    # Don't push if it's not a pull request
    echo "Skip pushing to ECR because it's not a pull request";
    exit 0;
fi

# Build and push
echo "Building CollAction image for $ECR_REPO:$TRAVIS_PULL_REQUEST"
docker build -t $ECR_REPO:$TRAVIS_PULL_REQUEST CollAction

# Log in to Amazon ECR.
# This uses the environment variables AWS_ACCESS_KEY_ID and AWS_ACCESS_SECRET_KEY which are set in the .travis.yml env.secure section. 
echo "Logging on to ECR"
eval $(aws ecr get-login --no-include-email --region eu-central-1)

echo "Pushing image $ECR_REPO:$TRAVIS_PULL_REQUEST"
docker push $ECR_REPO:$TRAVIS_PULL_REQUEST

echo "Done!"
