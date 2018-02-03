#! /bin/bash
if [ -z "$TRAVIS_PULL_REQUEST" ] && [ "$TRAVIS_PULL_REQUEST" == "true" ]; then 
    # Don't push if it's a pull request
    echo "Skipping deploy because it's a pull request";
    exit 0;
fi

if [ "$TRAVIS_BRANCH" != "$DEPLOY_BRANCH" ]; then
    # Don't push if it's not the correct branch
    echo "Skipping deploy because branch is not '$DEPLOY_BRANCH'"
    exit 0;
fi

# Build and push
echo "Building $DOCKER_REPO"
docker build -t $DOCKER_REPO CollAction

echo "Pushing $DOCKER_REPO"
echo $DOCKER_PASSWORD | docker login -u=$DOCKER_USERNAME --password-stdin
docker push $DOCKER_REPO

echo "Pushed $DOCKER_REPO"
