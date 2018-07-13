#! /bin/bash
if [ "$TRAVIS_PULL_REQUEST" == "true" ]; then 
    echo "Building pull request '$TRAVIS_PULL_REQUEST_BRANCH'";

    # Extract CA-number from branch name
    shopt -s nocasematch
    CA_REGEX='(CA-[[:digit:]]+)'

    if ! [[ "$TRAVIS_PULL_REQUEST_BRANCH" =~ $CA_REGEX ]]; then
        echo "Skip pushing to ECR because branch '$TRAVIS_PULL_REQUEST_BRANCH' doesn't contain a task number in the format 'CA-n'."
        exit 0;
    fi

    # Use the CA-number as image tag
    TAG=${BASH_REMATCH[1]}
elif [[ "$TRAVIS_BRANCH" == "master"  || "$TRAVIS_BRANCH" == "Friesland" ]]; then
    echo "Building branch '$TRAVIS_BRANCH' "

    # Use the branch name as image tag
    TAG=${TRAVIS_BRANCH}
else
    echo "Skip pushing to ECR because branch '$TRAVIS_BRANCH' isn't a pull request nor master or Friesland branch."
    exit 0;
fi

# Build 
echo "Building CollAction image for $ECR_REPO:$TAG"
# docker build -t $ECR_REPO:$TAG CollAction

# Log in to Amazon ECR.
# This uses the environment variables AWS_ACCESS_KEY_ID and AWS_ACCESS_SECRET_KEY which are set in the .travis.yml env.secure section. 
echo "Logging on to ECR"
eval $(aws ecr get-login --no-include-email --region eu-central-1)

# Push image to ECR
echo "Pushing image $ECR_REPO:$TAG"
docker push $ECR_REPO:$TAG

echo "Done!"
